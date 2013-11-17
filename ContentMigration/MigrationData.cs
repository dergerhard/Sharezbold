

namespace Sharezbold.ContentMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;
    using FileMigration;

    public class MigrationData
    {
        public SharePoint2010And2013Migrator FileMigrator { get; set; }
        public HashSet<string> WebUrlsToMigrate { get; set; }
        public ClientContext SourceClientContext { get; set; }
        public ClientContext TargetClientContext { get; set; }
    }
}
