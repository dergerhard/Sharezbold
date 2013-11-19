//-----------------------------------------------------------------------
// <copyright file="FileMigrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using System;
    using System.Threading;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Class to migrate files.
    /// </summary>
    internal class FileMigrator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileMigrator"/> class.
        /// </summary>
        internal FileMigrator()
        {
        }

        /// <summary>
        /// Migrates the file.
        /// </summary>
        /// <param name="file">file to migrate</param>
        /// <param name="specification">specifications for the migration</param>
        /// <param name="targetWeb">the target web</param>
        internal void MigrateFile(File file, FileMigrationSpecification specification, Web targetWeb)
        {
            SharePoint2010And2013Downloader downloader = new SharePoint2010And2013Downloader(specification);
            SharePoint2010And2013Uploader uploader = new SharePoint2010And2013Uploader(specification.TargetClientContext);

            try
            {                
                MigrationFile migrationFile = downloader.DownloadDocument(file);
                specification.Logger.AddMessage(string.Format("downloaded file '{0}'.", migrationFile.File.Name));
                Thread.Sleep(this.GetWaitingTime(specification.Bandwith));
                uploader.UploadDocument(migrationFile, targetWeb);
                specification.Logger.AddMessage(string.Format("uploaded file '{0}'.", migrationFile.File.Name));
            }
            catch (OperationCanceledException e)
            {
                specification.Logger.AddMessage(string.Format("Exception: {0}", e.Message));
            }
        }

        /// <summary>
        /// Calculates the waiting time for reduce bandwith.
        /// </summary>
        /// <param name="bandwith">bandwith to calculate waiting time</param>
        /// <returns>calculated waiting time</returns>
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
                return baseValue - ((100 - bandwith) * 10);
            }
        }
    }
}
