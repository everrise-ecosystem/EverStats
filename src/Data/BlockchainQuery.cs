// Copyright (c) EverRise Pte Ltd. All rights reserved.

using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using EverStats.Config;

namespace EverStats.Data;

public class BlockchainQuery
{
    private readonly static JsonSerializerOptions s_options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    private readonly static MediaTypeWithQualityHeaderValue s_jsonAccept = MediaTypeWithQualityHeaderValue.Parse("application/json");

    private readonly string[] _currentEndpoints;
    private readonly string[] _historyEndpoints;
    private readonly string _statsContractAddress;

    private HttpClient _nodeClient;
    private HttpClient _web3Client;

    private decimal _lastBlock = 0;
    private BlockchainStats _stats;
    private ApiConfig _config;

    private DateTimeOffset _lastHistoryCheck24hrs;
    private DateTimeOffset _lastHistoryCheck48hrs;

    private Task _history24hrTask = Task.CompletedTask;
    private Task _history48hrTask = Task.CompletedTask;

    public BlockchainQuery(ApiConfig config, BlockchainStats stats, string[] currentEndpoints, string[] archiveEndpoints, string statsContractAddress)
    {
        _config = config;
        _currentEndpoints = currentEndpoints;
        _historyEndpoints = archiveEndpoints;
        _statsContractAddress = statsContractAddress;
        _stats = stats;
        _nodeClient = GetHttpClient();
        _web3Client = GetHttpClient();
        _web3Client.DefaultRequestHeaders.Add("X-API-Key", config.MoralisConfiguration.Web3ApiKey);
    }

    private static HttpClient GetHttpClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(s_jsonAccept);
        return client;
    }

    public async Task GetData()
    {
        _stats.current = await QueryStats(_currentEndpoints, "latest", checkBlock: true);
        if (_stats.history24hrs is null)
        {
            _stats.history24hrs = _stats.current;
        }
        if (_stats.history48hrs is null)
        {
            _stats.history48hrs = _stats.current;
        }

        var now = DateTimeOffset.UtcNow;
        if (_history24hrTask.IsCompleted)
        {
            _history24hrTask = GetDataHistoricData(now, is48: false);
        }

        if (_history48hrTask.IsCompleted)
        {
            _history48hrTask = GetDataHistoricData(now, is48: true);
        }
    }

    public async Task GetDataHistoricData(DateTimeOffset date, bool is48)
    {
        try
        {
            if ((date - (is48 ? _lastHistoryCheck48hrs : _lastHistoryCheck24hrs)).TotalHours > 0.5)
            {
                var queryDate = DateTimeOffset.UtcNow.AddDays(is48 ? -2 : -1);
                var blockNumber = await QueryBlock(queryDate);
                if (blockNumber == 0) throw new InvalidDataException("Zero block number");

                var historyData = await QueryStats(_historyEndpoints, "0x" + NumberToHex(blockNumber));
                historyData.date = queryDate.ToString("s");

                if (!is48)
                {
                    _stats.history24hrs = historyData;
                    _lastHistoryCheck24hrs = date;
                }
                else
                {
                    _stats.history48hrs = historyData;
                    _lastHistoryCheck48hrs = date;
                }

            }
        }
        catch (Exception ex)
        {
            var checkDate = date.Subtract(TimeSpan.FromHours(0.25));
            if (!is48)
            {
                _lastHistoryCheck24hrs = checkDate;
            }
            else
            {
                _lastHistoryCheck48hrs = checkDate;
            }

            Console.WriteLine(_stats.Chain + " : " + ex.Message);
            _web3Client?.Dispose();
            _web3Client = GetHttpClient();
            _web3Client.DefaultRequestHeaders.Add("X-API-Key", _config.MoralisConfiguration.Web3ApiKey);
        }

        await Task.Delay(600_0000);
    }

    private async Task<int> QueryBlock(DateTimeOffset date)
    {
        var timeStamp = date.ToUnixTimeSeconds();
        var response = await _web3Client.GetAsync($"https://deep-index.moralis.io/api/v2/dateToBlock?chain={_stats.Chain}&date={timeStamp:0}");
        var data = await response.Content.ReadAsStringAsync();
        var nearest = JsonSerializer.Deserialize<NearestBlock>(data);
        return nearest.block;
    }

    private static string NumberToHex(int estimatedBlock)
    {
        var span = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref estimatedBlock, 1));
        span.Reverse();

        var blockNumber = Convert.ToHexString(span);
        int i = 0;
        for (; i < blockNumber.Length; i++)
        {
            if (blockNumber[i] != '0') break;
        }


        return blockNumber.Substring(i);
    }

    private async Task<BlockchainSample?> QueryStats(string[] apiEndpoints, string blockNumber, bool checkBlock = false)
    {
        if (apiEndpoints.Length == 0) return null;

        var rpc = new RpcCall(_statsContractAddress, blockNumber);

        var json = JsonSerializer.Serialize(rpc, s_options);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        RpcResult? response = null;
        while (response is null)
        {
            for (int i = 0; i < apiEndpoints.Length; i++)
            {
                var apiEndpoint = apiEndpoints[i];

                try
                {
                    response = await QueryEndpoint(apiEndpoint, content);
                    var quatities = GetData(response);
                    if (checkBlock)
                    {
                        if (_lastBlock > quatities.blockNumberValue)
                        {
                            response = null;
                            continue;
                        }
                        _lastBlock = quatities.blockNumberValue;
                    }
                    return quatities;
                }
                catch
                {
                    continue;
                }
            }

            _nodeClient?.Dispose();
            _nodeClient = GetHttpClient();
            await Task.Delay(200);
        }

        return null;
    }

    private BlockchainSample GetData(RpcResult response)
    {
        var quatities = new BlockchainSample();

        var length = 64;
        var hex = response.result.AsSpan(2);

        quatities.reservesBalanceValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.liquidityTokenValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.liquidityCoinValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.stakedValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.aveMultiplierValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.rewardsValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.volumeTransfersValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.volumeBuyValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.volumeSellValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.volumeTradeValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.bridgeVaultValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.tokenPriceCoinValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.coinPriceStableValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.tokenPriceStableValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.marketCapValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.blockNumberValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.holdersValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);

        var ten = 10;
        var tokenDivisor = (decimal)Math.Pow(ten, (double)HexToDecimal(hex.Slice(0, length)));
        hex = hex.Slice(length);
        var coinDivisor = (decimal)Math.Pow(ten, (double)HexToDecimal(hex.Slice(0, length)));
        hex = hex.Slice(length);
        var stableDivisor = (decimal)Math.Pow(ten, (double)HexToDecimal(hex.Slice(0, length)));
        hex = hex.Slice(length);
        var multiplierDivisor = (decimal)Math.Pow(ten, (double)HexToDecimal(hex.Slice(0, length)));
        hex = hex.Slice(length);

        quatities.reservesBalanceValue = quatities.reservesBalanceValue / coinDivisor;
        quatities.liquidityTokenValue = quatities.liquidityTokenValue / tokenDivisor;
        quatities.liquidityCoinValue = quatities.liquidityCoinValue / coinDivisor;
        quatities.stakedValue = quatities.stakedValue / tokenDivisor;
        quatities.rewardsValue = quatities.rewardsValue / tokenDivisor;

        quatities.stakedValue -= quatities.rewardsValue;

        quatities.aveMultiplierValue = quatities.aveMultiplierValue / multiplierDivisor;
        quatities.volumeTransfersValue = quatities.volumeTransfersValue / tokenDivisor;
        quatities.volumeBuyValue = quatities.volumeBuyValue / tokenDivisor;
        quatities.volumeSellValue = quatities.volumeSellValue / tokenDivisor;
        quatities.volumeTradeValue = quatities.volumeTradeValue / tokenDivisor;
        quatities.bridgeVaultValue = quatities.bridgeVaultValue / tokenDivisor;
        quatities.tokenPriceCoinValue = quatities.tokenPriceCoinValue / coinDivisor;
        quatities.coinPriceStableValue = quatities.coinPriceStableValue / stableDivisor;
        quatities.tokenPriceStableValue = quatities.tokenPriceStableValue / stableDivisor;
        quatities.marketCapValue = quatities.marketCapValue / stableDivisor;

        quatities.usdReservesBalanceValue = quatities.reservesBalanceValue * quatities.coinPriceStableValue;
        quatities.usdLiquidityTokenValue = quatities.liquidityTokenValue * quatities.tokenPriceStableValue;
        quatities.usdLiquidityCoinValue = quatities.liquidityCoinValue * quatities.coinPriceStableValue;
        quatities.usdStakedValue = quatities.stakedValue * quatities.tokenPriceStableValue;
        quatities.usdRewardsValue = quatities.rewardsValue * quatities.tokenPriceStableValue;
        quatities.usdVolumeTransfersValue = quatities.volumeTransfersValue * quatities.tokenPriceStableValue;
        quatities.usdVolumeBuyValue = quatities.volumeBuyValue * quatities.tokenPriceStableValue;
        quatities.usdVolumeSellValue = quatities.volumeSellValue * quatities.tokenPriceStableValue;
        quatities.usdVolumeTradeValue = quatities.volumeTradeValue * quatities.tokenPriceStableValue;

        return quatities;
    }

    private async Task<RpcResult?> QueryEndpoint(string endpoint, HttpContent content)
    {
        using var response = await _nodeClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<RpcResult>(json);
    }

    private async Task<Block?> QueryBlockEndpoint(string endpoint, HttpContent content)
    {
        using var response = await _nodeClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Block>(json);
    }

    public decimal HexToDecimal(ReadOnlySpan<char> hex, bool isHexLittleEndian = false)
    {
        if (hex == "0x0") return 0;

        var encoded = HexToByteArray(hex);

        if (BitConverter.IsLittleEndian != isHexLittleEndian)
        {
            var listEncoded = encoded.ToList();
            listEncoded.Insert(0, 0x00);
            encoded = listEncoded.ToArray().Reverse().ToArray();
        }
        return (decimal)(new BigInteger(encoded));
    }

    private static byte[] HexToByteArray(ReadOnlySpan<char> value)
    {
        try
        {
            return HexToByteArrayInternal(value);
        }
        catch (FormatException ex)
        {
            throw new FormatException(string.Format(
                "String '{0}' could not be converted to byte array (not hex?).", value.ToString()), ex);
        }
    }

    private static byte[] HexToByteArrayInternal(ReadOnlySpan<char> value)
    {
        byte[] bytes = Array.Empty<byte>();
        if (!value.IsEmpty)
        {
            var string_length = value.Length;
            var character_index = value.StartsWith("0x", StringComparison.Ordinal) ? 2 : 0;
            // Does the string define leading HEX indicator '0x'. Adjust starting index accordingly.               
            var number_of_characters = string_length - character_index;

            var add_leading_zero = false;
            if (0 != number_of_characters % 2)
            {
                add_leading_zero = true;

                number_of_characters += 1; // Leading '0' has been striped from the string presentation.
            }

            bytes = new byte[number_of_characters / 2]; // Initialize our byte array to hold the converted string.

            var write_index = 0;
            if (add_leading_zero)
            {
                bytes[write_index++] = FromCharacterToByte(value[character_index], character_index);
                character_index += 1;
            }

            for (var read_index = character_index; read_index < value.Length; read_index += 2)
            {
                var upper = FromCharacterToByte(value[read_index], read_index, 4);
                var lower = FromCharacterToByte(value[read_index + 1], read_index + 1);

                bytes[write_index++] = (byte)(upper | lower);
            }
        }

        return bytes;
    }

    private static byte FromCharacterToByte(char character, int index, int shift = 0)
    {
        var value = (byte)character;
        if (0x40 < value && 0x47 > value || 0x60 < value && 0x67 > value)
        {
            if (0x40 == (0x40 & value))
                if (0x20 == (0x20 & value))
                    value = (byte)((value + 0xA - 0x61) << shift);
                else
                    value = (byte)((value + 0xA - 0x41) << shift);
        }
        else if (0x29 < value && 0x40 > value)
        {
            value = (byte)((value - 0x30) << shift);
        }
        else
        {
            throw new FormatException(string.Format(
                "Character '{0}' at index '{1}' is not valid alphanumeric character.", character, index));
        }

        return value;
    }
}
