//-----------------------------------------------------------------------
// <copyright file="SharePoint2010And2013Uploader.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using System.IO;
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
        /// Uploads the given file to the SharePoint.
        /// </summary>
        /// <param name="documentListName">name of the list for the file</param>
        /// <param name="documentListURL">relative url of the document</param>
        /// <param name="documentStream">file-data as stream</param>
        internal void UploadDocument(string documentListName, string documentListURL, Stream documentStream)
        {
            //// gets the document-list
            List documentsList = this.targetClientContext.Web.Lists.GetByTitle(documentListName);

            var fileCreationInformation = new FileCreationInformation();
            fileCreationInformation.Content = this.ConvertStreamToByteArray(documentStream);
            fileCreationInformation.Overwrite = true;
            //// Upload URL:
            fileCreationInformation.Url = this.targetClientContext.Url + documentListURL;

            Microsoft.SharePoint.Client.File uploadFile = documentsList.RootFolder.Files.Add(fileCreationInformation);

            uploadFile.ListItemAllFields.Update();
            this.targetClientContext.ExecuteQuery();
        }

        /// <summary>
        /// Converts a stream to a byte-array.
        /// </summary>
        /// <param name="stream">stream to convert</param>
        /// <returns>bytearray of stream</returns>
        private byte[] ConvertStreamToByteArray(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
