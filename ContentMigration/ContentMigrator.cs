//-----------------------------------------------------------------------
// <copyright file="ContentMigrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ContentMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    public class ContentMigrator
    {
        /// <summary>
        /// Responsible for downloading data from Sharepoint
        /// </summary>
        private ContentDownloader downloader;

        /// <summary>
        /// Responsible for uploading data to Sharepoint
        /// </summary>
        private ContentUploader uploader;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagementMigration"/> class.
        /// </summary>
        /// <param name="sourceSharePoint">The ClientContext of the source SharePoint.</param>
        /// <param name="targetSharePoint">The ClientContext of the target SharePoint.</param>
        public ContentMigrator(ClientContext sourceSharePoint, ClientContext targetSharePoint)
        {
            this.downloader = new ContentDownloader(sourceSharePoint);
            this.uploader = new ContentUploader(targetSharePoint);
        }

        /// <summary>
        /// Generates a tree object for content selection
        /// </summary>
        /// <returns>Sharepoint tree node</returns>
        public SpTreeNode GenerateMigrationTree()
        {
            return downloader.GenerateMigrationTree();
        }

    }
}
