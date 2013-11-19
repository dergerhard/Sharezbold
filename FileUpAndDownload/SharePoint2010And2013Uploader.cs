//-----------------------------------------------------------------------
// <copyright file="SharePoint2010And2013Uploader.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using System;
    using System.Linq;
    using Microsoft.SharePoint.Client;
    
    /// <summary>
    /// Uploader of files to SharePoint 2010 or 2013.
    /// </summary>
    internal class SharePoint2010And2013Uploader
    {
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

        /// <summary>
        /// Uploads the document.
        /// </summary>
        /// <param name="migrationFile">the migration file</param>
        /// <param name="targetWeb">the target web</param>
        internal void UploadDocument(MigrationFile migrationFile,  Web targetWeb)
        {
            Console.WriteLine("Upload file '{0}' now.", migrationFile.File.Name);

            Folder rootFolder = targetWeb.RootFolder;
            this.targetClientContext.Load(rootFolder);
            this.targetClientContext.ExecuteQuery();

            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = migrationFile.Content;
            newFile.Overwrite = true;
            newFile.Url = this.GetSharedDocumentsUrl(rootFolder) + migrationFile.File.Name;

            File file;
            Folder folder = this.GetSharedDocumentsFolder(rootFolder);

            file = folder.Files.Add(newFile);
            this.targetClientContext.ExecuteQuery();
        }

        /// <summary>
        /// Gets the shared Documents url.
        /// </summary>
        /// <param name="rootFolder">the root folder</param>
        /// <returns>url of shared documents</returns>
        private string GetSharedDocumentsUrl(Folder rootFolder)
        {
            string url = this.GetSharedDocumentsFolder(rootFolder).ServerRelativeUrl;
            if (!url.EndsWith("/"))
            {
                url += "/";
            }

            return url;
        }

        /// <summary>
        /// Gets the shared documents folder.
        /// </summary>
        /// <param name="folder">the root folder</param>
        /// <returns>the shared documents folder</returns>
        private Folder GetSharedDocumentsFolder(Folder folder)
        {
            FolderCollection folders = folder.Folders;
            this.targetClientContext.Load(folders);
            this.targetClientContext.ExecuteQuery();

            FolderName folderName = new FolderName();
            Folder sharedDocumentsFolder = folders.Single(f => f.Name.Equals(folderName.SharedDocumentsFoldername));

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
