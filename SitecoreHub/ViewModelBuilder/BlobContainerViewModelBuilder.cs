using SitecoreHub.Models;
using SitecoreHub.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SitecoreHub.ViewModelBuilder
{
    public interface IBlobContainerViewModelBuilder
    {
        Task<IList<BlobStorageFile>> GetAll();
    }

    public class BlobContainerViewModelBuilder : IBlobContainerViewModelBuilder
    {
        IBlobContainerRetriever _blobContainerRetriever;
        public BlobContainerViewModelBuilder(IBlobContainerRetriever blobContainerRetriever)
        {
            _blobContainerRetriever = blobContainerRetriever;
        }

        public async Task<IList<BlobStorageFile>> GetAll()
        {
            var blobs = await _blobContainerRetriever.GetAll();

            return blobs;
        }
    }
}
