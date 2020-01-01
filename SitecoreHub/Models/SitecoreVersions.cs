using System.Collections.Generic;

namespace SitecoreHub.Models
{
    public class SitecoreVersions
    {
        public string Version { get; set; }
        public IList<string> Packages { get; set; }
    }
}
