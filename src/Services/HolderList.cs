using Azure.Storage.Blobs;
using Sylvan.Data.Csv;
using System.Reflection;
using EverStats.Config;
using EverStats.Data;

namespace EverStats.Services
{
    public class HolderList
    {
        private readonly ApiConfig _config;
        private readonly ILogger<BlockchainQuery> _logger;
        private readonly string[] _chains =
        {
            "bsc",
            "eth",
            "poly",
            "ftm",
            "avx",
        };

        private string[] _addresses;
        private object _lock = new object();

        private Task _loadHolderListTask;
        private DateTime _lastRefresh;

        public HolderList(ApiConfig config, ILogger<BlockchainQuery> logger)
        {
            _config = config;
            _logger = logger;
        }

        private Task GetOrCreateHolderList()
        {
            lock (_lock)
            {
                var task = _loadHolderListTask;
                if (task is null)
                {
                    _loadHolderListTask = task = LoadHolderList();
                }

                return task;
            }
        }

        public async Task<string[]> GetHolderList()
        {
            await GetOrCreateHolderList();

            if ((DateTime.UtcNow - _lastRefresh).TotalHours > 1)
            {
                _loadHolderListTask = LoadHolderList();
            }

            return _addresses;
        }

        private Task DownloadHolderLists()
        {
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(_config.AzureConfiguration.StorageConnection);

            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("holder-data");

            List<Task> downloads = new();
            foreach (var chain in _chains)
            {
                var fileName = $"{chain}-tokenholders-for-contract-0xc17c30e98541188614df99239cabd40280810ca3.csv";
                var destination = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);

                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                downloads.Add(blobClient.DownloadToAsync(destination));
            }

            return Task.WhenAll(downloads);
        }

        private async Task LoadHolderList()
        {
            await DownloadHolderLists();

            var addressesSet = new HashSet<string>();
            foreach (var chain in _chains)
            {
                var fileName = $"{chain}-tokenholders-for-contract-0xc17c30e98541188614df99239cabd40280810ca3.csv";
                var file = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);
                using var csv = CsvDataReader.Create(file);

                while (await csv.ReadAsync())
                {
                    var id = csv.GetString(0);
                    addressesSet.Add(id);
                }
            }

            _addresses = addressesSet.ToArray();

            _lastRefresh = DateTime.UtcNow;
        }
    }
}
