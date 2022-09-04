// Copyright (c) EverRise Pte Ltd. All rights reserved.

using System.Net.Http.Headers;
using System.Text.Json;

using EverStats.Config;
using EverStats.Data;
using Nethereum.Web3;
using Contracts.Contracts.EverRise;
using System.Runtime.ExceptionServices;
using Microsoft.Data.SqlClient;
using Nethereum.RPC.Eth.DTOs;
using System.Data;
using Nethereum.Signer;

namespace EverStats.Services;
public class QueryTime : IHostedService
{
    private readonly static MediaTypeWithQualityHeaderValue s_jsonAccept = MediaTypeWithQualityHeaderValue.Parse("application/json");
    private readonly ILogger<BlockchainQuery> _logger;
    private readonly ApiConfig _config;
    private readonly HolderList _holders;
    private CancellationTokenSource _cts = new CancellationTokenSource();

    private HttpClient _stakingClient = GetHttpClient();

    private BlockchainQuery _bscQuery;
    private BlockchainQuery _ethQuery;
    private BlockchainQuery _polyQuery;
    private BlockchainQuery _ftmQuery;
    private BlockchainQuery _avaxQuery;


    private async Task<int> QueryBlock(string chain, DateTimeOffset date)
    {
        var timeStamp = date.ToUnixTimeSeconds();
        var response = await _stakingClient.GetAsync($"https://deep-index.moralis.io/api/v2/dateToBlock?chain={chain}&date={timeStamp:0}");
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadAsStringAsync();
        var nearest = JsonSerializer.Deserialize<NearestBlock>(data);
        return nearest.block;
    }

    private static HttpClient GetHttpClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(s_jsonAccept);
        return client;
    }

    private async Task<(BlockchainSample unified, BlockchainSample eth, BlockchainSample bsc, BlockchainSample poly, BlockchainSample ftm, BlockchainSample avax)> 
        Calculate(DateTime date, bool storeInDb)
    {
        var holders = await _holders.GetHolderList();

        var ethBlock = (ulong)await QueryBlock("eth", date);
        var ethTask = Query(_ethQuery, ethBlock);

        var bscBlock = (ulong)await QueryBlock("bsc", date);
        var bscTask = Query(_bscQuery, bscBlock);

        var polyBlock = (ulong)await QueryBlock("polygon", date);
        var polyTask = Query(_polyQuery, polyBlock);


        var avaxBlock = (ulong)await QueryBlock("avalanche", date);
        var avaxTask = Query(_avaxQuery, avaxBlock);

        var ftmBlock = (ulong)await QueryBlock("fantom", date);
        
        Task<BlockchainSample> ftmTask;
        try
        {
            ftmTask = Query(_ftmQuery, ftmBlock);
            await ftmTask;
        }
        catch (Exception ex)
        {
            ftmBlock = (ulong)await QueryBlock("fantom", date.AddSeconds(-30));
            ftmTask = Query(_ftmQuery, ftmBlock);
            await ftmTask;
        }


        await Task.WhenAll(ethTask, bscTask, polyTask, ftmTask, avaxTask);

        var eth = await ethTask;
        var bsc = await bscTask;
        var poly = await polyTask;
        var avax = await avaxTask;
        var ftm = await ftmTask;

        await Task.WhenAll(
            SetLocked(1, eth, holders, _ethQuery.ArchiveEndpoints, ethBlock),
            SetLocked(56, bsc, holders, _bscQuery.ArchiveEndpoints, bscBlock),
            SetLocked(137, poly, holders, _polyQuery.ArchiveEndpoints, polyBlock),
            SetLocked(250, ftm, holders, _ftmQuery.ArchiveEndpoints, ethBlock),
            SetLocked(43114, avax, holders, _avaxQuery.ArchiveEndpoints, ethBlock));

        var unified = Recalculate(eth, bsc, poly, ftm, avax);

        if (storeInDb)
        {
            await StoreInDb(unified, eth, bsc, poly, ftm, avax);
        }

        return (unified, eth, bsc, poly, ftm, avax);
    }

    private async Task SetLocked(int chainId, BlockchainSample quatities, string[] addresses, EndPoints endpoints, ulong blockNumber)
    {
        (_, var amount) = await GetLocked(chainId, addresses, endpoints, blockNumber);
        quatities.stakedValue = (decimal)amount;
        quatities.aveMultiplierValue = quatities.veAmountValue / quatities.stakedValue;
        quatities.usdStakedValue = quatities.stakedValue * quatities.tokenPriceStableValue;
    }

    public class StakeAmounts
    {
        public string id { get; set; }
        public double amount { get; set; }
    }

    private async Task StoreInDb(BlockchainSample unified, BlockchainSample eth, BlockchainSample bsc, BlockchainSample poly, BlockchainSample ftm, BlockchainSample avax)
    {
        using var conn = new SqlConnection(_config.AzureConfiguration.SqlConnection);
        await conn.OpenAsync();

        await StoreInDb(chainId: 0, unified, conn);

        await StoreInDb(chainId: 1, eth, conn);
        await StoreInDb(chainId: 56, bsc, conn);
        await StoreInDb(chainId: 137, poly, conn);
        await StoreInDb(chainId: 250, ftm, conn);
        await StoreInDb(chainId: 43114, avax, conn);
    }

    private async Task StoreInDb(int chainId, BlockchainSample sample, SqlConnection conn)
    {
        try
        {
            if (sample.date != sample.lastStored)
            {
                using var cmd = new SqlCommand(@"
            INSERT INTO chain_time_data
                ([date], [chainId],
                [reservesCoinBalance], [reservesTokenBalance], [liquidityToken], [liquidityCoin], [veAmount], [staked], 
                [aveMultiplier], [rewards], [volumeBuy], [volumeSell], [volumeTrade], [bridgeVault], 
                [tokenPriceCoin], [coinPriceStable], [tokenPriceStable], [marketCap], [blockNumber], [holders], [burn], 
                [burnPercent], [totalSupply], [everSwap], [usdReservesCoinBalance], [usdReservesTokenBalance], [usdReservesBalance], 
                [usdLiquidityToken], [usdLiquidityCoin], [usdStaked], [usdRewards], [usdVolumeBuy], 
                [usdVolumeSell], [usdVolumeTrade], [usdEverSwap], [supplyOnChainPercent], [stakedOfTotalSupplyPercent], 
                [stakedOfOnChainPercent], [stakedOfTotalStakedPercent], [veRiseOnChainPercent], [unclaimedTokenBalance], [usdUnclaimedTokenBalance],
                [stakesCount], [mementosCount])
            VALUES
                (@date, @chainId, 
                @reservesCoinBalance, @reservesTokenBalance, @liquidityToken, @liquidityCoin, @veAmount, @staked, 
                @aveMultiplier, @rewards, @volumeBuy, @volumeSell, @volumeTrade, @bridgeVault, 
                @tokenPriceCoin, @coinPriceStable, @tokenPriceStable, @marketCap, @blockNumber, @holders, @burn, 
                @burnPercent, @totalSupply, @everSwap, @usdReservesCoinBalance, @usdReservesTokenBalance, @usdReservesBalance, 
                @usdLiquidityToken, @usdLiquidityCoin, @usdStaked, @usdRewards, @usdVolumeBuy, 
                @usdVolumeSell, @usdVolumeTrade, @usdEverSwap, @supplyOnChainPercent, @stakedOfTotalSupplyPercent, 
                @stakedOfOnChainPercent, @stakedOfTotalStakedPercent, @veRiseOnChainPercent, @unclaimedTokenBalance, @usdUnclaimedTokenBalance,
                @stakesCount, @mementosCount)", conn);

                var param = cmd.Parameters;
                param.AddWithValue("@date", sample.date);
                param.AddWithValue("@chainId", chainId);
                param.AddWithValue("@reservesTokenBalance", sample.reservesTokenBalanceValue);
                param.AddWithValue("@reservesCoinBalance", sample.reservesCoinBalanceValue);
                param.AddWithValue("@liquidityToken", sample.liquidityTokenValue);
                param.AddWithValue("@liquidityCoin", sample.liquidityCoinValue);
                param.AddWithValue("@veAmount", sample.veAmountValue);
                param.AddWithValue("@staked", sample.stakedValue);
                param.AddWithValue("@aveMultiplier", sample.aveMultiplierValue);
                param.AddWithValue("@rewards", sample.rewardsValue);
                param.AddWithValue("@volumeBuy", sample.volumeBuyValue);
                param.AddWithValue("@volumeSell", sample.volumeSellValue);
                param.AddWithValue("@volumeTrade", sample.volumeTradeValue);
                param.AddWithValue("@bridgeVault", sample.bridgeVaultValue);
                param.AddWithValue("@tokenPriceCoin", sample.tokenPriceCoinValue);
                param.AddWithValue("@coinPriceStable", sample.coinPriceStableValue);
                param.AddWithValue("@tokenPriceStable", sample.tokenPriceStableValue);
                param.AddWithValue("@marketCap", sample.marketCapValue);
                param.AddWithValue("@blockNumber", sample.blockNumberValue);
                param.AddWithValue("@timeStamp", sample.timeStampValue);
                param.AddWithValue("@holders", sample.holdersValue);
                param.AddWithValue("@burn", sample.burnValue);
                param.AddWithValue("@burnPercent", sample.burnPercentValue);
                param.AddWithValue("@totalSupply", sample.totalSupplyValue);
                param.AddWithValue("@everSwap", sample.everSwapValue);
                param.AddWithValue("@usdReservesTokenBalance", sample.usdReservesTokenBalanceValue);
                param.AddWithValue("@usdReservesCoinBalance", sample.usdReservesCoinBalanceValue);
                param.AddWithValue("@usdReservesBalance", sample.usdReservesBalanceValue);
                param.AddWithValue("@usdLiquidityToken", sample.usdLiquidityTokenValue);
                param.AddWithValue("@usdLiquidityCoin", sample.usdLiquidityCoinValue);
                param.AddWithValue("@usdStaked", sample.usdStakedValue);
                param.AddWithValue("@usdRewards", sample.usdRewardsValue);
                param.AddWithValue("@usdVolumeBuy", sample.usdVolumeBuyValue);
                param.AddWithValue("@usdVolumeSell", sample.usdVolumeSellValue);
                param.AddWithValue("@usdVolumeTrade", sample.usdVolumeTradeValue);
                param.AddWithValue("@usdEverSwap", sample.usdEverSwapValue);
                param.AddWithValue("@supplyOnChainPercent", sample.supplyOnChainPercentValue);
                param.AddWithValue("@stakedOfTotalSupplyPercent", sample.stakedOfTotalSupplyPercentValue);
                param.AddWithValue("@stakedOfOnChainPercent", sample.stakedOfOnChainPercentValue);
                param.AddWithValue("@stakedOfTotalStakedPercent", sample.stakedOfTotalStakedPercentValue);
                param.AddWithValue("@veRiseOnChainPercent", sample.veRiseOnChainPercentValue);
                param.AddWithValue("@unclaimedTokenBalance", sample.unclaimedTokenBalanceValue);
                param.AddWithValue("@usdUnclaimedTokenBalance", sample.usdUnclaimedTokenBalanceValue);
                param.AddWithValue("@stakesCount", sample.stakesCountValue);
                param.AddWithValue("@mementosCount", sample.mementosCountValue);

                await cmd.ExecuteNonQueryAsync();

                sample.lastStored = sample.date;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }

    private BlockchainSample? Recalculate(params BlockchainSample[] samples)
    {
        const decimal maxSupply = 71618033988m;

        var supplyTotal = 0m;
        var supplyWeights = new decimal[samples.Length];
        var liquidityTotal = 0m;
        var liquidityWeights = new decimal[samples.Length];
        var stakingTotal = 0m;
        var stakingWeights = new decimal[samples.Length];
        var timeStampValue = 0m;

        var burnTotal = 0m;
        for (var i = 0; i < samples.Length; i++)
        {
            var chain = samples[i];
            if (chain is null) continue;

            burnTotal += chain.burnValue;
            timeStampValue = Math.Max(timeStampValue, chain.timeStampValue);
        }

        decimal totalSupply = maxSupply - burnTotal;

        for (var i = 0; i < samples.Length; i++)
        {
            var chain = samples[i];
            if (chain is null) continue;

            var supply = chain.bridgeVaultValue == 0 ? 0 : maxSupply - chain.bridgeVaultValue;
            supplyTotal += supply;
            supplyWeights[i] = supply;

            var liquidity = chain.usdLiquidityCoinValue;
            liquidityTotal += liquidity;
            liquidityWeights[i] = liquidity;

            var staking = chain.stakedValue;
            stakingTotal += staking;
            stakingWeights[i] = staking;
        }

        if (supplyTotal == 0) return null;

        for (var i = 0; i < samples.Length; i++)
        {
            supplyWeights[i] /= supplyTotal;
            if (liquidityTotal > 0)
            {
                liquidityWeights[i] /= liquidityTotal;
            }
            stakingWeights[i] /= stakingTotal;
        }

        decimal reservesCoinBalanceValue = -1m;
        decimal reservesTokenBalanceValue = 0m;
        decimal liquidityTokenValue = 0m;
        decimal liquidityCoinValue = -1m;
        decimal stakedValue = stakingTotal;
        decimal aveMultiplierValue = 0m;
        decimal rewardsValue = 0m;
        decimal volumeTransfersValue = 0m;
        decimal volumeBuyValue = 0m;
        decimal volumeSellValue = 0m;
        decimal volumeTradeValue = 0m;
        decimal tokenPriceStableValue = 0m;
        decimal marketCapValue = 0m;
        decimal holdersValue = 0m;
        decimal veAmountValue = 0m;
        decimal unclaimedTokenBalanceValue = 0m;

        decimal usdLiquidityCoinValue = 0m;
        decimal usdEverSwapValue = 0m;
        decimal usdReservesCoinBalanceValue = 0m;
        decimal usdReservesTokenBalanceValue = 0m;
        decimal usdReservesBalanceValue = 0m;
        decimal usdUnclaimedTokenBalanceValue = 0m;
        decimal stakesCountValue = 0m;
        decimal mementosCountValue = 0m;

        for (var i = 0; i < samples.Length; i++)
        {
            var chain = samples[i];
            if (chain is null) continue;

            liquidityTokenValue += chain.liquidityTokenValue;
            aveMultiplierValue += chain.aveMultiplierValue * stakingWeights[i];
            rewardsValue += chain.rewardsValue;
            volumeTransfersValue += chain.volumeTransfersValue;
            volumeBuyValue += chain.volumeBuyValue;
            volumeSellValue += chain.volumeSellValue;
            volumeTradeValue += chain.volumeTradeValue;

            tokenPriceStableValue += chain.tokenPriceStableValue * liquidityWeights[i];
            marketCapValue += chain.marketCapValue * liquidityWeights[i];

            veAmountValue += chain.veAmountValue;
            holdersValue += chain.holdersValue;

            reservesTokenBalanceValue += chain.reservesTokenBalanceValue;
            usdLiquidityCoinValue += chain.usdLiquidityCoinValue;
            usdEverSwapValue += chain.usdEverSwapValue;

            usdReservesTokenBalanceValue += chain.usdReservesTokenBalanceValue;
            usdReservesCoinBalanceValue += chain.usdReservesCoinBalanceValue;
            usdReservesBalanceValue += chain.usdReservesBalanceValue;
            unclaimedTokenBalanceValue += chain.unclaimedTokenBalanceValue;
            usdUnclaimedTokenBalanceValue += chain.usdUnclaimedTokenBalanceValue;
            stakesCountValue += chain.stakesCountValue;
            mementosCountValue += chain.mementosCountValue;
        }

        decimal usdStakedValue = stakedValue * tokenPriceStableValue;
        decimal usdRewardsValue = rewardsValue * tokenPriceStableValue;
        decimal usdLiquidityTokenValue = liquidityTokenValue * tokenPriceStableValue;
        decimal usdVolumeTransfersValue = volumeTransfersValue * tokenPriceStableValue;
        decimal usdVolumeBuyValue = volumeBuyValue * tokenPriceStableValue;
        decimal usdVolumeSellValue = volumeSellValue * tokenPriceStableValue;
        decimal usdVolumeTradeValue = volumeTradeValue * tokenPriceStableValue;

        for (var i = 0; i < samples.Length; i++)
        {
            var chain = samples[i];
            if (chain is null) continue;

            var supplyOnChainPercentValue = supplyWeights[i];
            chain.supplyOnChainPercentValue = supplyOnChainPercentValue;
            chain.stakedOfTotalSupplyPercentValue = totalSupply == 0 ? 0 : chain.stakedValue / totalSupply;
            chain.stakedOfOnChainPercentValue = supplyOnChainPercentValue == 0 ? 0 : chain.stakedValue / (supplyOnChainPercentValue * totalSupply);
            chain.stakedOfTotalStakedPercentValue = totalSupply == 0 ? 0 : chain.stakedValue / stakingTotal;
            chain.veRiseOnChainPercentValue = veAmountValue == 0 ? 0 : chain.veAmountValue / veAmountValue;
        }

        var sample = new BlockchainSample()
        {
            timeStampValue = timeStampValue,
            date = (DateTime.UnixEpoch + TimeSpan.FromSeconds((int)timeStampValue)).ToString("s"),
            reservesCoinBalanceValue = reservesCoinBalanceValue,
            reservesTokenBalanceValue = reservesTokenBalanceValue,
            liquidityTokenValue = liquidityTokenValue,
            liquidityCoinValue = liquidityCoinValue,
            stakedValue = stakedValue,
            aveMultiplierValue = aveMultiplierValue,
            rewardsValue = rewardsValue,
            veAmountValue = veAmountValue,
            volumeTransfersValue = volumeTransfersValue,
            volumeBuyValue = volumeBuyValue,
            volumeSellValue = volumeSellValue,
            volumeTradeValue = volumeTradeValue,
            bridgeVaultValue = -1m,
            tokenPriceCoinValue = -1m,
            coinPriceStableValue = -1m,
            tokenPriceStableValue = tokenPriceStableValue,
            marketCapValue = marketCapValue,
            holdersValue = holdersValue,
            burnValue = burnTotal,
            burnPercentValue = burnTotal / maxSupply,
            totalSupplyValue = maxSupply - burnTotal,
            everSwapValue = -1m,
            blockNumberValue = -1m,
            usdLiquidityTokenValue = usdLiquidityTokenValue,
            usdLiquidityCoinValue = usdLiquidityCoinValue,
            usdStakedValue = usdStakedValue,
            usdRewardsValue = usdRewardsValue,
            usdVolumeTransfersValue = usdVolumeTransfersValue,
            usdVolumeBuyValue = usdVolumeBuyValue,
            usdVolumeSellValue = usdVolumeSellValue,
            usdVolumeTradeValue = usdVolumeTradeValue,
            usdEverSwapValue = usdEverSwapValue,

            supplyOnChainPercentValue = 1m,
            stakedOfTotalSupplyPercentValue = stakingTotal / totalSupply,
            stakedOfOnChainPercentValue = stakedValue / totalSupply,
            stakedOfTotalStakedPercentValue = 1m,
            veRiseOnChainPercentValue = 1m,

            usdReservesCoinBalanceValue = usdReservesCoinBalanceValue,
            usdReservesTokenBalanceValue = usdReservesTokenBalanceValue,
            usdReservesBalanceValue = usdReservesBalanceValue,
            unclaimedTokenBalanceValue = unclaimedTokenBalanceValue,
            usdUnclaimedTokenBalanceValue = usdUnclaimedTokenBalanceValue,
            stakesCountValue = stakesCountValue,
            mementosCountValue = mementosCountValue
        };

        sample.CreateStringRepresentations();

        return sample;
    }

    private async Task<BlockchainSample> Query(BlockchainQuery query, ulong blockNumber)
    {
        return await query.GetData(blockNumber);
    }

    private static async Task<decimal> GetLocked(Web3 web3, string[] addresses, ulong blockNumber)
    {
        var everRise = new EverRiseService(web3, "0xC17c30e98541188614dF99239cABD40280810cA3");

        var lockedAmount = await everRise.GetAmountLockedUsingMultiCallAsync(addresses, numberOfCallsPerRequest: 2000, block: new BlockParameter(blockNumber));

        var totalStaked = lockedAmount.Select(x => Nethereum.Util.UnitConversion.Convert.FromWei(x)).Sum();

        return totalStaked;
    }

    private async Task<(int ChainId, decimal Locked)> GetLocked(int chainId, string[] addresses, EndPoints endpoints, ulong blockNumber)
    {
        Exception ex = null;
        foreach (var endpoint in endpoints)
        {
            try
            {
                var data = (chainId, await GetLocked(new Web3(endpoint), addresses, blockNumber));
                _logger.LogInformation($"Locked Tokens for {chainId} is {data.Item2}");

                return data;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                endpoints.FailedEndpoint(endpoint);
                ex = e;
            }
        }

        ExceptionDispatchInfo.Throw(ex);
        throw ex;
    }

    public QueryTime(ApiConfig config, HolderList holders, ILogger<BlockchainQuery> logger)
    {
        _logger = logger;
        _config = config;
        _holders = holders;

        _stakingClient.DefaultRequestHeaders.Add("X-API-Key", config.MoralisConfiguration.Web3ApiKey);

        var speedyKey = config.MoralisConfiguration.SpeedyNodeKey;

        _bscQuery = new BlockchainQuery("bsc",
            config,
            null,
            currentEndpoints: new[] {
                "https://bsc-dataseed.binance.org/",
                "https://rpc.ankr.com/bsc",
                "https://bsc-dataseed1.defibit.io/",
                "https://bsc-dataseed1.ninicoin.io/",
                "https://bsc-dataseed2.defibit.io/",
                "https://bsc-dataseed3.defibit.io/",
                "https://bsc-dataseed4.defibit.io/",
                "https://bsc-dataseed2.ninicoin.io/",
                "https://bsc-dataseed3.ninicoin.io/",
                "https://bsc-dataseed4.ninicoin.io/",
                "https://bsc-dataseed1.binance.org/",
                "https://bsc-dataseed2.binance.org/",
                "https://bsc-dataseed3.binance.org/",
                "https://bsc-dataseed4.binance.org/",
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/bsc/mainnet"
            },
            archiveEndpoints: new[]
            {
                "https://bsc-mainnet.nodereal.io/v1/77a6ad1e2ca847ebae6f3632119c2bba",
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/bsc/mainnet/archive",
                "https://rpc.ankr.com/bsc",
            },
            statsContractAddress: "0x889f26f688f0b757F84e5C07bf9FeC6D6c368Af2",
            everSwapAddress: "0x71B089280237672837a23d3c2cC6cFC43424e08E",
            logger: logger);

        _ethQuery = new BlockchainQuery("eth",
            config,
            null,
            currentEndpoints: new[] {
                "https://rpc.ankr.com/eth",
                "https://mainnet.infura.io/v3/00df9e302326440a8c6c35255a17c265",
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/eth/mainnet",
                "https://main-light.eth.linkpool.io",
                "https://eth-rpc.gateway.pokt.network",
                "https://mainnet-nethermind.blockscout.com",
                "https://rpc.flashbots.net",
                "https://api.mycryptoapi.com/eth",
                "https://cloudflare-eth.com",
                "https://main-rpc.linkpool.io",
            },
            archiveEndpoints: new[]
            {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/eth/mainnet/archive",
                "https://rpc.ankr.com/eth",
            },
            statsContractAddress: "0x889f26f688f0b757F84e5C07bf9FeC6D6c368Af2",
            everSwapAddress: "0x8dc9b4e0a5688fa4869d438d8720c9621fa777dc",
            logger: logger);

        _polyQuery = new BlockchainQuery("polygon",
            config,
            null,
            currentEndpoints: new[] {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/polygon/mainnet",
                "https://rpc-mainnet.maticvigil.com/v1/9f9c72670ae63ead023c7cb64594c31c021e7e14",
                "https://polygon-rpc.com/",
                "https://rpc.ankr.com/polygon",
            },
            archiveEndpoints: new[]
            {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/polygon/mainnet/archive",
                "https://rpc.ankr.com/polygon"
            },
            statsContractAddress: "0x889f26f688f0b757F84e5C07bf9FeC6D6c368Af2",
            everSwapAddress: "0xa3C14Dfb714b9a7827393DC282ee3027e39B5557",
            logger: logger);

        _avaxQuery = new BlockchainQuery("avalanche",
            config,
            null,
            currentEndpoints: new[] {
                "https://rpc.ankr.com/avalanche",
                "https://ava-mainnet.public.blastapi.io/ext/bc/C/rpc",
                "https://avalancheapi.terminet.io/ext/bc/C/rpc",
                "https://rpc.ankr.com/avalanche-c",
                "https://api.avax.network/ext/bc/C/rpc",
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/avalanche/mainnet"
            },
            archiveEndpoints: new string[]
            {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/avalanche/mainnet"
            },
            statsContractAddress: "0x889f26f688f0b757F84e5C07bf9FeC6D6c368Af2",
            everSwapAddress: "0x6d9fA4Fb73942A416d89ad3f7553eFefF9b3F74B",
            logger: logger);

        _ftmQuery = new BlockchainQuery("fantom",
            config,
            null,
            currentEndpoints: new[] {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/fantom/mainnet",
                "https://rpcapi.fantom.network/",
                "https://rpc.ankr.com/fantom",
                "https://rpcapi.fantom.network",
            },
            archiveEndpoints: new string[]
            {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/fantom/mainnet",
                "https://rpc.ankr.com/fantom",
                "https://fantom-mainnet.public.blastapi.io",
                "https://rpc.ftm.tools",
                "https://rpcapi.fantom.network",
                "https://rpc2.fantom.network",
                "https://rpc.fantom.network",
                "https://rpc3.fantom.network"
            },
            statsContractAddress: "0x889f26f688f0b757F84e5C07bf9FeC6D6c368Af2",
            everSwapAddress: "0x557E769FC676a07fd04af671037EFfa218DB3F4E",
            logger: logger);
    }


    public async Task<DateTime> GetOldestDataDate()
    {
        using var conn = new SqlConnection(_config.AzureConfiguration.SqlConnection);
        using var cmd = new SqlCommand(@"SELECT earliestDate = Min([date]) FROM chain_time_data", conn);

        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();

        var data = new List<ChainData>();
        while (await reader.ReadAsync())
        {
            return reader.GetDateTime("earliestDate");
        }

        throw new Exception("No Data");
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var date = await GetOldestDataDate();
        while (!cancellationToken.IsCancellationRequested)
        {
            date = date.AddHours(-1);

            var result = await Calculate(date, storeInDb: true);

            Console.WriteLine($"block at {result.unified.date:dddd dd MMM yyyy HH:mm:ss}");

            await Task.Delay(15_000);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }
}