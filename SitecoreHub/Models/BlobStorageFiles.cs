using System;

namespace SitecoreHub.Models
{
    public class BlobStorageFile
    {
        public string FileName { get; set; }
        public DateTime LastModified { get; set; }
        public long Size { get; set; }
    }
}