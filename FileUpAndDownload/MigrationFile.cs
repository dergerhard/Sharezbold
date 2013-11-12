

namespace Sharezbold.FileMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    internal class MigrationFile
    {
        internal File File { get; set; }
        internal System.IO.Stream DownloadedStream { get; set; }
    }
}
