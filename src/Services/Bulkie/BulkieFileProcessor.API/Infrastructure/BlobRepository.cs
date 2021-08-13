using BulkieFileProcessor.API.Model;
using Minio;
using System.Net.Mime;
using System.Threading.Tasks;

namespace BulkieFileProcessor.API.Infrastructure
{
    public class BlobRepository : IBlobRepository
    {
        private const string BucketName = "files";

        private readonly MinioClient _client;

        public BlobRepository(MinioClient client)
        {
            _client = client;
        }

        public async Task Add(string name, string filePath)
        {
            await _client.PutObjectAsync(BucketName, name, filePath, contentType: MediaTypeNames.Application.Zip);
        }

        public async Task<bool> Exists(string name)
        {
            try
            {
                await _client.StatObjectAsync(BucketName, name);
                return true;
            }
            catch (Minio.Exceptions.ObjectNotFoundException)
            {
                return false;
            }
        }
    }
}