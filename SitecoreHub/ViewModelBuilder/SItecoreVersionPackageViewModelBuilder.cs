using SitecoreHub.Builders;
using SitecoreHub.Models;
using SitecoreHub.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SitecoreHub.ViewModelBuilder
{
    public interface ISitecoreVersionPackageViewModelBuilder
    {
        Task<IList<SitecorePackageViewModel>> GetAll();
        IList<string> GetSitecoreVersions();
    }

    public class SitecoreVersionPackageViewModelBuilder : ISitecoreVersionPackageViewModelBuilder
    {
        ISitecorePackagesBuilder _scPackageBuilder;
        ISitecoreVersionRetriever _scVersionRetriever;
        IBlobContainerRetriever _blobContainerRetriever;
        public SitecoreVersionPackageViewModelBuilder(ISitecorePackagesBuilder scPackageBuilder, ISitecoreVersionRetriever scVersionRetriever, IBlobContainerRetriever blobContainerRetriever)
        {
            _scPackageBuilder = scPackageBuilder;
            _scVersionRetriever = scVersionRetriever;
            _blobContainerRetriever = blobContainerRetriever;
        }

        public IList<string> GetSitecoreVersions() => _scVersionRetriever.GetAll();

        public async Task<IList<SitecorePackageViewModel>> GetAll()
        {
            var packageViewModels = new List<SitecorePackageViewModel>();
            var packages = await _scPackageBuilder.Build();
            var blobPackages = await _blobContainerRetriever.GetAll();

            foreach (var package in packages)
            {
                var packageViewModel = new SitecorePackageViewModel()
                {
                    Description = package.Description,
                    FileName = package.FileName,
                    Url = package.Url,
                    Version = package.Version,
                };

                if (blobPackages.Any(x => x.FileName == packageViewModel.FileName))
                {
                    packageViewModel.Installed = true;
                }

                packageViewModels.Add(packageViewModel);
            }

            return packageViewModels;
        }
    }
}
