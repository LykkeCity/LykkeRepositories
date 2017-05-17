using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Core.Azure.Blob
{
	internal static class BlobInMemoryHelper
	{
		public static void AddOrReplace(this Dictionary<string, MemoryStream> blob, string key, MemoryStream data)
		{
			if (blob.ContainsKey(key))
			{
				blob[key] = data;
				return;
			}


			blob.Add(key, data);
		}

		public static MemoryStream GetOrNull(this Dictionary<string, MemoryStream> blob, string key)
		{
			if (blob.ContainsKey(key))
				return blob[key];


			return null;
		}
	}


	public class AzureBlobInMemory : IBlobStorage
	{
		private readonly Dictionary<string, Dictionary<string, MemoryStream>> _blobs =
			new Dictionary<string, Dictionary<string, MemoryStream>>();


		private readonly object _lockObject = new object();


		private Dictionary<string, MemoryStream> GetBlob(string container)
		{
			if (!_blobs.ContainsKey(container))
				_blobs.Add(container, new Dictionary<string, MemoryStream>());


			return _blobs[container];
		}

		public async Task SaveBlob(string container, string key, Stream bloblStream)
		{
		    await Task.Run(() =>
		    {
		        lock (_lockObject)
		            GetBlob(container).AddOrReplace(key, new MemoryStream(bloblStream.ToBytes()));
		    });
		    
		}

		public async Task SaveBlobAsync(string container, string key, Stream bloblStream)
		{
			await SaveBlob(container, key, bloblStream);
		}

		public async Task SaveBlobAsync(string container, string key, byte[] blob)
		{
            await Task.Run(() =>
            {
                lock (_lockObject)
                    GetBlob(container).AddOrReplace(key, new MemoryStream(blob));
            });
		}

        public async Task<bool> HasBlobAsync(string container, string key)
		{
				return await Task.Run(() =>
				{
				    lock (_lockObject)
				        return _blobs[container].ContainsKey(key);
				});
		}

		public async Task<DateTime> GetBlobsLastModifiedAsync(string container)
		{
		    return await Task.FromResult(DateTime.UtcNow);
		}

		public MemoryStream this[string container, string key]
		{
			get
			{
				lock (_lockObject)
					return GetBlob(container).GetOrNull(key);
			}
		}

		public async Task<AzureBlobResult> GetAsync(string container, string key)
		{
			var result = this[container, key];
			return await Task.FromResult(new AzureBlobResult(result, string.Empty));
		}

		public async Task<string> GetAsTextAsync(string blobContainer, string key)
		{
			var result = this[blobContainer, key];
			using (var sr = new StreamReader(result))
			{
				return await Task.FromResult(sr.ReadToEnd());
			}
		}

		public string GetBlobUrl(string container, string key)
		{
			return string.Empty;
		}

		public async Task<IEnumerable<string>> FindNamesByPrefixAsync(string container, string prefix)
		{
				return await Task.Run(() =>
				{
				    lock (_lockObject)
				        return GetBlob(container).Where(itm => itm.Key.StartsWith(prefix)).Select(itm => itm.Key);
				});
		}

		public async Task<IEnumerable<string>> GetListOfBlobsAsync(string container)
		{
			
				return await Task.Run(() =>
				{
				    lock (_lockObject)
				        return GetBlob(container).Select(itm => itm.Key);
				});
		}

		public void DelBlob(string container, string key)
		{
			lock (_lockObject)
				GetBlob(container).Remove(key);
		}

		public async Task DelBlobAsync(string blobContainer, string key)
		{
			
			await Task.Run(()=> DelBlob(blobContainer, key));
		}

	    public string GetETag(string blobContainer, string key)
	    {
	        return string.Empty;
	    }
	}
}
