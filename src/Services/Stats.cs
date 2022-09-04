// Copyright (c) EverRise Pte Ltd. All rights reserved.

using Contracts.Contracts.EverRise;

using EverStats.Config;
using EverStats.Data;

using Microsoft.Data.SqlClient;

using Nethereum.Web3;

using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;

using static EverStats.Services.TwitterBot;

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

    internal Task DataReceived()
    {
        return _dataSemaphore.WaitAsync();
    }

    private Task HistoryTask = Task.CompletedTask;
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
                    switch (staked.id)
                    {
                        case "1":
                            quatities = eth.current;
                            break;
                        case "56":
                            quatities = bsc.current;
                            break;
                        case "137":
                            quatities = poly.current;
                            break;
                        case "250":
                            quatities = ftm.current;
                            break;
                        case "43114":
                            quatities = avax.current;
                            break;
                    }

                    quatities.stakedValue = (decimal)staked.amount;
                    quatities.aveMultiplierValue = quatities.veAmountValue / quatities.stakedValue;
                    quatities.usdStakedValue = quatities.stakedValue * quatities.tokenPriceStableValue;
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

        if (bsc.history24hrs != null && eth.history24hrs != null && poly.history24hrs != null && ftm.history24hrs != null && avax.history24hrs != null)
        {
            sample = Recalculate(bsc.history24hrs, eth.history24hrs, poly.history24hrs, ftm.history24hrs, avax.history24hrs);
            if (sample is not null)
            {
                unified.history24hrs = sample;
            }
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



    public class ChainSnapshot
    {
        public ChainData current { get; set; }
        public ChainData history24hrs { get; set; }
        public ChainData history48hrs { get; set; }
        public ChainData history7day { get; set; }
        public ChainData history14day { get; set; }
        public List<OhlcVol> ohlc { get; set; } = new();
    }

    public class Chains
    {
        public ChainSnapshot unified { get; set; } = new();
        public ChainSnapshot eth { get; set; } = new();
        public ChainSnapshot bsc { get; set; } = new();
        public ChainSnapshot poly { get; set; } = new();
        public ChainSnapshot ftm { get; set; } = new();
        public ChainSnapshot avax { get; set; } = new();
    }

    public async Task<Chains> GetChainData()
    {
        var chains = new Chains();

        using var conn = new SqlConnection(_config.AzureConfiguration.SqlConnection);
        await conn.OpenAsync();

        var data = await GetRecentDataSnapshots(conn);
        foreach (var row in data)
        {
            ChainSnapshot? chain = null;
            switch (row.chainId)
            {
                case 0:
                    chain = chains.unified;
                    break;
                case 1:
                    chain = chains.eth;
                    break;
                case 56:
                    chain = chains.bsc;
                    break;
                case 137:
                    chain = chains.poly;
                    break;
                case 250:
                    chain = chains.ftm;
                    break;
                case 43114:
                    chain = chains.avax;
                    break;
            }

            if (chain is null) continue;

            switch (row.entry)
            {
                case 1:
                    chain.current = row;
                    break;
                case 2:
                    chain.history24hrs = row;
                    break;
                case 3:
                    chain.history48hrs = row;
                    break;
                case 4:
                    chain.history7day = row;
                    break;
                case 5:
                    chain.history14day = row;
                    break;
            }
        }

        await AddOlhcData(chains, conn);

        return chains;
    }

    private async Task AddOlhcData(Chains chains, SqlConnection conn)
    {
        using var cmd = new SqlCommand(@"Get14DayCandleData", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var chainId = reader.GetInt32("chainId");
            ChainSnapshot? chain = null;
            switch (chainId)
            {
                case 0:
                    chain = chains.unified;
                    break;
                case 1:
                    chain = chains.eth;
                    break;
                case 56:
                    chain = chains.bsc;
                    break;
                case 137:
                    chain = chains.poly;
                    break;
                case 250:
                    chain = chains.ftm;
                    break;
                case 43114:
                    chain = chains.avax;
                    break;
                default:
                    continue;
            }

            var c = new OhlcVol();
            c.dateValue = DateOnly.FromDateTime(reader.GetDateTime("date"));

            c.openValue = reader.GetDecimal("Open");
            c.lowValue = reader.GetDecimal("Low");
            c.highValue = reader.GetDecimal("High");
            c.closeValue = reader.GetDecimal("Close");
            c.averageValue = reader.GetDecimal("Avg");
            c.volumeValue = reader.GetDecimal("Vol");

            c.CreateStringRepresentations();

            chain.ohlc.Add(c);
        }
    }

    public async Task<List<ChainData>> GetRecentDataSnapshots(SqlConnection conn)
    {
        using var cmd = new SqlCommand(@"GetRecentChainData", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        using var reader = await cmd.ExecuteReaderAsync();

        var data = new List<ChainData>();
        while (await reader.ReadAsync())
        {
            var c = new ChainData();
            c.entry = reader.GetInt32("entry");
            c.dateOnly = DateOnly.FromDateTime(reader.GetDateTime("dateOnly"));
            c.date = reader.GetDateTime("date");
            c.chainId = reader.GetInt32("chainId");

            c.reservesCoinBalance = reader.GetDecimal("reservesCoinBalance");
            c.reservesTokenBalance = reader.GetDecimal("reservesTokenBalance");
            c.liquidityToken = reader.GetDecimal("liquidityToken");
            c.liquidityCoin = reader.GetDecimal("liquidityCoin");
            c.veAmount = reader.GetDecimal("veAmount");
            c.staked = reader.GetDecimal("staked");
            c.aveMultiplier = reader.GetDecimal("aveMultiplier");
            c.rewards = reader.GetDecimal("rewards");
            c.volumeBuy = reader.GetDecimal("volumeBuy");
            c.volumeSell = reader.GetDecimal("volumeSell");
            c.volumeTrade = reader.GetDecimal("volumeTrade");
            c.bridgeVault = reader.GetDecimal("bridgeVault");
            c.tokenPriceCoin = reader.GetDecimal("tokenPriceCoin");
            c.coinPriceStable = reader.GetDecimal("coinPriceStable");
            c.tokenPriceStable = reader.GetDecimal("tokenPriceStable");
            c.marketCap = reader.GetDecimal("marketCap");
            c.blockNumber = reader.GetDecimal("blockNumber");
            c.holders = reader.GetDecimal("holders");
            c.burn = reader.GetDecimal("burn");
            c.burnPercent = reader.GetDecimal("burnPercent");
            c.totalSupply = reader.GetDecimal("totalSupply");
            c.everSwap = reader.GetDecimal("everSwap");
            c.usdReservesCoinBalance = reader.GetDecimal("usdReservesCoinBalance");
            c.usdReservesTokenBalance = reader.GetDecimal("usdReservesTokenBalance");
            c.usdReservesBalance = reader.GetDecimal("usdReservesBalance");
            c.usdLiquidityToken = reader.GetDecimal("usdLiquidityToken");
            c.usdLiquidityCoin = reader.GetDecimal("usdLiquidityCoin");
            c.usdStaked = reader.GetDecimal("usdStaked");
            c.usdRewards = reader.GetDecimal("usdRewards");
            c.usdVolumeBuy = reader.GetDecimal("usdVolumeBuy");
            c.usdVolumeSell = reader.GetDecimal("usdVolumeSell");
            c.usdVolumeTrade = reader.GetDecimal("usdVolumeTrade");
            c.usdEverSwap = reader.GetDecimal("usdEverSwap");
            c.supplyOnChainPercent = reader.GetDecimal("supplyOnChainPercent");
            c.stakedOfTotalSupplyPercent = reader.GetDecimal("stakedOfTotalSupplyPercent");
            c.stakedOfOnChainPercent = reader.GetDecimal("stakedOfOnChainPercent");
            c.stakedOfTotalStakedPercent = reader.GetDecimal("stakedOfTotalStakedPercent");
            c.veRiseOnChainPercent = reader.GetDecimal("veRiseOnChainPercent");

            if (!reader.IsDBNull("unclaimedTokenBalance"))
            {
                c.unclaimedTokenBalance = reader.GetDecimal("unclaimedTokenBalance");
                c.usdUnclaimedTokenBalance = reader.GetDecimal("usdUnclaimedTokenBalance");
            }
            if (!reader.IsDBNull("stakesCount"))
            {
                c.veRiseOnChainPercent = reader.GetDecimal("stakesCount");
                c.veRiseOnChainPercent = reader.GetDecimal("mementosCount");
            }

            data.Add(c);
        }

        return data;
    }

    public async Task GetDataHistoricData()
    {
        while (!_cts.IsCancellationRequested)
        {
            var waitTime = TimeSpan.FromMinutes(15);

            try
            {
                var chainData = await GetChainData();

                unified.history24hrs = new BlockchainSample(chainData.unified.history24hrs);
                unified.history48hrs = new BlockchainSample(chainData.unified.history48hrs);
                unified.history7day = new BlockchainSample(chainData.unified.history7day);
                unified.history14day = new BlockchainSample(chainData.unified.history14day);

                bsc.history24hrs = new BlockchainSample(chainData.bsc.history24hrs);
                bsc.history48hrs = new BlockchainSample(chainData.bsc.history48hrs);
                bsc.history7day = new BlockchainSample(chainData.bsc.history7day);
                bsc.history14day = new BlockchainSample(chainData.bsc.history14day);

                eth.history24hrs = new BlockchainSample(chainData.eth.history24hrs);
                eth.history48hrs = new BlockchainSample(chainData.eth.history48hrs);
                eth.history7day = new BlockchainSample(chainData.eth.history7day);
                eth.history14day = new BlockchainSample(chainData.eth.history14day);

                poly.history24hrs = new BlockchainSample(chainData.poly.history24hrs);
                poly.history48hrs = new BlockchainSample(chainData.poly.history48hrs);
                poly.history7day = new BlockchainSample(chainData.poly.history7day);
                poly.history14day = new BlockchainSample(chainData.poly.history14day);

                ftm.history24hrs = new BlockchainSample(chainData.ftm.history24hrs);
                ftm.history48hrs = new BlockchainSample(chainData.ftm.history48hrs);
                ftm.history7day = new BlockchainSample(chainData.ftm.history7day);
                ftm.history14day = new BlockchainSample(chainData.ftm.history14day);

                avax.history24hrs = new BlockchainSample(chainData.avax.history24hrs);
                avax.history48hrs = new BlockchainSample(chainData.avax.history48hrs);
                avax.history7day = new BlockchainSample(chainData.avax.history7day);
                avax.history14day = new BlockchainSample(chainData.avax.history14day);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());

                waitTime = TimeSpan.FromSeconds(30);
            }

            await Task.Delay(waitTime);
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
            unclaimedTokenBalanceValue = unclaimedTokenBalanceValue,
            usdUnclaimedTokenBalanceValue = usdUnclaimedTokenBalanceValue,
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

    private async Task<(int ChainId, decimal Locked)> GetLockedInner(ChainInfo chain, string[] addresses)
    {
        Exception ex = null;
        var endpoints = chain.Query.CurrentEndpoints;
        foreach (var endpoint in endpoints)
        {
            try
            {
                var data = (chain.ChainId, await GetLocked(new Web3(endpoint), addresses));
                if (data.Item2 == 0)
                {
                    throw new InvalidDataException($"Zero locked tokens for {chain.Chain}");
                }

                _logger.LogInformation($"Locked Tokens for {chain.Chain} is {data.Item2}");

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

    private async Task<(int ChainId, decimal Locked)> GetLocked(ChainInfo chain, string[] addresses)
    {
        Exception ex = null;
        var retries = 0;
        while (retries < 4) {
            try
            {
                return await GetLockedInner(chain, addresses);
            }
            catch (Exception)
            {
            }

            retries--;

            await Task.Delay(10000 * retries);
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
                "https://bsc-dataseed4.binance.org/"
            },
            archiveEndpoints: new[]
            {
                "https://bsc-mainnet.nodereal.io/v1/77a6ad1e2ca847ebae6f3632119c2bba",
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
                "https://rpc.ankr.com/eth",
            },
            statsContractAddress: "0x889f26f688f0b757F84e5C07bf9FeC6D6c368Af2",
            everSwapAddress: "0x8dc9b4e0a5688fa4869d438d8720c9621fa777dc",
            logger: logger);

        _polyQuery = new BlockchainQuery("polygon",
            config,
            poly,
            currentEndpoints: new[] {
                "https://rpc-mainnet.maticvigil.com/v1/9f9c72670ae63ead023c7cb64594c31c021e7e14",
                "https://polygon-rpc.com/",
                "https://rpc.ankr.com/polygon",
            },
            archiveEndpoints: new[]
            {
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
            },
            archiveEndpoints: new string[]
            {
            },
            statsContractAddress: "0x889f26f688f0b757F84e5C07bf9FeC6D6c368Af2",
            everSwapAddress: "0x6d9fA4Fb73942A416d89ad3f7553eFefF9b3F74B",
            logger: logger);

        _ftmQuery = new BlockchainQuery("fantom",
            config,
            ftm,
            currentEndpoints: new[] {
                "https://rpcapi.fantom.network/",
                "https://rpc.ankr.com/fantom",
                "https://rpcapi.fantom.network",
            },
            archiveEndpoints: new string[]
            {
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

        HistoryTask = GetDataHistoricData();
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