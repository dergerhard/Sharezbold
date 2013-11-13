//-----------------------------------------------------------------------
// <copyright file="SharePoint2010And2013Downloader.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using System;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Downloader for files of SharePoint 2010 or 2013.
    /// </summary>
    internal class SharePoint2010And2013Downloader
    {
        /// <summary>
        /// The ClientContext of the source SharePoint.
        /// </summary>
        private ClientContext sourceClientContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePoint2010And2013Downloader"/> class.
        /// </summary>
        /// <param name="sourceClientContext">clientContext of the source SharePoint</param>
        public SharePoint2010And2013Downloader(ClientContext sourceClientContext)
        {
            this.sourceClientContext = sourceClientContext;
        }

        internal MigrationFile DownloadDocument(File file)
        {
            Console.WriteLine("Downlaod file '{0}' now.", file.Name);
            FileInformation fileInformation = File.OpenBinaryDirect(this.sourceClientContext, file.ServerRelativeUrl);

            this.sourceClientContext.ExecuteQuery();

            System.IO.Stream streamResult = fileInformation.Stream;

            MigrationFile migrationFile = new MigrationFile();
            migrationFile.File = file;
            migrationFile.DownloadedStream = streamResult;

            return migrationFile;
        }
    }
}
