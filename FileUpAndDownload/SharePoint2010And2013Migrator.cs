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
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Threading;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Abstract class for migrating files from source SharePoint to target SharePont.
    /// </summary>
    public class SharePoint2010And2013Migrator
    {
        private FileMigrationSpecification fileMigrationSpecification;

        private File currentFile;
        private Web currentTargetWeb;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePoint2010And2013Migrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of target SharePoint</param>
        internal SharePoint2010And2013Migrator(FileMigrationSpecification fileMigrationSpecification)
        {
            this.fileMigrationSpecification = fileMigrationSpecification;

            this.SpecifySharePointSpecification();
        }

        public void MigrateFilesOfWeb(Web sourceWeb, Web targetWeb)
        {
            // throws ValidationException if sourceWeb or targetWeb does not exists.
            Validator.ValidateIfWebExists(fileMigrationSpecification.SourceClientContext, sourceWeb);
            Validator.ValidateIfWebExists(fileMigrationSpecification.SourceClientContext, sourceWeb);
            
            this.currentTargetWeb = targetWeb;

            FileCollection files = GetFilesOfSharedDocumentsFolder(this.fileMigrationSpecification.SourceClientContext, sourceWeb);

            ThreadPool.SetMaxThreads(this.fileMigrationSpecification.NumberOfThreads, this.fileMigrationSpecification.NumberOfThreads);
            // List<FileCollection> allFiles = GetAllFilesOfWeb(this.fileMigrationSpecification.SourceClientContext, sourceWeb);
            /*
            foreach (FileCollection files in allFiles)
            {*/
                foreach (File file in files)
                {
                    this.currentFile = file;
                    
                    ThreadPool.QueueUserWorkItem(new WaitCallback(MigrationThreadProcess));
                    
                }    
           // }
        }

        private void MigrationThreadProcess(Object stateInfo)
        {
            new FileMigrator().MigrateFile(this.currentFile, this.fileMigrationSpecification, this.currentTargetWeb);
        }

        private List<FileCollection> GetAllFilesOfWeb(ClientContext clientContext, Web web)
        {
            List<FileCollection> files = new List<FileCollection>();

            Folder rootFolder = web.RootFolder;
            clientContext.Load(rootFolder);
            clientContext.ExecuteQuery();

            files = GetFilesOfAllFolder(clientContext, rootFolder, files);

            return files;
        }

        private List<FileCollection> GetFilesOfAllFolder(ClientContext clientContext, Folder folder, List<FileCollection> files)
        {
            FolderCollection folders = folder.Folders;
            clientContext.Load(folders);
            clientContext.ExecuteQuery();

            if (folders.Count == 0)
            {
                FileCollection currentFiles = folder.Files;
                clientContext.Load(currentFiles);
                clientContext.ExecuteQuery();

                files.Add(currentFiles);
            }
            else
            {
                foreach (Folder f in folders)
                {
                    files = GetFilesOfAllFolder(clientContext, f, files);
                }
            }

            return files;
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

            Folder sharedDocumentsFolder = folders.Single(f => f.Name.Equals(FolderName.SHARED_DOCUMENTS_FOLDERNAME));

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

        private void SpecifySharePointSpecification()
        {
            try
            {
                var endpointAddress = new EndpointAddress(this.fileMigrationSpecification.ServiceAddress);
                FileMigrationService.FileMigrationClient client = new FileMigrationService.FileMigrationClient(new WSHttpBinding(SecurityMode.None), endpointAddress);
                FileMigrationService.IFileMigration fileMigrationService = client.ChannelFactory.CreateChannel();

                Console.WriteLine("Setting the max file size.");
                this.fileMigrationSpecification.MaxFileSize = fileMigrationService.GetMaxFileSize();
            }
            catch (Exception)
            {
                throw new FileMigrationException("Could not connect to the service '" + this.fileMigrationSpecification.ServiceAddress.ToString() + "'.");
            }
        }
    }
}
