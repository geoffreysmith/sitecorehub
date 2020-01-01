using SitecoreHub.Dtos;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SitecoreHub.Repositories
{
    public interface ISitecorePackageMetadataRetriever
    {
        Task<IDictionary<string, PackageMetadata>> GetAll();
    }

    public class SitecorePackageMetadataRetriever : ISitecorePackageMetadataRetriever
    {
        private readonly IHttpClientFactory _clientFactory;
        public SitecorePackageMetadataRetriever(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // Most authoritative list of Sitecore package URLs publically available.
        public async Task<IDictionary<string, PackageMetadata>> GetAll()
        {
            var url = "https://raw.githubusercontent.com/Sitecore/docker-images/master/sitecore-packages.json";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<IDictionary<string, PackageMetadata>>(responseStream);
            }
            else
            {
                throw new HttpRequestException($"Unable to retrieve sitecore-packages.json from {url}");
            }
        }
    }
}
