using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using SitecoreHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SitecoreHub.Repositories
{
    public interface IBlobContainerRetriever
    {
        Task<IList<BlobStorageFile>> GetAll();
    }
    public class BlobContainerRetriever : IBlobContainerRetriever
    {
        private readonly IAzureClientFactory<BlobServiceClient> _blobClientFactory;
        public BlobContainerRetriever(IAzureClientFactory<BlobServiceClient> blobClientFactory)
        {
            _blobClientFactory = blobClientFactory;
        }

        public async Task<IList<BlobStorageFile>> GetAll()
        {
            var fileNames = new List<BlobStorageFile>();
            var client = _blobClientFactory.CreateClient("Default");
            var container = client.GetBlobContainerClient("sitecore");

            await foreach (var blob in container.GetBlobsAsync(Azure.Storage.Blobs.Models.BlobTraits.All))
            {
                fileNames.Add(new BlobStorageFile
                {
                    FileName = blob.Name,
                    LastModified = blob.Properties.LastModified.Value.DateTime,
                    Size = blob.Properties.ContentLength.Value / (1024 * 1024)
                });
            }

            return fileNames;
        }
    }
}
