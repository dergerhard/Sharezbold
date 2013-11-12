//-----------------------------------------------------------------------
// <copyright file="SharePoint2010And2013Migrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Abstract class for migrating files from source SharePoint to target SharePont.
    /// </summary>
    public class SharePoint2010And2013Migrator
    {
        /// <summary>
        /// Name of shared documents folder.
        /// </summary>
        private static string SHARED_DOCUMENTS_FOLDERNAME = "Shared Documents";

        /// <summary>
        /// ClientContext of the source SharePoint.
        /// </summary>
        private ClientContext sourceClientContext;

        /// <summary>
        /// ClientContext of the target SharePoint.
        /// </summary>
        private ClientContext targetClientContext;

        /// <summary>
        /// List of found files.
        /// </summary>
        private List<ListItem> listItemList;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePoint2010And2013Migrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of target SharePoint</param>
        public SharePoint2010And2013Migrator(ClientContext sourceClientContext, ClientContext targetClientContext)
        {
            this.sourceClientContext = sourceClientContext;
            this.targetClientContext = targetClientContext;
        }

        public void MigrateFilesOfWeb(Web web)
        {
            GetFilesOfSharedDocumentsFolder(this.sourceClientContext, web);
            // TODO download files...
            // TODO upload files...
        }

        private FileCollection GetFilesOfSharedDocumentsFolder(ClientContext clientContext, Web web)
        {
            Folder sharedDocumentFolder = GetSharedDocumentsFolder(clientContext, web);
            return GetFiles(clientContext, sharedDocumentFolder);
        }

        private Folder GetSharedDocumentsFolder(ClientContext clientContext, Web web)
        {
            Folder rootFolder = web.RootFolder;
            clientContext.Load(rootFolder);
            clientContext.ExecuteQuery();

            FolderCollection folders = rootFolder.Folders;
            clientContext.Load(folders);
            clientContext.ExecuteQuery();

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

        private FileCollection GetFiles(ClientContext clientContext, Folder folder)
        {
            FileCollection files = folder.Files;
            clientContext.Load(files);
            clientContext.ExecuteQuery();
            
            // TODO where files ends with...

            return files;
        }
    }
}
