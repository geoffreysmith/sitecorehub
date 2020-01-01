using SitecoreHub.Dtos;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SitecoreHub.Repositories
{
    public interface ISitecorePackageFileNameRetriever
    {
        Task<IList<string>> Get(string scVersion);
    }
    public class SitecorePackageFileNameRetriever : ISitecorePackageFileNameRetriever
    {
        private readonly IHttpClientFactory _clientFactory;

        public SitecorePackageFileNameRetriever(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;

        }

        // Retrieves filenamess per version
        public async Task<IList<string>> Get(string scVersion)
        {
            var assetUrl = $"https://raw.githubusercontent.com/Sitecore/docker-images/master/windows/{scVersion}/sitecore-assets/build.json";

            var request = new HttpRequestMessage(HttpMethod.Get, assetUrl);

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var assets = await JsonSerializer.DeserializeAsync<SitecorePackageResponse>(responseStream);

                return assets.FileNames;
            }
            else
            {
                throw new HttpRequestException($"Unable to retrieve {assetUrl}");
            }
        }
    }
}
