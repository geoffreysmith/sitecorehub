using System;

namespace SitecoreHub.Models
{
    public class SitecorePackage
    {
        public string Version { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public Uri Url { get; set; }
    }
}
