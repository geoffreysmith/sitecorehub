using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SitecoreHub.Dtos
{
    // from sitecore-packages
    public class PackageMetadata
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class SitecorePackageResponse
    {
        [JsonPropertyName("sources")]
        public IList<string> FileNames { get; set; }
    }
}
