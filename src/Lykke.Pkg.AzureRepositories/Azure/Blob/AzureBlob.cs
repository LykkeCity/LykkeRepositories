using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Lykke.Core.Azure.Blob
{

    public class AzureBlobStorage : IBlobStorage
    {
        private readonly CloudBlobClient _blobClient;

        public AzureBlobStorage(string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task SaveBlobAsync(string container, string key, Stream bloblStream)
        {
            var containerRef = _blobClient.GetContainerReference(container);
            await containerRef.CreateIfNotExistsAsync();

            var blockBlob = containerRef.GetBlockBlobReference(key);

            bloblStream.Position = 0;
            await blockBlob.UploadFromStreamAsync(bloblStream);
        }

        public async Task SaveBlobAsync(string container, string key, byte[] blob)
        {
            var containerRef = _blobClient.GetContainerReference(container);
            await containerRef.CreateIfNotExistsAsync();

            var blockBlob = containerRef.GetBlockBlobReference(key);
          
            await blockBlob.UploadFromByteArrayAsync(blob, 0, blob.Length);
        }


        public async Task<bool> HasBlobAsync(string container, string key)
        {
            var containerRef = _blobClient.GetContainerReference(container);
            return await containerRef.ExistsAsync();
        }

        public async Task<DateTime> GetBlobsLastModifiedAsync(string container)
        {
            BlobContinuationToken continuationToken = null;
            var results = new List<IListBlobItem>();
            var containerRef = _blobClient.GetContainerReference(container);

            do
            {
                var response = await containerRef.ListBlobsSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                foreach (var listBlobItem in response.Results)
                {
                    if (listBlobItem is CloudBlob)
                        results.Add(listBlobItem);
                }
            } while (continuationToken != null);

            var dateTimeOffset = results.Where(x => x is CloudBlob).Max(x => ((CloudBlob) x).Properties.LastModified);

            return dateTimeOffset.GetValueOrDefault().UtcDateTime;
        }

        public async Task<AzureBlobResult> GetAsync(string blobContainer, string key)
        {
            var containerRef = _blobClient.GetContainerReference(blobContainer);
            var blockBlob = containerRef.GetBlockBlobReference(key);
            var memStream = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(memStream);
            return new AzureBlobResult(memStream, blockBlob.Properties.ETag);
        }

        public string GetETag(string blobContainer, string key)
        {
            var containerRef = _blobClient.GetContainerReference(blobContainer);
            var blockBlob = containerRef.GetBlockBlobReference(key);
            return blockBlob.Properties.ETag;
        }

        public string GetBlobUrl(string container, string key)
        {
            var containerRef = _blobClient.GetContainerReference(container);
            var blockBlob = containerRef.GetBlockBlobReference(key);

            return blockBlob.Uri.AbsoluteUri;
        }

        public async Task<IEnumerable<string>> FindNamesByPrefixAsync(string container, string prefix)
        {
            BlobContinuationToken continuationToken = null;
            var results = new List<string>();
            var containerRef = _blobClient.GetContainerReference(container);

            do
            {
                var response = await containerRef.ListBlobsSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                foreach (var listBlobItem in response.Results)
                {
                    if (listBlobItem.Uri.ToString().StartsWith(prefix))
                        results.Add(listBlobItem.Uri.ToString());
                }
            } while (continuationToken != null);

            return results;
        }

        public async Task<IEnumerable<string>> GetListOfBlobsAsync(string container)
        {
            var containerRef = _blobClient.GetContainerReference(container);

            BlobContinuationToken token = null;
            var results = new List<string>();
            do
            {
                var result = await containerRef.ListBlobsSegmentedAsync(token);
                token = result.ContinuationToken;
                foreach (var listBlobItem in result.Results)
                {
                    results.Add(listBlobItem.Uri.ToString());
                }

                //Now do something with the blobs
            } while (token != null);

            return results;
        }

        public Task DelBlobAsync(string blobContainer, string key)
        {
            var containerRef = _blobClient.GetContainerReference(blobContainer);

            var blockBlob = containerRef.GetBlockBlobReference(key);
            return blockBlob.DeleteAsync();
        }
    }
}