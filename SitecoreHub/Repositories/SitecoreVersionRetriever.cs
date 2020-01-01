using System.Collections.Generic;

namespace SitecoreHub.Repositories
{
    public interface ISitecoreVersionRetriever
    {
        IList<string> GetAll();
    }
    // Right now I am counting on (hoping) for a better way to get
    // v9+ versions, otherwise this will be moved to a config
    public class SitecoreVersionRetriever : ISitecoreVersionRetriever
    {
        private readonly string[] Versions = { "9.0.2", "9.1.1", "9.2.0", "9.3.0" };
        public IList<string> GetAll()
        {
            return Versions;
        }
    }
}
