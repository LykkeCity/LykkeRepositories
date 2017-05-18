using System;
using System.Text;
using System.Threading.Tasks;
using Lykke.Core;
using Lykke.Core.Azure;

namespace Lykke.AzureRepositories
{

    public class JsonDataRepository : IJsonDataRepository
    {
        private readonly IBlobStorage _blobStorage;

        private readonly string _container;

        private readonly string _historyContainer;

        private readonly string _file;

        public JsonDataRepository(IBlobStorage blobStorage, string container, string historyContainer, string file){
            _blobStorage = blobStorage;
            _container = container;
            _historyContainer = historyContainer;
            _file = file;

        }

        public async Task<string> GetDataAsync()
        {
            return (await GetDataWithMetaAsync()).Item1;
        }

        public async Task<Tuple<string, string>> GetDataWithMetaAsync()
        {
            var result = await _blobStorage.GetAsync(_container, _file);
            return new Tuple<string, string>(result.AsString(), result.ETag);
        }

        public string GetETag()
        {
            return _blobStorage.GetETag(_container, _file);
        }

        public async Task  UpdateBlobAsync(string json, string userName, string ipAddress)
        {
            var data = Encoding.UTF8.GetBytes(json);
            await _blobStorage.SaveBlobAsync(_container, _file, data);
            if (!string.IsNullOrEmpty(_historyContainer))
            {
                await _blobStorage.SaveBlobAsync(_historyContainer, $"{_file}_{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}_{userName}_{ipAddress}",
                    data);
            }
        }
    }


}