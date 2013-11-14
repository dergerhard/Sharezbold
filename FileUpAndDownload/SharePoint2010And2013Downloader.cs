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
        private FileMigrationSpecification specification;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePoint2010And2013Downloader"/> class.
        /// </summary>
        /// <param name="sourceClientContext">clientContext of the source SharePoint</param>
        public SharePoint2010And2013Downloader(FileMigrationSpecification specification)
        {
            this.specification = specification;
        }

        internal MigrationFile DownloadDocument(File file)
        {
            ClientContext sourceClientContext = this.specification.SourceClientContext;
            Console.WriteLine("Downlaod file '{0}' now.", file.Name);
            FileInformation fileInformation = File.OpenBinaryDirect(sourceClientContext, file.ServerRelativeUrl);
            sourceClientContext.ExecuteQuery();

            byte[] content = ReadFully(fileInformation.Stream);

            if (content.Length > this.specification.MaxFileSize)
            {
                throw new OperationCanceledException("Filesize of file '" + file.Name + "' is too big for upload.");
            }

            MigrationFile migrationFile = new MigrationFile();
            migrationFile.File = file;
            migrationFile.Content = content;

            return migrationFile;
        }

        private byte[] ReadFully(System.IO.Stream input)
        {

            byte[] buffer = new byte[16 * 1024];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
