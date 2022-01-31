// Copyright (c) EverRise Pte Ltd. All rights reserved.

using System.IO.Compression;
using System.Text;
using System.Text.Json;

using EverStats.Config;
using EverStats.Data;

namespace EverStats.Services;
public class Stats : IHostedService
{
    private static JsonSerializerOptions _options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    private CancellationTokenSource _cts = new CancellationTokenSource();
    private SemaphoreSlim _calcSemaphore = new SemaphoreSlim(0);
    private SemaphoreSlim _serializeSemaphore = new SemaphoreSlim(0);
    private SemaphoreSlim _dataSemaphore = new SemaphoreSlim(0);

    private BlockchainQuery _bscQuery;
    private BlockchainQuery _ethQuery;
    private BlockchainQuery _polyQuery;
    private BlockchainQuery _ftmQuery;
    private BlockchainQuery _avaxQuery;

    public BlockchainStats unified { get; set; } = new BlockchainStats("unified");
    public BlockchainStats bsc { get; set; } = new BlockchainStats("bsc");
    public BlockchainStats eth { get; set; } = new BlockchainStats("eth");
    public BlockchainStats poly { get; set; } = new BlockchainStats("polygon");
    public BlockchainStats ftm { get; set; } = new BlockchainStats("fantom");
    public BlockchainStats avax { get; set; } = new BlockchainStats("avalanche");

    public string Json = "{}";
    public byte[] JsonBytes = Array.Empty<byte>();
    public byte[] JsonBytesBr = Array.Empty<byte>();
    public byte[] JsonBytesGzip = Array.Empty<byte>();

    internal Task DataReceived()
    {
        return _dataSemaphore.WaitAsync();
    }

    private Task BscQueryTask = Task.CompletedTask;
    private Task EthQueryTask = Task.CompletedTask;
    private Task PolyQueryTask = Task.CompletedTask;
    private Task FtmQueryTask = Task.CompletedTask;
    private Task AvaxQueryTask = Task.CompletedTask;
    private Task SerializeQueryTask = Task.CompletedTask;
    private Task CalculateTask = Task.CompletedTask;

    private async Task Serialize()
    {
        while (!_cts.IsCancellationRequested)
        {
            await _serializeSemaphore.WaitAsync();

            Json = JsonSerializer.Serialize(this, _options);
            JsonBytes = Encoding.UTF8.GetBytes(Json);

            {
                var memoryStream = new MemoryStream();
                using (var compression = new BrotliStream(memoryStream, CompressionLevel.Optimal, leaveOpen: true))
                {
                    compression.Write(JsonBytes);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                JsonBytesBr = memoryStream.ToArray();
            }

            {
                var memoryStream = new MemoryStream();
                using (var compression = new GZipStream(memoryStream, CompressionLevel.Optimal, leaveOpen: true))
                {
                    compression.Write(JsonBytes);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                JsonBytesGzip = memoryStream.ToArray();
            }
        }
    }

    private async Task Calculate()
    {
        while (!_cts.IsCancellationRequested)
        {
            await _calcSemaphore.WaitAsync();

            if (RecalculateUnified())
            {
                _serializeSemaphore.Release();
            }
        }
    }

    private bool RecalculateUnified()
    {
        var sample = Recalculate(bsc.current, eth.current, poly.current, ftm.current, avax.current);
        if (sample is null) return false;
        unified.current = sample;

        sample = Recalculate(bsc.history24hrs, eth.history24hrs, poly.history24hrs, ftm.history24hrs, avax.history24hrs);
        if (sample is not null)
        {
            unified.history24hrs = sample;
        }

        sample = Recalculate(bsc.history48hrs, eth.history48hrs, poly.history48hrs, ftm.history48hrs, avax.history48hrs);
        if (sample is not null)
        {
            unified.history48hrs = sample;
        }

        sample = Recalculate(bsc.history7day, eth.history7day, poly.history7day, ftm.history7day, avax.history7day);
        if (sample is not null)
        {
            unified.history7day = sample;
        }

        sample = Recalculate(bsc.history14day, eth.history14day, poly.history14day, ftm.history14day, avax.history14day);
        if (sample is not null)
        {
            unified.history14day = sample;
        }

        bsc.CreateStringRepresentations();
        eth.CreateStringRepresentations();
        poly.CreateStringRepresentations();
        ftm.CreateStringRepresentations();
        avax.CreateStringRepresentations();
        unified.CreateStringRepresentations();

        if (_dataSemaphore.CurrentCount == 0)
        {
            _dataSemaphore.Release();
        }

        return true;
    }

    private BlockchainSample? Recalculate(params BlockchainSample[] samples)
    {
        const decimal totalSupply = 71618033988m;

        var supplyTotal = 0m;
        var supplyWeights = new decimal[samples.Length];
        var liquidityTotal = 0m;
        var liquidityWeights = new decimal[samples.Length];
        var stakingTotal = 0m;
        var stakingWeights = new decimal[samples.Length];

        for (var i = 0; i < samples.Length; i++)
        {
            var chain = samples[i];
            if (chain is null) continue;

            var supply = chain.bridgeVaultValue == 0 ? 0 : totalSupply - chain.bridgeVaultValue;
            supplyTotal += supply;
            supplyWeights[i] = supply;

            var liquidity = chain.usdLiquidityCoinValue;
            liquidityTotal += liquidity;
            liquidityWeights[i] = liquidity;

            var staking = chain.stakedValue;
            stakingTotal += staking;
            stakingWeights[i] = staking;
        }

        for (var i = 0; i < samples.Length; i++)
        {
            supplyWeights[i] /= supplyTotal;
            liquidityWeights[i] /= liquidityTotal;
            stakingWeights[i] /= stakingTotal;
        }

        decimal reservesBalanceValue = -1m;
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

        decimal usdReservesBalanceValue = 0m;
        decimal usdLiquidityTokenValue = 0m;
        decimal usdLiquidityCoinValue = 0m;
        decimal usdStakedValue = 0m;
        decimal usdRewardsValue = 0m;
        decimal usdVolumeTransfersValue = 0m;
        decimal usdVolumeBuyValue = 0m;
        decimal usdVolumeSellValue = 0m;
        decimal usdVolumeTradeValue = 0m;

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

            holdersValue += chain.holdersValue;

            usdReservesBalanceValue += chain.usdReservesBalanceValue;
            usdLiquidityTokenValue += chain.usdLiquidityTokenValue;
            usdLiquidityCoinValue += chain.usdLiquidityCoinValue;
            usdStakedValue += chain.usdStakedValue;
            usdRewardsValue += chain.usdRewardsValue;
            usdVolumeTransfersValue += chain.usdVolumeTransfersValue;
            usdVolumeBuyValue += chain.usdVolumeBuyValue;
            usdVolumeSellValue += chain.usdVolumeSellValue;
            usdVolumeTradeValue += chain.usdVolumeTradeValue;
        }


        for (var i = 0; i < samples.Length; i++)
        {
            var chain = samples[i];
            if (chain is null) continue;

            var supplyOnChainPercentValue = supplyWeights[i];
            chain.supplyOnChainPercentValue = supplyOnChainPercentValue;
            chain.stakedOfTotalSupplyPercentValue = supplyTotal == 0 ? 0 : chain.stakedValue / supplyTotal;
            chain.stakedOfOnChainPercentValue = supplyOnChainPercentValue == 0 ? 0 : chain.stakedValue / (supplyOnChainPercentValue * totalSupply);
            chain.stakedOfTotalStakedPercentValue = supplyTotal == 0 ? 0 : chain.stakedValue / stakingTotal;
        }

        var sample = new BlockchainSample()
        {
            reservesBalanceValue = reservesBalanceValue,
            liquidityTokenValue = liquidityTokenValue,
            liquidityCoinValue = liquidityCoinValue,
            stakedValue = stakedValue,
            aveMultiplierValue = aveMultiplierValue,
            rewardsValue = rewardsValue,
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
            blockNumberValue = -1m,
            usdReservesBalanceValue = usdReservesBalanceValue,
            usdLiquidityTokenValue = usdLiquidityTokenValue,
            usdLiquidityCoinValue = usdLiquidityCoinValue,
            usdStakedValue = usdStakedValue,
            usdRewardsValue = usdRewardsValue,
            usdVolumeTransfersValue = usdVolumeTransfersValue,
            usdVolumeBuyValue = usdVolumeBuyValue,
            usdVolumeSellValue = usdVolumeSellValue,
            usdVolumeTradeValue = usdVolumeTradeValue,

            supplyOnChainPercentValue = 1m,
            stakedOfTotalSupplyPercentValue = stakingTotal / totalSupply,
            stakedOfOnChainPercentValue = stakedValue / totalSupply,
            stakedOfTotalStakedPercentValue = 1m
        };

        sample.CreateStringRepresentations();

        return sample;
    }

    private async Task Query(BlockchainQuery query)
    {
        while (!_cts.IsCancellationRequested)
        {
            await query.GetData();

            if (_calcSemaphore.CurrentCount == 0)
            {
                _calcSemaphore.Release();
            }

            await Task.Delay(TimeSpan.FromSeconds(15), _cts.Token);
        }
    }

    public Stats(ApiConfig config)
    {
        var speedyKey = config.MoralisConfiguration.SpeedyNodeKey;

        _bscQuery = new BlockchainQuery(
            config,
            bsc,
            currentEndpoints: new[] {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/bsc/mainnet"
            },
            archiveEndpoints: new[]
            {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/bsc/mainnet/archive",
            },
            statsContractAddress: "0xC4DD716a29357317BdC66d9D9cF2ec02fD995742");

        _ethQuery = new BlockchainQuery(
            config,
            eth,
            currentEndpoints: new[] {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/eth/mainnet"
            },
            archiveEndpoints: new[]
            {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/eth/mainnet/archive"
            },
            statsContractAddress: "0xC4DD716a29357317BdC66d9D9cF2ec02fD995742");

        _polyQuery = new BlockchainQuery(
            config,
            poly,
            currentEndpoints: new[] {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/polygon/mainnet",
            },
            archiveEndpoints: new[]
            {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/polygon/mainnet/archive"
            },
            statsContractAddress: "0xC4DD716a29357317BdC66d9D9cF2ec02fD995742");

        _avaxQuery = new BlockchainQuery(
            config,
            avax,
            currentEndpoints: new[] {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/avalanche/mainnet",
            },
            archiveEndpoints: new string[]
            {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/avalanche/mainnet"
            },
            statsContractAddress: "0xC4DD716a29357317BdC66d9D9cF2ec02fD995742");

        _ftmQuery = new BlockchainQuery(
            config,
            ftm,
            currentEndpoints: new[] {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/fantom/mainnet",
            },
            archiveEndpoints: new string[]
            {
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/fantom/mainnet"
            },
            statsContractAddress: "0xC4DD716a29357317BdC66d9D9cF2ec02fD995742");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        BscQueryTask = Query(_bscQuery);
        EthQueryTask = Query(_ethQuery);
        PolyQueryTask = Query(_polyQuery);
        FtmQueryTask = Query(_ftmQuery);
        AvaxQueryTask = Query(_avaxQuery);
        SerializeQueryTask = Serialize();
        CalculateTask = Calculate();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }
}