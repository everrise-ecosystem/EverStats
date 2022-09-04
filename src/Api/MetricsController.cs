using EverStats.Config;
using EverStats.Data;

using ExCSS;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

using System.Data;
using System.Drawing;
using System.Text.Json;

using static Org.BouncyCastle.Math.EC.ECCurve;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EverStats.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private JsonSerializerOptions s_options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        private readonly static ChainInfo[] s_chains =
        {
            new ("Unified", "USDT eqv"),
            new ("Ethereum", "ETH"),
            new ("BNB Chain", "BNB"),
            new ("Polygon", "MATIC"),
            new ("Avalanche", "AVAX"),
            new ("Fantom", "FTM")
        };
        private readonly static LibrarySymbolInfo[] s_allSymbols = CreateSymbols();
        private readonly static Dictionary<string, LibrarySymbolInfo> s_symbolsSearch = CreateSymbolSearch();

        public MetricsController(ApiConfig config, ILogger<MetricsController> logger)
        {
            _config = config;
            _logger = logger;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        private struct ChainInfo
        {
            public string Name;
            public string Coin;
            public string Exchange;

            public ChainInfo(string name, string coin)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Coin = coin ?? throw new ArgumentNullException(nameof(coin));
                Exchange = name == "Unified" ? name : coin;
            }
        }

        private static void AddAllChains(LibrarySymbolInfo item, List<LibrarySymbolInfo> items, bool skipUnified = false)
        {
            foreach (var chain in s_chains)
            {
                if (skipUnified && chain.Name == "Unified") continue;

                var symbol = new LibrarySymbolInfo
                {
                    symbol = item.symbol.Replace("{COIN}", chain.Coin),
                    full_name = item.full_name.Replace("{COIN}", chain.Coin),
                    description = chain.Name + " " + item.description.Replace("{COIN}", chain.Coin),
                    exchange = chain.Name.Replace("{COIN}", chain.Coin),
                    ticker = item.ticker.Replace("{EXCH}", chain.Exchange),
                    pricescale = item.pricescale
                };

                symbol.name = symbol.ticker;
                items.Add(symbol);
            }
        }
        private static Dictionary<string, LibrarySymbolInfo> CreateSymbolSearch()
        {
            var symbols = new Dictionary<string, LibrarySymbolInfo>();
            foreach (var symbol in s_allSymbols)
            {
                symbols.Add(symbol.ticker, symbol);
            }

            symbols["RISE:Unified"].pricescale = 100_000m;

            return symbols;
        }

        private static LibrarySymbolInfo[] CreateSymbols()
        {
            List<LibrarySymbolInfo> items = new();

            AddAllChains(new()
            {
                symbol = "RISE{COIN}",
                full_name = "RISE / {COIN}",
                description = "RISE / {COIN}",
                ticker = "RISE:{EXCH}"
            }, items);
            AddAllChains(new()
            {
                symbol = "RISE{COIN}USDT",
                full_name = "RISE / {COIN}USDT",
                description = "RISE / {COIN} in USDT equivalent",
                ticker = "RISEUSDT:{EXCH}",
                pricescale = 100_000m
            }, items, skipUnified: true);
            AddAllChains(new() 
            {
                symbol = "RISESTAKED",
                full_name = "RISE / STAKED",
                description = "Staked EverRise",
                ticker = "RISESTAKED:{EXCH}"
            }, items);
            AddAllChains(new()
            {
                symbol = "RISE LP",
                full_name = "RISE / LP",
                description = "EverRise RISE Liquidity",
                ticker = "RISELP:{EXCH}"
            }, items);
            AddAllChains(new()
            {
                symbol = "{COIN} LP",
                full_name = "{COIN} / LP",
                description = "EverRise {COIN} Liquidity",
                ticker = "COINLP:{EXCH}"
            }, items);
            AddAllChains(new()
            {
                symbol = "{COIN} Reserves",
                full_name = "{COIN} / Reserves",
                description = "EverRise {COIN} Reserves",
                ticker = "COINRESERVES:{EXCH}"
            }, items);
            AddAllChains(new()
            {
                symbol = "RISE Reserves",
                full_name = "RISE / Reserves",
                description = "EverRise RISE Reserves",
                ticker = "RISERESERVES:{EXCH}"
            }, items);

            return items.ToArray();
        }

        public class SearchSymbolRequest 
        {
            public string userInput { get; set; }
            public string exchange { get; set; }
            public string symbolType { get; set; }
        }

        [HttpPost(nameof(searchSymbols))]
        public LibrarySymbolInfo[] searchSymbols([FromBody] SearchSymbolRequest symbols)
        {
            return s_allSymbols;
        }

        public struct SymbolName
        {
            public string symbolName { get; set; }
        }

        // POST api/<ValuesController>
        [HttpPost(nameof(resolveSymbol))]
        public LibrarySymbolInfo? resolveSymbol([FromBody] SymbolName symbol)
        {
            return s_symbolsSearch.TryGetValue(symbol.symbolName, out var value) ? value : null;
        }
        public class HistoryMetadata
        {
            public bool? noData  { get; set; }
            public decimal? nextTime { get; set; }
        }

        public class Bar
        {
            public decimal time { get; set; }
	        public decimal open { get; set; }
            public decimal high { get; set; }
            public decimal low { get; set; }
            public decimal close { get; set; }
            public decimal? volume { get; set; }
        }

        public class BarRequestInfo
        {
            public LibrarySymbolInfo symbolInfo { get; set; }
            public string resolution { get; set; }
            public PeriodParams periodParams { get; set; }
        }

        public class BarsReturn
        {
            public Bar[] bars { get; set; }
            public HistoryMetadata? meta { get; set; }
        }

        // POST api/<ValuesController>
        [HttpPost(nameof(getBars))]
        public async Task<BarsReturn> getBars([FromBody] BarRequestInfo requestInfo)
        {
            var symbol = requestInfo.symbolInfo.ticker;
            var split = symbol.IndexOf(':');

            var type = symbol[..split];
            var chainId = symbol[(split + 1)..] switch
            {
                "Unified" => 0,
                "ETH" => 1,
                "BNB" => 56,
                "MATIC" => 137,
                "AVAX" => 43114,
                "FTM" => 250,
                _ => 0
            };

            var now = DateTime.UtcNow;
            var from = now.AddSeconds(requestInfo.periodParams.from);
            var to = now.AddSeconds(requestInfo.periodParams.to);

            var query = type switch
            {
                "RISE" when chainId != 0 => "GetRisePriceCoin",
                "RISE" when chainId == 0 => "GetRisePriceUsdt",
                "RISE" => "GetRisePriceCoin",
                "RISEUSDT" => "GetRisePriceUsdt",
                "RISESTAKED" => "GetRiseStaked",
                "RISELP" => "GetRiseLP",
                "COINLP" => "GetCoinLP",
                "COINRESERVES" => "GetCoinReserves",
                "RISERESERVES" => "GetRiseReserves",
            };

            var bars = new List<Bar>();
            if (requestInfo.periodParams.firstDataRequest)
            {
                using var conn = new SqlConnection(_config.AzureConfiguration.SqlConnection);
                await conn.OpenAsync();

                using var cmd = new SqlCommand(query, conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ChainId", chainId);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                cmd.Parameters.AddWithValue("@resolution", requestInfo.resolution);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var b = new Bar();

                    b.time = new DateTimeOffset(reader.GetDateTime(0)).ToUnixTimeMilliseconds();
                    b.open = reader.GetDecimal("Open");
                    b.high = reader.GetDecimal("Close");
                    b.low = reader.GetDecimal("Low");
                    b.close = reader.GetDecimal("Close");
                    b.volume = reader.GetDecimal("Vol");

                    bars.Add(b);
                }
            }
            var meta = new HistoryMetadata { noData = !requestInfo.periodParams.firstDataRequest };

            return new BarsReturn { bars = bars.ToArray(), meta = meta };
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private readonly ApiConfig _config;
        private readonly ILogger<MetricsController> _logger;
    }

    public class PeriodParams
    {
        public int from { get; set; }
        public int to { get; set; }
        public int countBack { get; set; }
        public bool firstDataRequest { get; set; }
    }

    public class LibrarySymbolInfo
    {
        // Symbol Name
        public string symbol { get; set; }
        public string name { get; set; }
        public string full_name { get; set; }
        public string[]? base_name { get; set; }

        // Unique symbol id
        public string? ticker { get; set; }
        public string description { get; set; }
        public string type { get; set; } = "crypto";

        // @example "1700-0200"
        public string session { get; set; } = "24x7";
        public string? session_display { get; set; }
        /**
        * @example "20181105,20181107,20181112"
        */
        public string? holidays { get; set; } = String.Empty;
        /**
        * @example "1900F4-2350F4,1000-1845:20181113;1000-1400:20181114"
        */
        public string? corrections { get; set; } = String.Empty;
        /**
        * Traded exchange
        * @example "NYSE"
        */
        public string exchange { get; set; }
        public string? listed_exchange { get; set; }
        public string timezone { get; set; } = "Etc/UTC";
        /**
        * Prices format: "price" or "volume"
        */
        public string format { get; set; } = SeriesFormat.price.ToString();
        /**
        * Code (Tick)
        * @example 8/16/.../256 (1/8/100 1/16/100 ... 1/256/100) or 1/10/.../10000000 (1 0.1 ... 0.0000001)
        */
        public decimal pricescale { get; set; } = 1m;
        /**
        * The number of units that make up one tick.
        * @example For example, U.S. equities are quotes in decimals, and tick in decimals, and can go up +/- .01. So the tick increment is 1. But the e-mini S&P futures contract, though quoted in decimals, goes up in .25 increments, so the tick increment is 25. (see also Tick Size)
        */
        public decimal minmov { get; set; } = 1m;
        public bool? fractional { get; set; }
        /**
        * @example Quarters of 1/32: pricescale=128, minmovement=1, minmovement2=4
        */
        public decimal? minmove2 { get; set; }
        /**
        * false if DWM only
        */
        public bool? has_intraday { get; set; } = true;
        /**
        * An array of resolutions which should be enabled in resolutions picker for this symbol.
        */
        public string[] supported_resolutions { get; set; } = new[] { "1H", "4H", "1D", "1W", "1M" };
        /**
        * @example (for ex.: "1,5,60") - only these resolutions will be requested, all others will be built using them if possible
        */
        public string[]? intraday_multipliers { get; set; } = Array.Empty<string>();
        public bool? has_seconds { get; set; } = false;
        public bool? has_ticks { get; set; } = false;
        /**
        * It is an array containing seconds resolutions (in seconds without a postfix) the datafeed builds by itself.
        */
        public string[]? seconds_multipliers { get; set; }
        public bool? has_daily { get; set; } = true;
        public bool? has_weekly_and_monthly { get; set; } = true;
        public bool? has_empty_bars { get; set; }
        public bool? has_no_volume { get; set; } = false;
        /**
        * Integer showing typical volume value decimal places for this symbol
        */
        public decimal? volume_precision { get; set; } = 1;
        public string? data_status { get; set; } = DataStatus.streaming.ToString();

        // Boolean showing whether this symbol is expired futures contract or not.
        public bool? expired { get; set; }

        // Unix timestamp of expiration date.
        public int? expiration_date { get; set; }
        public string? sector { get; set; }
        public string? industry { get; set; }
        public string? currency_code { get; set; }
        public string? original_currency_code { get; set; }
        public string? unit_id { get; set; }
        public string? original_unit_id { get; set; }
        public string[]? unit_conversion_types;
    }

    public enum SeriesFormat
    {
        price,
        volume
    }
    public enum DataStatus
    {
        streaming,
        endofday,
        pulsed,
        delayed_streaming
    }
}
