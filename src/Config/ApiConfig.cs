// Copyright (c) EverRise Pte Ltd. All rights reserved.

using System.Text.Json;

namespace EverStats.Config;
public class ApiConfig
{
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    private static readonly string _configurationFileName = $"{nameof(ApiConfig).ToLower()}.json";

#nullable disable
    public bool StoreInDb { get; set; }
    public bool SendTweets { get; set; }
    public AzureConfiguration AzureConfiguration { get; set; }
    public TwitterConfiguration TwitterConfiguration { get; set; }
    public MoralisConfiguration MoralisConfiguration { get; set; }

    public static ApiConfig GetConfiguration() =>
       JsonSerializer.Deserialize<ApiConfig>(File.ReadAllText(_configurationFileName), _options);

}