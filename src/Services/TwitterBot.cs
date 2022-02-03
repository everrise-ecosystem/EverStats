// Copyright (c) EverRise Pte Ltd. All rights reserved.

using System.Text;
using System.Text.Json;

using LinqToTwitter;
using LinqToTwitter.OAuth;
using EverStats.Config;
using EverStats.Data;
using Svg.Skia;
using SkiaSharp;

namespace EverStats.Services;
public class TwitterBot : IHostedService
{
    static StringBuilder s_svg = new StringBuilder();
    private ApiConfig _config;
    private Stats _stats;
    private CancellationTokenSource _cts = new CancellationTokenSource();
    private Task _generateTweets;

    public TwitterBot(ApiConfig config, Stats stats)
    {
        _config = config;
        _stats = stats;
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

    private async Task GenerateTweets()
    {
        var sb = new StringBuilder();
        await _stats.DataReceived();
        await Task.Delay(60_000);

        while (true)
        {
            var delay = 600_000;
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
        var (price, change) = GetPriceChange(_stats.unified);
        sb.AppendLine($"💵 ${price:0.0000000} Δ {FormatChange(change)} {ChangeIcon(change)}");
        sb.AppendLine();

        (price, change) = GetPriceChange(_stats.eth);
        sb.AppendLine($"${price:0.0000000} (Eth) Δ {FormatChange(change)} {ChangeIcon(change)}");

        (price, change) = GetPriceChange(_stats.bsc);
        sb.AppendLine($"${price:0.0000000} (BSC) Δ {FormatChange(change)} {ChangeIcon(change)}");

        (price, change) = GetPriceChange(_stats.poly);
        sb.AppendLine($"${price:0.0000000} (Poly) Δ {FormatChange(change)} {ChangeIcon(change)}");

        (price, change) = GetPriceChange(_stats.ftm);
        sb.AppendLine($"${price:0.0000000} (Ftm) Δ {FormatChange(change)} {ChangeIcon(change)}");

        (price, change) = GetPriceChange(_stats.avax);
        sb.AppendLine($"${price:0.0000000} (Avax) Δ {FormatChange(change)} {ChangeIcon(change)}");

        sb.AppendLine();
        change = GetChange(_stats.unified.history24hrs.marketCapValue, _stats.unified.current.marketCapValue);
        sb.AppendLine($"💰 ${FormatNumber(_stats.unified.current.marketCapValue)} MC {FormatChange(change)}");

        change = GetChange(_stats.unified.history24hrs.holdersValue, _stats.unified.current.holdersValue);
        sb.AppendLine($"🤝 {_stats.unified.current.holders:N0} Hodlrs {FormatChange(change)}");

        sb.AppendLine();
        change = GetChange(_stats.unified.history24hrs.usdStakedValue, _stats.unified.current.usdStakedValue);
        sb.AppendLine($"🔐 ${FormatNumber(_stats.unified.current.usdStakedValue)} Staked TVL");// {FormatChange(change)}");
        sb.AppendLine();
        change = GetPercent(_stats.unified.current.usdLiquidityCoinValue, _stats.unified.current.marketCapValue);
        sb.AppendLine($"💧 ${FormatNumber(_stats.unified.current.usdLiquidityCoinValue * 2)} LP");
        change = GetPercent(_stats.unified.current.usdReservesBalanceValue, _stats.unified.current.marketCapValue);
        sb.AppendLine($"🐳 ${FormatNumber(_stats.unified.current.usdReservesBalanceValue)} Buyback");
        sb.AppendLine();
        sb.Append("#EverRise");

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

        var svgXml = GetSpreadSvg(_stats);
        byte[] imageData;
        using (var svg = new SKSvg())
        {
            svg.Load(new MemoryStream(Encoding.UTF8.GetBytes(svgXml ?? "")));
            var bitmapSteam = new MemoryStream();
            svg.Picture.ToImage(bitmapSteam, SKColors.Empty, SKEncodedImageFormat.Png, 100, 1f, 1f, SKColorType.Rgb888x, SKAlphaType.Premul, SKSvgSettings.s_srgb);
            imageData = bitmapSteam.ToArray();
        }

        if (imageData != null)
        {
            var uploadedMedia = await twitterContext.UploadMediaAsync(imageData, "image/png", "tweet_image");
            var mediaIds = new List<string> { uploadedMedia.MediaID.ToString("0") };
            await twitterContext.TweetMediaAsync(tweet, mediaIds);
        }
        else
        {
            await twitterContext.TweetAsync(tweet);
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

    private static string GetSpreadSvg(Stats stats)
    {
        var sb = s_svg;
        sb.Clear();

        var unifiedPrice = stats.unified.current.tokenPriceStableValue;
        var prices = new decimal[]
        {
                stats.bsc.current.tokenPriceStableValue,
                stats.eth.current.tokenPriceStableValue,
                stats.poly.current.tokenPriceStableValue,
                stats.ftm.current.tokenPriceStableValue,
                stats.avax.current.tokenPriceStableValue,
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

        var graphWidth = 420;
        sb.Append($@"<svg version=""1.1"" xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" x=""0px"" y=""0px"" viewBox=""0 0 {graphWidth} {graphWidth / 2}"" xml:space=""preserve"">");
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
    fill: rgb(0, 17, 48);
}
</style>
");
        sb.Append($@"<g>
                <rect class=""graph-background"" width=""{graphWidth}"" height=""{graphWidth / 2}"" />

                <g transform=""translate(0, 55)"" class=""graph-priceRange"">
                <text text-anchor=""middle"" x=""{graphWidth / 2}"" y=""0"" class=""graph-text-title"">EverBridge Arbitrage</text>
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

            sb.Append(@$"<image class=""graph-icon-rise"" x=""{x - 8:0.00}"" y=""8"" href=""https://data.everrise.com/icons/smartchain/0x0cd022dde27169b20895e0e2b2b8a33b25e63579.png"" height=""16"" width=""16""/>");

        }

        sb.Append(@"</g></g></g>");
        sb.Append(@"</svg>");

        return sb.ToString();
    }

    private static (decimal price, decimal change) GetPriceChange(BlockchainStats chain)
    {
        return (chain.current.tokenPriceStableValue, (chain.current.tokenPriceStableValue - chain.history24hrs.tokenPriceStableValue) / chain.history24hrs.tokenPriceStableValue);
    }

    static string FormatPercent(decimal change)
    {
        if (change == decimal.MaxValue) return "♾️%";
        return $"{change:P1}";
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