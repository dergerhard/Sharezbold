

namespace Sharezbold
{
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;
    using FileMigration;

    internal class MigrationData
    {
        internal SharePoint2010And2013Migrator FileMigrator { get; set; }
        internal HashSet<string> WebUrlsToMigrate { get; set; }
        internal ClientContext SourceClientContext { get; set; }
        internal ClientContext TargetClientContext { get; set; }
    }
}
