

namespace Sharezbold.FileMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO;
    using Microsoft.SharePoint.Client;


    internal class SharePoint2010And2013Uploader
    {

        private ClientContext targetClientContext;

        public SharePoint2010And2013Uploader(ClientContext clientContext)
        {
            this.targetClientContext = clientContext;
        }

        /// <summary>
        /// Uploads the given document, which is handed by a bytearray.
        /// </summary>
        /// <param name="documentListName">name of document-list</param>
        /// <param name="documentListURL">url of document-list</param>
        /// <param name="documentName">name of document</param>
        /// <param name="documentStream">documentstream as bytearray to upload</param>
        internal void UploadDocument(string documentListName, string documentListURL, Stream documentStream)
        {
            //Get Document List
            List documentsList = targetClientContext.Web.Lists.GetByTitle(documentListName);

            var fileCreationInformation = new FileCreationInformation();
            //Assign to content byte[] i.e. documentStream
            fileCreationInformation.Content = ConvertStreamToByteArray(documentStream);

            //Allow owerwrite of document
            fileCreationInformation.Overwrite = false;

            //Upload URL
            fileCreationInformation.Url = targetClientContext.Url + documentListURL;

            Microsoft.SharePoint.Client.File uploadFile = documentsList.RootFolder.Files.Add(
                fileCreationInformation);

            uploadFile.ListItemAllFields.Update();
            targetClientContext.ExecuteQuery();
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
