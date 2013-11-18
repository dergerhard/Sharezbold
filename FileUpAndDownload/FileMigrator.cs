

namespace Sharezbold.FileMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    internal class FileMigrator
    {

        internal void MigrateFile(File file, FileMigrationSpecification specification, Web targetWeb)
        {
            SharePoint2010And2013Downloader downloader = new SharePoint2010And2013Downloader(specification);
            SharePoint2010And2013Uploader uploader = new SharePoint2010And2013Uploader(specification.TargetClientContext);

            try
            {                
                MigrationFile migrationFile = downloader.DownloadDocument(file);
                Thread.Sleep(GetWaitingTime(specification.Bandwith));
                uploader.UploadDocument(migrationFile, targetWeb);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
        }

        internal int GetWaitingTime(int bandwith)
        {
            if (bandwith == 0)
            {
                bandwith = 1;
            }

            if (bandwith == 100)
            {
                return 0;
            }
            else
            {
                int baseValue = 1000;
                return baseValue - ((100 - bandwith * 10));
            }
        }
    }
}
