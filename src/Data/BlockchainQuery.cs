﻿// Copyright (c) EverRise Pte Ltd. All rights reserved.

using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using EverStats.Config;
using System.Data;
using EverStats.Services;

namespace EverStats.Data;

public class BlockchainQuery
{
    private static string s_riseContract = "0xC17c30e98541188614dF99239cABD40280810cA3";
    private static string s_buybackWalletRaw = "78b939518f51b6da10afb3c3238Dd04014e00057";
    private static string s_buybackWallet = "0x" + s_buybackWalletRaw;
    private static string s_distributorRaw = "3776B8C349BC9Af202E4D98Af163D59cA56d2fC5";
    private static string s_distributor = "0x" + s_distributorRaw;
    private static string s_veRiseContract = "0xDbA7b24257fC6e397cB7368B4BC922E944072f1b";
    private static string s_nftRiseAddressRaw = "23cD2E6b283754Fd2340a75732f9DdBb5d11807e";
    private static string s_nftRiseAddress = "0x" + s_nftRiseAddressRaw;
    private static string s_memeRiseAddressRaw = "1C57a5eE9C5A90C9a5e31B5265175e0642b943b1";
    private static string s_memeRiseAddress = "0x" + s_memeRiseAddressRaw;
    private readonly static JsonSerializerOptions s_options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    private readonly static MediaTypeWithQualityHeaderValue s_jsonAccept = MediaTypeWithQualityHeaderValue.Parse("application/json");

    public EndPoints CurrentEndpoints { get; }
    public EndPoints ArchiveEndpoints { get; }
    private readonly string _statsContractAddress;
    private readonly string _everSwapAddress;
    private HttpClient _nodeClient;
    private HttpClient _web3Client;

    private decimal _lastBlock = 0;
    private BlockchainStats _stats;
    private readonly string _chain;
    private ApiConfig _config;

    private DateTimeOffset _avaxFtmLaunch = new DateTimeOffset(new DateTime(2022, 04, 04, 1, 0, 00));

    private ILogger<BlockchainQuery> _logger;

    public BlockchainQuery(string chain, ApiConfig config, BlockchainStats stats, EndPoints currentEndpoints, EndPoints archiveEndpoints, string statsContractAddress, string everSwapAddress, ILogger<BlockchainQuery> logger)
    {
        _chain = chain;
        _config = config;
        CurrentEndpoints = currentEndpoints;
        ArchiveEndpoints = archiveEndpoints;
        _statsContractAddress = statsContractAddress;
        _everSwapAddress = everSwapAddress;
        _stats = stats;
        _nodeClient = GetHttpClient();
        _web3Client = GetHttpClient();
        _web3Client.DefaultRequestHeaders.Add("X-API-Key", config.MoralisConfiguration.Web3ApiKey);

        _logger = logger;
    }

    private static HttpClient GetHttpClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(s_jsonAccept);
        return client;
    }

    public enum Period
    {
        History24hr,
        History48hr,
        History7day,
        History14day,
    }


    public async Task<BlockchainSample> GetData(ulong blockNumber)
    {
        return await QueryStats(ArchiveEndpoints, "0x" + NumberToHex((int)blockNumber));
    }

    public async Task GetData()
    {
        var lastStore = _stats?.current?.lastStored;
        _stats.current = await QueryStats(CurrentEndpoints, "latest", checkBlock: true);
        _stats.current.lastStored = lastStore;

        if (_stats.history24hrs is null)
        {
            _stats.history24hrs = _stats.current;
        }
        if (_stats.history48hrs is null)
        {
            _stats.history48hrs = _stats.current;
        }
        if (_stats.history7day is null)
        {
            _stats.history7day = _stats.current;
        }
        if (_stats.history14day is null)
        {
            _stats.history14day = _stats.current;
        }
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

    //public Task<BlockchainSample?> QueryHistoricStats(int blockNumber)
    //{
    //    return QueryStats(ArchiveEndpoints, "0x" + NumberToHex(blockNumber));
    //}

    private async Task<BlockchainSample?> QueryStats(EndPoints apiEndpoints, string blockNumber, bool checkBlock = false)
    {
        if (apiEndpoints.Length == 0) return null;

        var rpc = new[] {
            new RpcCall(
                address: _statsContractAddress,
                method: "0xc59d4847000000000000000000000000",
                id: 1,
                blockNumber),
            new RpcCall(
                address: s_veRiseContract,
                method: "0x18160ddd000000000000000000000000",
                id: 2,
                blockNumber),
            // Burn
            new RpcCall(
                address: s_riseContract,
                method: "0x70a08231000000000000000000000000000000000000000000000000000000000000dead",
                id: 3,
                blockNumber),
            new RpcCall(
                address: _everSwapAddress,
                method: "0x588eb7c5000000000000000000000000",
                id: 4,
                blockNumber),
            // Rewards Buyback
            new RpcCall(
                address: s_buybackWallet,
                id: 5,
                blockNumber
            ),
            // Rewards RISE
            new RpcCall(
                address: s_riseContract,
                method: "0x70a08231000000000000000000000000" + s_buybackWalletRaw,
                id: 6,
                blockNumber),
            // Distributor RISE
            new RpcCall(
                address: s_riseContract,
                method: "0x70a08231000000000000000000000000" + s_distributorRaw,
                id: 7,
                blockNumber),
            // nftRISE RISE
            new RpcCall(
                address: s_riseContract,
                method: "0x70a08231000000000000000000000000" + s_nftRiseAddressRaw,
                id: 8,
                blockNumber),
            // count of nftRISE
            new RpcCall(
                address: s_nftRiseAddress,
                method: "0x18160ddd000000000000000000000000",
                id: 9,
                blockNumber),
            // count of memeRISE
            new RpcCall(
                address: s_memeRiseAddress,
                method: "0x18160ddd000000000000000000000000",
                id: 10,
                blockNumber),

        };

        var json = JsonSerializer.Serialize(rpc, s_options);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        RpcResult[]? response = null;
        while (response is null)
        {
            foreach (var apiEndpoint in apiEndpoints)
            {
                try
                {
                    response = await QueryEndpoint(apiEndpoint, content);
                    if (response is null)
                    {
                        //_logger.LogError($"{apiEndpoint} : {response.error.message}");
                        continue;
                    }

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
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"{_chain} : {ex.Message}");
                    apiEndpoints.FailedEndpoint(apiEndpoint);
                    continue;
                }
            }

            _logger.LogError($"{_chain} : Tried {apiEndpoints.Length} endpoints, none sucessful.");
            _nodeClient?.Dispose();
            _nodeClient = GetHttpClient();

            throw new DataException($" Last response: {JsonSerializer.Serialize(response)}");

            await Task.Delay(200);
        }

        return null;
    }

    static RpcResult? GetRespose(int id, RpcResult[] responses)
    {
        for (int i = 0; i < responses.Length; i++)
        {
            var response = responses[i];
            if (response.id == id) return response;
        }

        return null;
    }

    private BlockchainSample GetData(RpcResult[] responses)
    {
        RpcResult responseStats = GetRespose(1, responses);
        if (responseStats?.error is not null)
        {
            throw new DataException(responseStats.error.message);
        }

        var quatities = new BlockchainSample();

        var length = 64;
        var hex = responseStats.result.AsSpan(2);

        quatities.reservesCoinBalanceValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.liquidityTokenValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.liquidityCoinValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        //quatities.stakedValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        //quatities.aveMultiplierValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.rewardsValue = HexToDecimal(hex.Slice(0, length));
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
        quatities.timeStampValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);
        quatities.holdersValue = HexToDecimal(hex.Slice(0, length));
        hex = hex.Slice(length);

        quatities.date = (DateTime.UnixEpoch + TimeSpan.FromSeconds((int)quatities.timeStampValue)).ToString("s");

        var ten = 10;
        var tokenDivisor = (decimal)Math.Pow(ten, (double)HexToDecimal(hex.Slice(0, length)));
        hex = hex.Slice(length);
        var coinDivisor = (decimal)Math.Pow(ten, (double)HexToDecimal(hex.Slice(0, length)));
        hex = hex.Slice(length);
        var stableDivisor = (decimal)Math.Pow(ten, (double)HexToDecimal(hex.Slice(0, length)));
        hex = hex.Slice(length);
        var multiplierDivisor = (decimal)Math.Pow(ten, (double)HexToDecimal(hex.Slice(0, length)));
        hex = hex.Slice(length);

        quatities.reservesCoinBalanceValue = quatities.reservesCoinBalanceValue / coinDivisor;
        quatities.liquidityTokenValue = quatities.liquidityTokenValue / tokenDivisor;
        quatities.liquidityCoinValue = quatities.liquidityCoinValue / coinDivisor;
        //quatities.stakedValue = quatities.stakedValue / tokenDivisor;
        quatities.rewardsValue = quatities.rewardsValue / tokenDivisor;

        //quatities.aveMultiplierValue = quatities.aveMultiplierValue / multiplierDivisor;
        quatities.volumeTransfersValue = quatities.volumeTransfersValue / tokenDivisor;
        quatities.volumeBuyValue = quatities.volumeBuyValue / tokenDivisor;
        quatities.volumeSellValue = quatities.volumeSellValue / tokenDivisor;
        quatities.volumeTradeValue = quatities.volumeTradeValue / tokenDivisor;
        quatities.bridgeVaultValue = quatities.bridgeVaultValue / tokenDivisor;
        quatities.tokenPriceCoinValue = quatities.tokenPriceCoinValue / coinDivisor;
        quatities.coinPriceStableValue = quatities.coinPriceStableValue / stableDivisor;
        quatities.tokenPriceStableValue = quatities.tokenPriceStableValue / stableDivisor;
        quatities.marketCapValue = quatities.marketCapValue / stableDivisor;

        quatities.usdReservesCoinBalanceValue = quatities.reservesCoinBalanceValue * quatities.coinPriceStableValue;
        quatities.usdLiquidityTokenValue = quatities.liquidityTokenValue * quatities.tokenPriceStableValue;
        quatities.usdLiquidityCoinValue = quatities.liquidityCoinValue * quatities.coinPriceStableValue;
        //quatities.usdStakedValue = quatities.stakedValue * quatities.tokenPriceStableValue;
        quatities.usdRewardsValue = quatities.rewardsValue * quatities.tokenPriceStableValue;
        quatities.usdVolumeTransfersValue = quatities.volumeTransfersValue * quatities.tokenPriceStableValue;
        quatities.usdVolumeBuyValue = quatities.volumeBuyValue * quatities.tokenPriceStableValue;
        quatities.usdVolumeSellValue = quatities.volumeSellValue * quatities.tokenPriceStableValue;
        quatities.usdVolumeTradeValue = quatities.volumeTradeValue * quatities.tokenPriceStableValue;

        RpcResult responseSupply = GetRespose(2, responses);

        length = 64;
        hex = responseSupply.result.AsSpan(2);

        quatities.veAmountValue = HexToDecimal(hex.Slice(0, length), false, tokenDivisor);

        RpcResult responseBurn = GetRespose(3, responses);

        length = 64;
        hex = responseBurn.result.AsSpan(2);

        quatities.burnValue = HexToDecimal(hex.Slice(0, length), false, tokenDivisor);
        quatities.burnPercentValue = quatities.burnValue / 71_618_033_988m;

        RpcResult responseEverSwap = GetRespose(4, responses);
        hex = responseEverSwap.result.AsSpan(2);

        quatities.everSwapValue = hex.Length == 0 ? 0 : HexToDecimal(hex.Slice(0, length), false, coinDivisor);
        quatities.usdEverSwapValue = quatities.everSwapValue * quatities.coinPriceStableValue;

        RpcResult buybackRewards = GetRespose(5, responses);
        hex = buybackRewards.result.AsSpan(2);

        var extraReserves = hex.Length <= 1 ? 0 : HexToDecimal(hex, false, coinDivisor);

        quatities.reservesCoinBalanceValue += extraReserves;
        quatities.usdReservesCoinBalanceValue = quatities.reservesCoinBalanceValue * quatities.coinPriceStableValue;

        RpcResult rewards = GetRespose(6, responses);
        hex = rewards.result.AsSpan(2);

        var extraRewards = hex.Length <= 1 ? 0 : HexToDecimal(hex, false, tokenDivisor);

        RpcResult distributingRewards = GetRespose(7, responses);
        hex = distributingRewards.result.AsSpan(2);

        extraRewards += hex.Length <= 1 ? 0 : HexToDecimal(hex, false, tokenDivisor);

        quatities.reservesTokenBalanceValue = extraRewards;
        quatities.usdReservesTokenBalanceValue = quatities.reservesTokenBalanceValue * quatities.tokenPriceStableValue;

        quatities.usdReservesBalanceValue = quatities.usdReservesTokenBalanceValue + quatities.usdReservesCoinBalanceValue;

        RpcResult unclaimedRewardsResponse = GetRespose(8, responses);
        hex = unclaimedRewardsResponse.result.AsSpan(2);

        var unclaimedRewards = hex.Length <= 1 ? 0 : HexToDecimal(hex, false, tokenDivisor);

        quatities.unclaimedTokenBalanceValue = unclaimedRewards;
        quatities.usdUnclaimedTokenBalanceValue = unclaimedRewards * quatities.tokenPriceStableValue;

        RpcResult stakesResponse = GetRespose(9, responses);
        hex = stakesResponse.result.AsSpan(2);

        var stakesCount = hex.Length <= 1 ? 0 : HexToDecimal(hex, false);

        quatities.stakesCountValue = stakesCount;


        RpcResult mementosCountResponse = GetRespose(10, responses);
        hex = mementosCountResponse.result.AsSpan(2);

        var mementosCount = hex.Length <= 1 ? 0 : HexToDecimal(hex, false);

        quatities.mementosCountValue = mementosCount;

        return quatities;
    }

    private async Task<RpcResult[]?> QueryEndpoint(string endpoint, HttpContent content)
    {
        using var response = await _nodeClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<RpcResult[]>(json);
    }


    public decimal HexToDecimal(ReadOnlySpan<char> hex, bool isHexLittleEndian = false, decimal divisor = 1m)
    {
        if (hex.Length == 0 || hex == "0" || hex == "0x0") return 0;

        var encoded = HexToByteArray(hex);

        if (BitConverter.IsLittleEndian != isHexLittleEndian)
        {
            var listEncoded = encoded.ToList();
            listEncoded.Insert(0, 0x00);
            encoded = listEncoded.ToArray().Reverse().ToArray();
        }

        var nominator = new BigInteger(encoded);
        if (divisor != 1m)
        {
            if (nominator > new BigInteger(decimal.MaxValue))
            {
                return (decimal)(nominator / new BigInteger(divisor));
            }
            else
            {
                return ((decimal)nominator / (decimal)new BigInteger(divisor));
            }
        }  
        return ((decimal)nominator);
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
