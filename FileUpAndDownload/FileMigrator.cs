

namespace Sharezbold.FileMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    internal class FileMigrator
    {

        internal void MigrateFile(File file, ClientContext sourceClientContext, ClientContext targetClientContext, Web targetWeb)
        {
            SharePoint2010And2013Downloader downloader = new SharePoint2010And2013Downloader(sourceClientContext);
            SharePoint2010And2013Uploader uploader = new SharePoint2010And2013Uploader(targetClientContext);

            MigrationFile migrationFile = downloader.DownloadDocument(file);
            uploader.UploadDocument(migrationFile, targetWeb);
        }
    }
}
