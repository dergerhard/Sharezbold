//-----------------------------------------------------------------------
// <copyright file="SharePoint2010And2013FileMigrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Migrates files from SharePoint 2010 or 2013 to SharePoint 2013.
    /// </summary>
    internal class SharePoint2010And2013FileMigrator
    {
        /// <summary>
        /// The ClientContext of the source SharePoint.
        /// </summary>
        private ClientContext sourceClientContext;

        /// <summary>
        /// The ClientContext of the target SharePoint.
        /// </summary>
        private ClientContext targetClientContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePoint2010And2013FileMigrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">clientContext of the source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of the target SharePoint</param>
        public SharePoint2010And2013FileMigrator(ClientContext sourceClientContext, ClientContext targetClientContext)
        {
            this.sourceClientContext = sourceClientContext;
            this.targetClientContext = targetClientContext;
        }

        /// <summary>
        /// Migrates the file from the source to the target SharePoint.
        /// </summary>
        /// <param name="documentListName">name of the documentlist</param>
        /// <param name="documentName">name of the document to migrate</param>
        /// <exception cref="FileMigrationException">if migration of file fails</exception>
        internal void MigrateFile(string documentListName, string documentName)
        {
            Stream fileStream = null;
            try
            {
                fileStream = this.DownloadDocument(documentListName, documentName);
            }
            catch (FileNotFoundException e)
            {
                throw new FileMigrationException(e.Message, e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during download file from source-SharePoint. Error:");
                Console.WriteLine(e);
                throw new FileMigrationException("could not download file from source SharePoint.", e);
            }

            try
            {
                ////TODO get the document-list-url
                this.UploadDocument(documentListName, null, documentName, ConvertStreamToByteArray(fileStream));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during download file to target-SharePoint. Error:");
                Console.WriteLine(e);
                throw new FileMigrationException("could not upload file to target SharePoint.", e);
            }
        }

        /// <summary>
        /// Downloads the document from the SharePoint.
        /// </summary>
        /// <param name="documentListName">name of the documentList</param>
        /// <param name="documentName">document name to download</param>
        /// <returns>downloaded file as stream</returns>
        /// <exception cref="FileNotFoundException">if the file was not found</exception>
        private Stream DownloadDocument(string documentListName, string documentName)
        {
            ListItem item = GetDocumentFromSharePoint(documentListName, documentName);
            if (item != null)
            {
                FileInformation fileInformation = Microsoft.SharePoint.Client.File.OpenBinaryDirect(sourceClientContext,
                    item["FileRef"].ToString());

                return fileInformation.Stream;


            }

            throw new FileNotFoundException("File '" + documentName + "' not found on SharePoint.");
        }

        /// <summary>
        /// Uploads the given document, which is handed by a bytearray.
        /// </summary>
        /// <param name="documentListName">name of document-list</param>
        /// <param name="documentListURL">url of document-list</param>
        /// <param name="documentName">name of document</param>
        /// <param name="documentStream">documentstream as bytearray to upload</param>
        private void UploadDocument(string documentListName, string documentListURL, string documentName, byte[] documentStream)
        {
            //Get Document List
            List documentsList = targetClientContext.Web.Lists.GetByTitle(documentListName);

            var fileCreationInformation = new FileCreationInformation();
            //Assign to content byte[] i.e. documentStream
            fileCreationInformation.Content = documentStream;

            //Allow owerwrite of document
            fileCreationInformation.Overwrite = true;

            //Upload URL
            fileCreationInformation.Url = targetClientContext.Url + documentListURL + documentName;

            Microsoft.SharePoint.Client.File uploadFile = documentsList.RootFolder.Files.Add(
                fileCreationInformation);

            //Update the metadata for a field having name "DocType"
            uploadFile.ListItemAllFields["DocType"] = "Favourites";

            uploadFile.ListItemAllFields.Update();
            targetClientContext.ExecuteQuery();
        }

        /// <summary>
        /// Get the document from the SharePoint as ListItem.
        /// </summary>
        /// <param name="documentListName">name of the documentlist</param>
        /// <param name="documentName">name of the document</param>
        /// <returns>document from SharePoint as ListItem</returns>
        private ListItem GetDocumentFromSharePoint(string documentListName, string documentName)
        {
            ListItemCollection listItems = GetListItemCollectionFromSharePoint(documentListName, "FileLeafRef",
                documentName, "Text", 1);


            return (listItems != null && listItems.Count == 1) ? listItems[0] : null;
        }

        /// <summary>
        /// Returns the ListItemCollection of the given SharePoint.
        /// </summary>
        /// <param name="documentListName">name of documentList</param>
        /// <param name="name">name of field-ref for the query.</param>
        /// <param name="value">value of given type for the query</param>
        /// <param name="type">type for the query</param>
        /// <param name="rowLimit">limit of the row for the query</param>
        /// <returns>ListItemCollection</returns>
        private ListItemCollection GetListItemCollectionFromSharePoint(string documentListName, string name, string value, string type, int rowLimit)
        {
            ListItemCollection listItems = null;
            List documentsList = sourceClientContext.Web.Lists.GetByTitle(documentListName);


            CamlQuery camlQuery = new CamlQuery(); ;

            camlQuery.ViewXml =
            @"<View>

<Query>
<Where>

<Eq>
<FieldRef Name='" + name + @"'/>

<Value Type='" + type + "'>" + value + @"</Value>
</Eq>

</Where>                    
<RowLimit>" + rowLimit.ToString() + @"</RowLimit>

</Query>
</View>";


            listItems = documentsList.GetItems(camlQuery);

            sourceClientContext.Load(documentsList);
            sourceClientContext.Load(listItems);

            sourceClientContext.ExecuteQuery();

            return listItems;
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
