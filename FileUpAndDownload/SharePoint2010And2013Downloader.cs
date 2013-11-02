//-----------------------------------------------------------------------
// <copyright file="SharePoint2010And2013Downloader.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using System;
    using System.IO;
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

        /// <summary>
        /// Downloads the document from the SharePoint.
        /// </summary>
        /// <param name="relativeUrl">relative url of file</param>
        /// <returns>Stream of file</returns>
        internal Stream DownloadDocument(string relativeUrl)
        {
            Stream stream = null;
            if (relativeUrl != null)
            {
                FileInformation fileInformation = Microsoft.SharePoint.Client.File.OpenBinaryDirect(this.sourceClientContext, relativeUrl);

                stream = fileInformation.Stream;
            }
            else
            {
                Console.WriteLine("no file-reference was given.");
            }

            return stream;
        }
    }
}
