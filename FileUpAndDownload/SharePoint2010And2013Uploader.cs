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

        internal void UploadDocument(MigrationFile migrationFile,  Web targetWeb)
        {
            Console.WriteLine("Upload file '{0}' now.", migrationFile.File.Name);

            Folder rootFolder = targetWeb.RootFolder;
            this.targetClientContext.Load(rootFolder);
            this.targetClientContext.ExecuteQuery();

            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = ReadFully(migrationFile.DownloadedStream);
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

        private System.IO.MemoryStream ConvertStreamToMemoryStream(System.IO.Stream stream)
        {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
 
            if (stream != null)
            {

                byte[] buffer = ReadFully(stream);
 
                if (buffer != null)
                {
                    var binaryWriter = new System.IO.BinaryWriter(memoryStream);
                    binaryWriter.Write(buffer);
                }
            }
            return memoryStream;
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
