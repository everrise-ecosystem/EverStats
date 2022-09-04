// Copyright (c) EverRise Pte Ltd. All rights reserved.

using System.Text;
using System.Text.Json;

using LinqToTwitter;
using LinqToTwitter.OAuth;
using EverStats.Config;
using EverStats.Data;
using Svg.Skia;
using SkiaSharp;

using Microsoft.Data.SqlClient;
using System.Data;
using EverStats.Api;

namespace EverStats.Services;
public class TwitterBot : IHostedService
{
    static StringBuilder s_svg = new StringBuilder();
    private ApiConfig _config;
    private ILogger _logger;
    private CancellationTokenSource _cts = new CancellationTokenSource();
    private Task _generateTweets;

    public TwitterBot(ApiConfig config, ILoggerFactory loggerFactory)
    {
        _config = config;
        _logger = loggerFactory.CreateLogger<TwitterBot>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _generateTweets = GenerateTweets();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();

        return Task.CompletedTask;
    }

    public class OhlcVol
    {
        public DateOnly dateValue;
        public decimal openValue;
        public decimal lowValue;
        public decimal highValue;
        public decimal closeValue;
        public decimal averageValue;
        public decimal volumeValue;

        public string date { get; set; }
        public string open { get; set; }
        public string low { get; set; }
        public string high { get; set; }
        public string close { get; set; }
        public string average { get; set; }
        public string volume { get; set; }

        public void CreateStringRepresentations()
        {
            date = dateValue.ToString("yyyy-MM-dd");
            open = openValue.ToString("0.00000000");
            low = lowValue.ToString("0.00000000");
            high = highValue.ToString("0.00000000");
            close = closeValue.ToString("0.00000000");
            average = averageValue.ToString("0.00000000");
            volume = volumeValue.ToString("0.00000000");
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
            c.reservesTokenBalance= reader.GetDecimal("reservesTokenBalance");
            c.liquidityToken= reader.GetDecimal("liquidityToken");
            c.liquidityCoin= reader.GetDecimal("liquidityCoin");
            c.veAmount= reader.GetDecimal("veAmount");
            c.staked= reader.GetDecimal("staked");
            c.aveMultiplier= reader.GetDecimal("aveMultiplier");
            c.rewards= reader.GetDecimal("rewards");
            c.volumeBuy= reader.GetDecimal("volumeBuy");
            c.volumeSell= reader.GetDecimal("volumeSell");
            c.volumeTrade= reader.GetDecimal("volumeTrade");
            c.bridgeVault= reader.GetDecimal("bridgeVault");
            c.tokenPriceCoin= reader.GetDecimal("tokenPriceCoin");
            c.coinPriceStable= reader.GetDecimal("coinPriceStable");
            c.tokenPriceStable= reader.GetDecimal("tokenPriceStable");
            c.marketCap= reader.GetDecimal("marketCap");
            c.blockNumber= reader.GetDecimal("blockNumber");
            c.holders= reader.GetDecimal("holders");
            c.burn= reader.GetDecimal("burn");
            c.burnPercent= reader.GetDecimal("burnPercent");
            c.totalSupply= reader.GetDecimal("totalSupply");
            c.everSwap= reader.GetDecimal("everSwap");
            c.usdReservesCoinBalance= reader.GetDecimal("usdReservesCoinBalance");
            c.usdReservesTokenBalance= reader.GetDecimal("usdReservesTokenBalance");
            c.usdReservesBalance= reader.GetDecimal("usdReservesBalance");
            c.usdLiquidityToken= reader.GetDecimal("usdLiquidityToken");
            c.usdLiquidityCoin= reader.GetDecimal("usdLiquidityCoin");
            c.usdStaked= reader.GetDecimal("usdStaked");
            c.usdRewards= reader.GetDecimal("usdRewards");
            c.usdVolumeBuy= reader.GetDecimal("usdVolumeBuy");
            c.usdVolumeSell= reader.GetDecimal("usdVolumeSell");
            c.usdVolumeTrade= reader.GetDecimal("usdVolumeTrade");
            c.usdEverSwap= reader.GetDecimal("usdEverSwap");
            c.supplyOnChainPercent= reader.GetDecimal("supplyOnChainPercent");
            c.stakedOfTotalSupplyPercent= reader.GetDecimal("stakedOfTotalSupplyPercent");
            c.stakedOfOnChainPercent= reader.GetDecimal("stakedOfOnChainPercent");
            c.stakedOfTotalStakedPercent= reader.GetDecimal("stakedOfTotalStakedPercent");
            c.veRiseOnChainPercent= reader.GetDecimal("veRiseOnChainPercent");

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

    private async Task GenerateTweets()
    {
        var sb = new StringBuilder();

        while (true)
        {
            var delay = 300_000;
            try
            {
                await GenerateTweets(sb);
            }
            catch (Exception ex)
            {
                delay = 60_000;
                Console.WriteLine(ex);
            }
            sb.Clear();
            await Task.Delay(delay);
        }
    }

    private async Task GenerateTweets(StringBuilder sb)
    {
        var data = await GetChainData();
        var (price, change) = GetPriceChange(data.unified);
        sb.AppendLine($"#EverRise $RISE");
        sb.AppendLine();
        sb.AppendLine($"${price:0.000000} {ChangeIcon(change)} {FormatChange(change)}");
        change = GetChange(data.unified.history24hrs?.marketCap ?? 0, data.unified.current.marketCap);
        sb.AppendLine($"${FormatNumber(data.unified.current.marketCap)} Market Cap {FormatChange(change)}");

        change = GetChange(data.unified.history24hrs?.holders ?? 0, data.unified.current.holders);
        sb.AppendLine($"{data.unified.current.holders:N0} Hodlrs {FormatChange(change)}");

        sb.AppendLine();

        (price, change) = GetPriceChange(data.eth);
        sb.AppendLine($"${price:0.000000} {ChangeIcon(change)} {FormatChange(change)} #Eth");

        (price, change) = GetPriceChange(data.bsc);
        sb.AppendLine($"${price:0.000000} {ChangeIcon(change)} {FormatChange(change)} #BNB");

        (price, change) = GetPriceChange(data.poly);
        sb.AppendLine($"${price:0.000000} {ChangeIcon(change)} {FormatChange(change)} #Polygon");

        (price, change) = GetPriceChange(data.ftm);
        sb.AppendLine($"${price:0.000000} {ChangeIcon(change)} {FormatChange(change)} #Ftm");

        (price, change) = GetPriceChange(data.avax);
        sb.AppendLine($"${price:0.000000} {ChangeIcon(change)} {FormatChange(change)} #Avax");

        sb.AppendLine();

        //change = GetChange(data.unified.history24hrs?.burnPercent ?? 0, data.unified.current.burnPercent);
        sb.AppendLine($"{FormatPercent(data.unified.current.burnPercent)} Burn"); //{FormatChange(change)}");
        change = GetChange(data.unified.history24hrs?.usdStaked ?? 0, data.unified.current.usdStaked);
        sb.AppendLine($"${FormatNumber(data.unified.current.usdStaked)} Staked TVL ({FormatPercent(data.unified.current.stakedOfTotalSupplyPercent)})");// {FormatChange(change)}");
        sb.AppendLine();
        change = GetPercent(data.unified.current.usdLiquidityCoin, data.unified.current.marketCap);
        sb.AppendLine($"${FormatNumber(data.unified.current.usdLiquidityCoin * 2)} LP");
        change = GetPercent(data.unified.current.usdReservesBalance, data.unified.current.marketCap);
        sb.Append($"${FormatNumber(data.unified.current.usdReservesBalance)} Reserves");
        //sb.AppendLine();
        //sb.Append("#BinanceSmartChain #BSC");

        var tweet = sb.ToString();
        sb.Clear();

        var twitterContext = new TwitterContext(new SingleUserAuthorizer
        {
            CredentialStore = new SingleUserInMemoryCredentialStore
            {
                ConsumerKey = _config.TwitterConfiguration.ConsumerKey,
                ConsumerSecret = _config.TwitterConfiguration.ConsumerSecret,
                AccessToken = _config.TwitterConfiguration.AccessToken,
                AccessTokenSecret = _config.TwitterConfiguration.AccessTokenSecret,
            }
        });

        var svgXml = GetSpreadSvg(data);
        byte[] imageData;
        using (var svg = new SKSvg())
        {
            svg.Load(new MemoryStream(Encoding.UTF8.GetBytes(svgXml ?? "")));
            var bitmapSteam = new MemoryStream();
            svg.Picture.ToImage(bitmapSteam, SKColors.Empty, SKEncodedImageFormat.Png, 100, 1f, 1f, SKColorType.Rgb888x, SKAlphaType.Premul, SKSvgSettings.s_srgb);
            imageData = bitmapSteam.ToArray();
        }

        File.WriteAllBytes(@"C:\GitHub\everrise-ecosystem\data\docs\graph.png", imageData);
        //imageData = File.ReadAllBytes(@"C:\Users\thund\OneDrive\Pictures\coin\binance.jpg");

        if (_config.SendTweets)
        {
            var wasError = false;
            if (imageData != null)
            {
                try
                {
                    var uploadedMedia = await twitterContext.UploadMediaAsync(imageData, "image/png", "tweet_image");
                    var mediaIds = new List<string> { uploadedMedia.MediaID.ToString("0") };
                    await twitterContext.TweetMediaAsync(tweet, mediaIds);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    wasError = true;
                }
            }

            if (imageData is null || wasError)
            {
                await twitterContext.TweetAsync(tweet);
            }
        }

        Console.WriteLine(tweet);
        Console.WriteLine("");
        Console.WriteLine("*****");
        Console.WriteLine("");
    }

    private static string[] colors = new[]
    {
        "rgba(234,185,2,1)",
        "rgba(103,109,143,1)",
        "rgba(124,65,213,1)",
        "rgba(25,105,255,1)",
        "rgba(232,65,66,1)",
        "rgba(255,255,255,1)"
    };

    private static string GetSpreadSvg(Chains stats)
    {
        var sb = s_svg;
        sb.Clear();

        var graphWidth = 420;
        sb.Append($@"<svg version=""1.1"" xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" x=""0px"" y=""0px"" viewBox=""0 0 {graphWidth} {graphWidth}"" xml:space=""preserve"">");
        sb.Append(@"
<style type=""text/css"">
.graph-text-title{
    font-family: 'Montserrat';
    stroke: none;
    font-size: 12px;
    fill: white;
    font-weight:bold;
}
.graph-text-percent, .graph-text-price{
    font-family: 'Lato';
    stroke: none;
    font-size: 11px;
}
.graph-text-percent{
    fill: silver;
}
.graph-text-price{
    fill: silver;
    font-size: 9px;
}
.graph-background {
    fill: rgb(0, 8, 24);
}
.redBar {
    stroke: rgba(255, 0, 0, 0.5);
    fill: rgba(128, 0, 0, 0.5);
}
.greenBar {
    stroke: rgba(0, 255, 0, 0.5);
    fill: rgba(0, 128, 0, 0.5);
}
</style>
");
        sb.Append($@"<g>
                <rect class=""graph-background"" width=""{graphWidth}"" height=""{graphWidth}"" />");

        DrawSpread(stats, graphWidth, sb);
        DrawTimeSeries(stats, graphWidth, sb);

        sb.Append(@"</g></svg>");

        return sb.ToString();
    }

    private static void DrawTimeSeries(Chains stats, int graphWidth, StringBuilder sb)
    {
        var minVol = decimal.MaxValue;
        var maxVol = decimal.MinValue;

        foreach (var ohlc in stats.unified.ohlc)
        {
            if (maxVol < ohlc.volumeValue) maxVol = ohlc.volumeValue;
            if (minVol > ohlc.volumeValue) minVol = ohlc.volumeValue;
        }

        var prices = new List<OhlcVol>[]
        {
            stats.bsc.ohlc,
            stats.eth.ohlc,
            stats.poly.ohlc,
            stats.ftm.ohlc,
            stats.avax.ohlc,
            stats.unified.ohlc
        };

        var minPrice = decimal.MaxValue;
        var maxPrice = decimal.MinValue;
        foreach (var ohlcList in prices)
        {
            foreach (var ohlc in ohlcList)
            {
                if (maxPrice < ohlc.averageValue) maxPrice = ohlc.averageValue;
                if (minPrice > ohlc.averageValue) minPrice = ohlc.averageValue;
            }
        }

        sb.Append($@"
                <g transform=""translate(0, {graphWidth / 3:0.0})"" class=""graph-priceGraph"">
                <text text-anchor=""middle"" x=""{graphWidth / 2:0.0}"" y=""0"" class=""graph-text-title"">Last 14 Days Arbitrage</text>
                <g transform=""translate(0, 20)"" class=""graph-priceRange"">");

        sb.Append($@"<path fill=""none"" stroke=""rgb(128,128,128)"" d=""M 15 {graphWidth / 2 + 35 } h {graphWidth - 20} M 15 {graphWidth / 2 + 35} v -{graphWidth / 2 + 35} "" />");

        var barWidth = (decimal)graphWidth / (stats.unified.ohlc.Count + 2);
        var x = barWidth;
        var volRange = maxVol - minVol;
        var maxHeight = graphWidth / 16m + graphWidth / 4m;

        foreach (var ohlc in stats.unified.ohlc)
        {
            var color = ohlc.openValue > ohlc.closeValue ? "redBar" : "greenBar";
            var vol = (ohlc.volumeValue - minVol) / volRange;
            var height = graphWidth / 32 + vol * graphWidth / 8;

            sb.Append($@"
                <rect class=""{color}"" x=""{x + 4:0.0}"" y=""{graphWidth / 4 + maxHeight - height:0.0}"" width=""{barWidth - 4:0.0}"" height=""{height:0.0}"" />");

            x += barWidth;
        }

        var priceRange = maxPrice - minPrice;
        var colorIndex = 0;
        maxHeight = graphWidth / 2m;
        foreach (var ohlcList in prices)
        {
            sb.Append($@"
                <path stroke=""{colors[colorIndex]}""  stroke-width=""{(colorIndex < 5 ? 2 : 4)}"" fill=""none"" d=""");
            x = barWidth * 1.5m;
            var type = "M";
            foreach (var ohlc in ohlcList)
            {
                var y = maxHeight - (ohlc.averageValue - minPrice) / priceRange * maxHeight;
                sb.Append($"{type} {x:0.0} {y:0.0} ");
                type = "L";
                x += barWidth;
            }
            sb.Append($@""" />");

            colorIndex++;
        }

        sb.Append($@"
                </g></g>");

    }

    private static void DrawSpread(Chains stats, int graphWidth, StringBuilder sb)
    {
        var unifiedPrice = stats.unified.current.tokenPriceStable;
        var prices = new decimal[]
        {
            stats.bsc.current.tokenPriceStable,
            stats.eth.current.tokenPriceStable,
            stats.poly.current.tokenPriceStable,
            stats.ftm.current.tokenPriceStable,
            stats.avax.current.tokenPriceStable,
        };

        var min = 1000000m;
        var max = 0m;
        for (var i = 0; i < prices.Length; i++)
        {
            var value = prices[i];
            if (min > value)
            {
                min = value;
            }
            if (max < value)
            {
                max = value;
            }
        }

        var range = max - min;
        var buffer = range * 0.025m;
        var minX = min - buffer;
        range *= 1.05m;

        var positions = new decimal[prices.Length];

        sb.Append($@"
                <g transform=""translate(0, 20)"" class=""graph-priceRange"">
                <text text-anchor=""middle"" x=""{graphWidth / 2}"" y=""0"" class=""graph-text-title"">Current EverBridge Arbitrage</text>
                <g transform=""translate(0, 20)"" class=""graph-priceRange"">");

        for (var i = 0; i < prices.Length; i++)
        {
            var value = prices[i];

            if (value <= 0) continue;

            var x = (((value - minX) * graphWidth) / range);
            positions[i] = x;
            var xText = x.ToString("0.00");

            sb.Append(@"<image class=""");
            sb.Append(i switch
            {
                0 => @"graph-icon-bsc"" href=""https://data.everrise.com/icons/networks/smartchain.svg"" height=""16"" width=""16"" ",
                1 => @"graph-icon-eth"" href=""https://data.everrise.com/icons/networks/ethereum.svg"" height=""16"" width=""16"" ",
                2 => @"graph-icon-poly"" href=""https://data.everrise.com/icons/networks/polygon.svg"" height=""16"" width=""16"" ",
                3 => @"graph-icon-ftm"" href=""https://data.everrise.com/icons/networks/fantom.svg"" height=""16"" width=""16"" ",
                4 => @"graph-icon-avax"" href=""https://data.everrise.com/icons/networks/avalanche.svg"" height=""16"" width=""16"" ",
                _ => ""
                //3 => @"<image class=""graph-icon-rise"" href=""https://data.everrise.com/icons/smartchain/0x0cd022dde27169b20895e0e2b2b8a33b25e63579.png"" height=""16"" width=""16""/>"
            });

            sb.Append($@" x=""{x - 8:0.00}"" y=""8"" />");

            sb.Append($@"<line x1=""{xText}"" x2=""{xText}"" y1=""22"" y2=""56"" stroke=""{colors[i]}"" stroke-width=""2""/>");
        }

        if (unifiedPrice > 0)
        {
            var x = (((unifiedPrice - minX) * graphWidth) / range);
            var xText = x.ToString("0.00");

            sb.Append(@$"
                <line x1=""{xText}"" x2=""{xText}"" y1=""18"" y2=""56"" stroke=""{colors[colors.Length - 1]}"" stroke-width=""3""/>
                <text x=""5"" y=""66"" class=""graph-text-price"">{min:0.0000000}</text>
                <text x=""{xText}"" y=""66"" class=""graph-text-price"" text-anchor=""middle"">{unifiedPrice:0.0000000}</text>
                <text x=""{graphWidth - 5}"" y=""66"" class=""graph-text-price"" text-anchor=""end"">{max:0.0000000}</text>");

            for (var i = 0; i < prices.Length; i++)
            {
                var price = prices[i];
                if (price <= 0) continue;

                decimal xPos;
                if (positions[i] > x)
                {
                    xPos = x + (positions[i] - x) / 2 - 20;
                }
                else
                {
                    xPos = positions[i] + (x - positions[i]) / 2 - 20;
                }

                var percentChange = (((price - unifiedPrice) * 100) / unifiedPrice);

                var yText = (24 + i * 8);
                sb.Append(@$"<line x1=""{positions[i]:0.00}"" stroke-dasharray=""2"" x2=""{xText}"" y1=""{yText}"" y2=""{yText}"" stroke=""{colors[i]}"" stroke-width=""1""/>
                    <text x=""{xPos:0.00}"" y=""{yText - 4}"" class=""graph-text-percent"">{(percentChange > 0 ? "+" : "")}{percentChange:0.00}%</text>");

            }

            sb.Append(@$"<image class=""graph-icon-rise"" x=""{x - 8:0.00}"" y=""8"" href=""https://data.everrise.com/icons/smartchain/0xc17c30e98541188614df99239cabd40280810ca3.png"" height=""16"" width=""16""/>");

        }

        sb.Append(@"</g></g>");
    }

    private static (decimal price, decimal change) GetPriceChange(ChainSnapshot chain)
    {
        if (chain.history24hrs is not null && chain.history24hrs.tokenPriceStable != 0)
        {
            return (chain.current.tokenPriceStable, (chain.current.tokenPriceStable - chain.history24hrs.tokenPriceStable) / chain.history24hrs.tokenPriceStable);
        }

        return (chain.current.tokenPriceStable, 0);
    }

    static string FormatPercent(decimal change)
    {
        if (change == decimal.MaxValue) return "♾️%";
        return $"{change:P3}";
    }
    static string FormatChange(decimal change)
    {
        if (change == decimal.MaxValue) return "+ ♾️%";
        return $"{(change > 0 ? "+" : "")}{change:P1}";
    }
    static string ChangeIcon(decimal change)
    {
        return $"{(change > 0 ? "🟢" : "🔴")}";
    }
    static decimal GetPercent(decimal current, decimal initial)
    {
        if (initial > 0)
        {
            return current / initial;
        }

        return decimal.MaxValue;
    }
    static decimal GetChange(decimal initial, decimal current)
    {
        if (initial > 0)
        {
            return (current - initial) / initial;
        }

        return decimal.MaxValue;
    }

    static string FormatNumber(decimal value, bool doubleDecimal = false)
    {
        var suffix = "";

        if (value > 1_000_000_000_000_000)
        {
            value /= 1_000_000_000_000_000;
            suffix = "Qu";
        }
        else if (value > 1_000_000_000_000)
        {
            value /= 1_000_000_000_000;
            suffix = "Tn";
        }
        else if (value > 1_000_000_000)
        {
            value /= 1_000_000_000;
            suffix = "Bn";
        }
        else if (value > 1_000_000)
        {
            value /= 1_000_000;
            suffix = "M";
        }
        else if (value > 1_000)
        {
            value /= 1_000;
            suffix = "k";
        }

        return ((!doubleDecimal && suffix.Length > 0) ? value.ToString("N1") : value.ToString("N2")) + suffix;
    }
}