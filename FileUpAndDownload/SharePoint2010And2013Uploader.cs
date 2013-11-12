//-----------------------------------------------------------------------
// <copyright file="SharePoint2010And2013Uploader.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using System.Linq;
    using Microsoft.SharePoint.Client;
    
    /// <summary>
    /// Uploader of files to SharePoint 2010 or 2013.
    /// </summary>
    internal class SharePoint2010And2013Uploader
    {

        private static string SHARED_DOCUMENTS_FOLDERNAME = "Shared Documents";

        /// <summary>
        /// ClientContext of the server.
        /// </summary>
        private ClientContext targetClientContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePoint2010And2013Uploader"/> class.
        /// </summary>
        /// <param name="clientContext">ClientContext of the target SharePoint.</param>
        public SharePoint2010And2013Uploader(ClientContext clientContext)
        {
            this.targetClientContext = clientContext;
        }

        internal void UploadDocument(MigrationFile migrationFile,  Web targetWeb)
        {
            Folder rootFolder = targetWeb.RootFolder;
            this.targetClientContext.Load(rootFolder);
            this.targetClientContext.ExecuteQuery();

            FileCreationInformation newFile = new FileCreationInformation();
            newFile.ContentStream = migrationFile.DownloadedStream;
            newFile.Overwrite = true;
            newFile.Url = GetSharedDocumentsUrl(rootFolder) + migrationFile.File.Name;

            File file;
            Folder folder = GetSharedDocumentsFolder(rootFolder);

            file = folder.Files.Add(newFile);
            this.targetClientContext.ExecuteQuery();
        }

        private string GetSharedDocumentsUrl(Folder rootFolder)
        {
            string url = GetSharedDocumentsFolder(rootFolder).ServerRelativeUrl;
            if (!url.EndsWith("/"))
            {
                url += "/";
            }

            return url;
        }

        private Folder GetSharedDocumentsFolder(Folder folder)
        {
            FolderCollection folders = folder.Folders;
            targetClientContext.Load(folders);
            targetClientContext.ExecuteQuery();

            Folder sharedDocumentsFolder = folders.Single(f => f.Name.Equals(SHARED_DOCUMENTS_FOLDERNAME));

            if (sharedDocumentsFolder == null)
            {
                throw new FileMigrationException("Shared Documents folder not found!");
            }
            else
            {
                return sharedDocumentsFolder;
            }
        }
    }
}
