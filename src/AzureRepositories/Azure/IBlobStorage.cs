using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Core.Azure
{

    public struct AzureBlobResult
    {

        private readonly MemoryStream _stream;

        public string ETag { get; private set; }

        public AzureBlobResult(MemoryStream stream, string eTag)
        {
            _stream = stream;
            _stream.Position = 0;
            ETag = eTag;
        }

        public Stream AsStream()
        {

            return _stream;
        }

        public byte[] AsBytes()
        {
            return _stream.ToBytes();
        }

        public string AsString(Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            return encoding.GetString(AsBytes());
        }


    }
    public interface IBlobStorage
    {
        /// <summary>
        ///     Сохранить двоичный поток в контейнер
        /// </summary>
        /// <param name="container">Имя контейнера</param>
        /// <param name="key">Ключ</param>
        /// <param name="bloblStream">Поток</param>
        Task SaveBlobAsync(string container, string key, Stream bloblStream);

        Task SaveBlobAsync(string container, string key, byte[] blob);

        Task<bool> HasBlobAsync(string container, string key);

        /// <summary>
        ///     Returns datetime of latest modification among all blobs
        /// </summary>
        Task<DateTime> GetBlobsLastModifiedAsync(string container);

        Task<AzureBlobResult> GetAsync(string blobContainer, string key);

        string GetETag(string blobContainer, string key);

        string GetBlobUrl(string container, string key);

        Task<IEnumerable<string>> FindNamesByPrefixAsync(string container, string prefix);


        Task<IEnumerable<string>> GetListOfBlobsAsync(string container);

        Task DelBlobAsync(string blobContainer, string key);
    }
}