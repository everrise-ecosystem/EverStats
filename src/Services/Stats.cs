// Copyright (c) EverRise Pte Ltd. All rights reserved.

using Contracts.Contracts.EverRise;

using EverStats.Config;
using EverStats.Data;

using Microsoft.Data.SqlClient;

using Nethereum.Web3;

using System.Collections;
using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;

namespace EverStats.Services;

public class EndPoints : IEnumerable<string>
{
    private object _lock = new object();
    private string[] _endpoints;
    private string[] a;

    public EndPoints(string[] a)
    {
        _endpoints = a;
    }

    public int Length => _endpoints.Length;

    public static implicit operator EndPoints(string[] a) => new EndPoints(a);

    public void FailedEndpoint(string endpoint)
    {
        lock (_lock)
        {
            string[] endpoints = _endpoints;

            endpoints = endpoints.Where(e => e != endpoint)
                .Append(endpoint).ToArray();

            _endpoints = endpoints;
        }
    }

    public struct EndpointEnumerator : IEnumerator<string>
    {
        private int _index;
        private string[] _array;

        public string Current => _array[_index];
        object IEnumerator.Current => Current;

        public EndpointEnumerator (string[] array)
        {
            _index = -1;
            _array = array;
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _array.Length;
        }

        public void Dispose()
        {
        }

        public void Reset() => throw new NotSupportedException();
    }

    public EndpointEnumerator GetEnumerator()
    {
        return new EndpointEnumerator(_endpoints);
    }

    IEnumerator<string> IEnumerable<string>.GetEnumerator()
    {
        return (IEnumerator<string>)_endpoints.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _endpoints.GetEnumerator();
    }
}

public class Stats : IHostedService
{
    private readonly static MediaTypeWithQualityHeaderValue s_jsonAccept = MediaTypeWithQualityHeaderValue.Parse("application/json");
    private readonly static JsonSerializerOptions _options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    private readonly ILogger<BlockchainQuery> _logger;
    private readonly ApiConfig _config;
    private readonly HolderList _holders;
    private CancellationTokenSource _cts = new CancellationTokenSource();
    private SemaphoreSlim _calcSemaphore = new SemaphoreSlim(0);
    private SemaphoreSlim _serializeSemaphore = new SemaphoreSlim(0);
    private SemaphoreSlim _dataSemaphore = new SemaphoreSlim(0);

    private HttpClient _stakingClient = GetHttpClient();

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

    public async Task<decimal> QueryCoinPrice(string chain, int blockNumber)
    {
        var stats = await (chain switch
        {
            "eth" => _ethQuery.QueryHistoricStats(blockNumber),
            "bsc" => _ethQuery.QueryHistoricStats(blockNumber),
            "poly" => _ethQuery.QueryHistoricStats(blockNumber),
            "ftm" => _ethQuery.QueryHistoricStats(blockNumber),
            "avax" => _ethQuery.QueryHistoricStats(blockNumber),
            _ => null
        });

        return stats.coinPriceStableValue;
    }

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


    private static HttpClient GetHttpClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(s_jsonAccept);
        return client;
    }

    private async Task Calculate()
    {
        while (!_cts.IsCancellationRequested)
        {
            await _calcSemaphore.WaitAsync();

            try
            {
                var stakedAmounts = await GetStakeAmounts();
                for (int i = 0; i < stakedAmounts.Length; i++)
                {
                    var staked = stakedAmounts[i];
                    BlockchainSample quatities = null;
                    BlockchainSample history24hrs = null;
                    BlockchainSample history48hrs = null;
                    BlockchainSample history7day = null;
                    BlockchainSample history14day = null;
                    switch (staked.id)
                    {
                        case "1":
                            quatities = eth.current;
                            history24hrs = eth.history24hrs;
                            history48hrs = eth.history48hrs;
                            history7day = eth.history7day;
                            history14day = eth.history14day;
                            break;
                        case "56":
                            quatities = bsc.current;
                            history24hrs = bsc.history24hrs;
                            history48hrs = bsc.history48hrs;
                            history7day = bsc.history7day;
                            history14day = bsc.history14day;
                            break;
                        case "137":
                            quatities = poly.current;
                            history24hrs = poly.history24hrs;
                            history48hrs = poly.history48hrs;
                            history7day = poly.history7day;
                            history14day = poly.history14day;
                            break;
                        case "250":
                            quatities = ftm.current;
                            history24hrs = ftm.history24hrs;
                            history48hrs = ftm.history48hrs;
                            history7day = ftm.history7day;
                            history14day = ftm.history14day;
                            break;
                        case "43114":
                            quatities = avax.current;
                            history24hrs = avax.history24hrs;
                            history48hrs = avax.history48hrs;
                            history7day = avax.history7day;
                            history14day = avax.history14day;
                            break;
                    }

                    quatities.stakedValue = (decimal)staked.amount;
                    quatities.aveMultiplierValue = quatities.veAmountValue / quatities.stakedValue;
                    quatities.usdStakedValue = quatities.stakedValue * quatities.tokenPriceStableValue;

                    if (history24hrs != null)
                    {
                        history24hrs.stakedValue = quatities.stakedValue;
                    }
                    if (history48hrs != null)
                    {
                        history48hrs.stakedValue = quatities.stakedValue;
                    }
                    if (history7day != null)
                    {
                        history7day.stakedValue = quatities.stakedValue;
                    }
                    if (history14day != null)
                    {
                        history14day.stakedValue = quatities.stakedValue;
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _stakingClient.Dispose();
                _stakingClient = GetHttpClient();
            }

            if (await RecalculateUnified())
            {
                _serializeSemaphore.Release();
            }
        }
    }

    public class StakeAmounts
    {
        public string id { get; set; }
        public double amount { get; set; }
    }

    private async Task<bool> RecalculateUnified()
    {
        var sample = Recalculate(bsc.current, eth.current, poly.current, ftm.current, avax.current);
        if (sample is null) return false;
        unified.current = sample;

        if (_config.StoreInDb) {
            await StoreInDb();
        }

        sample = Recalculate(bsc.history24hrs, eth.history24hrs, poly.history24hrs, ftm.history24hrs, avax.history24hrs);
        if (sample is not null)
        {
            unified.history24hrs = sample;
        }

        if (bsc.history48hrs != null && eth.history48hrs != null && poly.history48hrs != null && ftm.history48hrs != null && avax.history48hrs != null)
        {
            sample = Recalculate(bsc.history48hrs, eth.history48hrs, poly.history48hrs, ftm.history48hrs, avax.history48hrs);
            if (sample is not null)
            {
                unified.history48hrs = sample;
            }
        }

        if (bsc.history7day != null && eth.history7day != null && poly.history7day != null && ftm.history7day != null && avax.history7day != null)
        {
            sample = Recalculate(bsc.history7day, eth.history7day, poly.history7day, ftm.history7day, avax.history7day);
            if (sample is not null)
            {
                unified.history7day = sample;
            }
        }

        if (bsc.history14day != null && eth.history14day != null && poly.history14day != null && ftm.history14day != null && avax.history14day != null)
        {
            sample = Recalculate(bsc.history14day, eth.history14day, poly.history14day, ftm.history14day, avax.history14day);
            if (sample is not null)
            {
                unified.history14day = sample;
            }
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

    private async Task StoreInDb()
    {
        using var conn = new SqlConnection(_config.AzureConfiguration.SqlConnection);
        await conn.OpenAsync();

        await StoreInDb(chainId: 0, unified.current, conn);
        foreach (var chain in _chains)
        {
            await StoreInDb(chain.ChainId, chain.Stats.current, conn);
        }
    }

    private async Task StoreInDb(int chainId, BlockchainSample sample, SqlConnection conn)
    {
        try
        {
            if (sample.date != sample.lastStored) {
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
            lastStored = unified?.current?.lastStored,
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
            stakesCountValue = stakesCountValue,
            mementosCountValue = mementosCountValue
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

    struct ChainInfo
    {
        public BlockchainStats Stats;
        public BlockchainQuery Query;
        public string Chain;
        public int ChainId;
    }

    static ChainInfo[] _chains;


    private static async Task<decimal> GetLocked(Web3 web3, string[] addresses)
    {
        var everRise = new EverRiseService(web3, "0xC17c30e98541188614dF99239cABD40280810cA3");

        var lockedAmount = await everRise.GetAmountLockedUsingMultiCallAsync(addresses, numberOfCallsPerRequest: 2000);

        var totalStaked = lockedAmount.Select(x => Nethereum.Util.UnitConversion.Convert.FromWei(x)).Sum();

        return totalStaked;
    }

    private async Task<(int ChainId, decimal Locked)> GetLocked(ChainInfo chain, string[] addresses)
    {
        Exception ex = null;
        foreach (var endpoint in chain.Query.CurrentEndpoints)
        {
            try
            {
                var data = (chain.ChainId, await GetLocked(new Web3(endpoint), addresses));
                _logger.LogInformation($"Locked Tokens for {chain.Chain} is {data.Item2}");

                return data;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                ex = e;
            }
        }

        ExceptionDispatchInfo.Throw(ex);
        throw ex;
    }

    private StakeAmounts[] _stakedAmounts;

    Stopwatch stopwatch = Stopwatch.StartNew();
    Task _stakeAmountsTask = Task.CompletedTask;

    private async Task<StakeAmounts[]> GetStakeAmounts()
    {
        if (_stakedAmounts is null || stopwatch.ElapsedMilliseconds > 10 * 60 * 1_000)
        {
            stopwatch.Restart();

            var prevTask = _stakeAmountsTask;

            if (!prevTask.IsCompleted)
            {
                await prevTask;
            }
            else
            {
                _stakeAmountsTask = RegenerateStakeAmounts();
                if (_stakedAmounts is null)
                {
                    await _stakeAmountsTask;
                }
            }
        }

        return _stakedAmounts;
    }

    private async Task RegenerateStakeAmounts()
    {
        var addresses = await _holders.GetHolderList();

        var tasks = new List<Task<(int ChainId, decimal Locked)>>();
        foreach (var chain in _chains)
        {
            tasks.Add(GetLocked(chain, addresses));
        }

        await Task.WhenAll(tasks);

        var amounts = new List<StakeAmounts>();
        foreach (var item in tasks)
        {
            (int ChainId, decimal Locked) = await item;
            amounts.Add(new StakeAmounts { id = ChainId.ToString("0"), amount = (double)Locked });
        }

        _stakedAmounts = amounts.ToArray();
    }

    public Stats(ApiConfig config, HolderList holders, ILogger<BlockchainQuery> logger)
    {
        _logger = logger;
        _config = config;
        _holders = holders;

        var speedyKey = config.MoralisConfiguration.SpeedyNodeKey;

        _bscQuery = new BlockchainQuery("bsc",
            config,
            bsc,
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
                $"https://speedy-nodes-nyc.moralis.io/{speedyKey}/bsc/mainnet/archive",
                "https://rpc.ankr.com/bsc",
            },
            statsContractAddress: "0x889f26f688f0b757F84e5C07bf9FeC6D6c368Af2",
            everSwapAddress: "0x71B089280237672837a23d3c2cC6cFC43424e08E",
            logger: logger);

        _ethQuery = new BlockchainQuery("eth",
            config,
            eth,
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
            poly,
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
            avax,
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
            ftm,
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

        _chains = new ChainInfo[]
        { 
            new ()
            {
                Query = _bscQuery,
                Chain = "bsc", 
                ChainId = 56, 
                Stats = bsc
            },
            new ()
            {
                Query = _ethQuery,
                Chain = "eth",
                ChainId = 1,
                Stats = eth
            },
            new ()
            {
                Query = _polyQuery,
                Chain = "poly",
                ChainId = 137,
                Stats = poly
            },
            new ()
            {
                Query = _ftmQuery,
                Chain = "ftm",
                ChainId = 250,
                Stats = ftm
            },
            new ()
            {
                Query = _avaxQuery,
                Chain = "avx",
                ChainId = 43114,
                Stats = avax
            }
        };
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _holders.GetHolderList();

        BscQueryTask = Query(_bscQuery);
        EthQueryTask = Query(_ethQuery);
        PolyQueryTask = Query(_polyQuery);
        FtmQueryTask = Query(_ftmQuery);
        AvaxQueryTask = Query(_avaxQuery);
        SerializeQueryTask = Serialize();
        CalculateTask = Calculate();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }
}