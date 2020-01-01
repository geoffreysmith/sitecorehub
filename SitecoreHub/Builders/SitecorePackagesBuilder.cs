using SitecoreHub.Repositories;
using SitecoreHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SitecoreHub.Builders
{
    public interface ISitecorePackagesBuilder
    {
        Task<IList<SitecorePackage>> Build();
    }
    public class SitecorePackagesBuilder : ISitecorePackagesBuilder
    {
        ISitecorePackageFileNameRetriever _scPackageFileNameRetriever;
        ISitecorePackageMetadataRetriever _scPackageMetadataRetriever;
        ISitecoreVersionRetriever _scVersionRetriever;

        public SitecorePackagesBuilder(ISitecorePackageFileNameRetriever scPackageFileNameRetriever,
            ISitecorePackageMetadataRetriever scPackageMetadataRetriever,
            ISitecoreVersionRetriever scVersionRetriever)
        {
            _scPackageFileNameRetriever = scPackageFileNameRetriever;
            _scPackageMetadataRetriever = scPackageMetadataRetriever;
            _scVersionRetriever = scVersionRetriever;
        }

        // Creates list with just 9+
        public async Task<IList<SitecorePackage>> Build()
        {
            var sitecorePackage = new List<SitecorePackage>();

            foreach (var scVersion in _scVersionRetriever.GetAll())
            {
                var filesByVersion = await _scPackageFileNameRetriever.Get(scVersion);
                var allPackages = await _scPackageMetadataRetriever.GetAll();

                foreach (var package in filesByVersion)
                {
                    var packageInfo = allPackages.First(x => x.Key == package);

                    sitecorePackage.Add(new SitecorePackage
                    {
                        Description = packageInfo.Value.Description,
                        FileName = package,
                        Url = string.IsNullOrEmpty(packageInfo.Value.Url) ? new Uri("about:blank") : new Uri(packageInfo.Value.Url),
                        Version = scVersion
                    });
                }
            }

            return sitecorePackage;
        }
    }
}
