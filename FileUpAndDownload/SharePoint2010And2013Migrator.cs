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
        /// <summary>
        /// The FileMigrationSpecification.
        /// </summary>
        private FileMigrationSpecification fileMigrationSpecification;

        /// <summary>
        /// The current file.
        /// </summary>
        private File currentFile;

        /// <summary>
        /// The current target web.
        /// </summary>
        private Web currentTargetWeb;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePoint2010And2013Migrator"/> class.
        /// </summary>
        /// <param name="fileMigrationSpecification">instance of the FileMigrationSpecification</param>
        internal SharePoint2010And2013Migrator(FileMigrationSpecification fileMigrationSpecification)
        {
            this.fileMigrationSpecification = fileMigrationSpecification;

            this.SpecifySharePointSpecification();
        }

        /// <summary>
        /// Migrates the files of a web in a shared documents folder.
        /// </summary>
        /// <param name="sourceWeb">the source web</param>
        /// <param name="targetWeb">the target web</param>
        public void MigrateFilesOfWeb(Web sourceWeb, Web targetWeb)
        {
            //// throws ValidationException if sourceWeb or targetWeb does not exists.
            Validator.ValidateIfWebExists(this.fileMigrationSpecification.SourceClientContext, sourceWeb);
            Validator.ValidateIfWebExists(this.fileMigrationSpecification.SourceClientContext, sourceWeb);

            this.currentTargetWeb = targetWeb;

            FileCollection files = this.GetFilesOfSharedDocumentsFolder(this.fileMigrationSpecification.SourceClientContext, sourceWeb);

            ThreadPool.SetMaxThreads(this.fileMigrationSpecification.NumberOfThreads, this.fileMigrationSpecification.NumberOfThreads);

            foreach (File file in files)
            {
                this.currentFile = file;

                ThreadPool.QueueUserWorkItem(new WaitCallback(this.MigrationThreadProcess));
            }
        }

        /// <summary>
        /// Start the migration thread process for threadpool.
        /// </summary>
        /// <param name="stateInfo">info of the state of the thread</param>
        private void MigrationThreadProcess(object stateInfo)
        {
            new FileMigrator().MigrateFile(this.currentFile, this.fileMigrationSpecification, this.currentTargetWeb);
        }

        /// <summary>
        /// Returns all files of web.
        /// </summary>
        /// <param name="clientContext">the ClientContext</param>
        /// <param name="web">the Web</param>
        /// <returns>all files of web</returns>
        private List<FileCollection> GetAllFilesOfWeb(ClientContext clientContext, Web web)
        {
            List<FileCollection> files = new List<FileCollection>();

            Folder rootFolder = web.RootFolder;
            clientContext.Load(rootFolder);
            clientContext.ExecuteQuery();

            files = this.GetFilesOfAllFolder(clientContext, rootFolder, files);

            return files;
        }

        /// <summary>
        /// Gets all files of all folders of the web.
        /// </summary>
        /// <param name="clientContext">the ClientContext</param>
        /// <param name="folder">the folder</param>
        /// <param name="files">already found files</param>
        /// <returns>all files</returns>
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
                    files = this.GetFilesOfAllFolder(clientContext, f, files);
                }
            }

            return files;
        }

        /// <summary>
        /// Gets the files of the shared documents folder.
        /// </summary>
        /// <param name="clientContext">the ClientContext</param>
        /// <param name="web">the Web</param>
        /// <returns>files of shared documents folder</returns>
        private FileCollection GetFilesOfSharedDocumentsFolder(ClientContext clientContext, Web web)
        {
            Folder sharedDocumentFolder = this.GetSharedDocumentsFolder(clientContext, web);
            return this.GetFiles(clientContext, sharedDocumentFolder);
        }

        /// <summary>
        /// Gets the shared documents folder of the web
        /// </summary>
        /// <param name="clientContext">the ClientContext</param>
        /// <param name="web">the Web</param>
        /// <returns>the shared documents folder</returns>
        /// <exception cref="FileMigrationException">if the shared documents folder was not found</exception>
        private Folder GetSharedDocumentsFolder(ClientContext clientContext, Web web)
        {
            Folder rootFolder = web.RootFolder;
            clientContext.Load(rootFolder);
            clientContext.ExecuteQuery();

            FolderCollection folders = rootFolder.Folders;
            clientContext.Load(folders);
            clientContext.ExecuteQuery();

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

        /// <summary>
        /// Gets the files of the given folder.
        /// </summary>
        /// <param name="clientContext">the ClientContext</param>
        /// <param name="folder">the Folder</param>
        /// <returns>files of folder</returns>
        private FileCollection GetFiles(ClientContext clientContext, Folder folder)
        {
            FileCollection files = folder.Files;
            clientContext.Load(files);
            clientContext.ExecuteQuery();

            //// TODO where files ends with...

            return files;
        }

        /// <summary>
        /// Sets necessary specifications.
        /// </summary>
        /// <exception cref="FileMigrationException">if the migration fails.</exception>
        private void SpecifySharePointSpecification()
        {
            try
            {
                var endpointAddress = new EndpointAddress(this.fileMigrationSpecification.ServiceAddress);
                FileMigrationService.FileMigrationClient client = new FileMigrationService.FileMigrationClient(new WSHttpBinding(SecurityMode.None), endpointAddress);
                FileMigrationService.IFileMigration fileMigrationService = client.ChannelFactory.CreateChannel();

                Console.WriteLine("Setting the max file size.");
                this.fileMigrationSpecification.MaxFileSize = fileMigrationService.GetMaxFileSize();
                this.fileMigrationSpecification.MaxFileSize = 2;
            }
            catch (Exception)
            {
                throw new FileMigrationException("Could not connect to the service '" + this.fileMigrationSpecification.ServiceAddress.ToString() + "'.");
            }
        }
    }
}
