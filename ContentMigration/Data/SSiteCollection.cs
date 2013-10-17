

namespace Sharezbold.ContentMigration.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
using System.Xml;

    /// <summary>
    /// Information for a Sharepoint site collection
    /// </summary>
    public class SSiteCollection
    {
        /// <summary>
        /// Represents all sub sites
        /// </summary>
        public List<KeyValuePair<SSite, bool>> Sites { get; set; }

        /// <summary>
        /// Represents the site that is the site collection
        /// </summary>
        public SSite SiteCollectionSite { get; set;}

        /// <summary>
        /// Default constructor
        /// </summary>
        public SSiteCollection()
        {
            this.Sites = new List<KeyValuePair<SSite, bool>>();
            this.SiteCollectionSite = null;
        }
    }
}
