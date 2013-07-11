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

    internal class SharePoint2010And2013FileMigrator
    {
        private ClientContext sourceClientContext;
        private ClientContext targetClientContext;

        public SharePoint2010And2013FileMigrator(ClientContext sourceClientContext, ClientContext targetClientContext)
        {
            this.sourceClientContext = sourceClientContext;
            this.targetClientContext = targetClientContext;
        }

        internal void MigrateFile(string documentListName, string documentName)
        {
            Stream fileStream = null;
            try
            {
                fileStream = this.DownloadDocument(documentName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during download file from source-SharePoint. Error:");
                Console.WriteLine(e);
                throw new FileMigrationException("could not download file from source SharePoint.", e);
            }

            try
            {
                this.UploadDocument(documentListName, null, documentName, ConvertStreamToByteArray(fileStream));
            }
            catch (Exception)
            {
                Console.WriteLine("Error during download file to target-SharePoint. Error:");
                Console.WriteLine(e);
                throw new FileMigrationException("could not upload file to target SharePoint.", e);
            }
        }

        private Stream DownloadDocument(string documentName)
        {
            ListItem item = GetDocumentFromSP(documentName);
            if (item != null)
            {
                FileInformation fileInformation = Microsoft.SharePoint.Client.File.OpenBinaryDirect(sourceClientContext,
                    item["FileRef"].ToString());

                return fileInformation.Stream;


            }
            return null;
        }

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


        private ListItem GetDocumentFromSP(string documentName)
        {
            //This method is discussed above i.e. Get List Item Collection from SharePoint
            //Document List
            ListItemCollection listItems = GetListItemCollectionFromSP("FileLeafRef",
                documentName, "Text", 1);


            return (listItems != null && listItems.Count == 1) ? listItems[0] : null;
        }

        private ListItemCollection GetListItemCollectionFromSP(string name, string value, string type, int rowLimit)
        {
            //Update siteURL and DocumentListName with as per your site
            string siteURL = "URL of the Site";
            string documentListName = "DocumentList";
            ListItemCollection listItems = null;
            using (ClientContext clientContext = new ClientContext(siteURL))
            {
                List documentsList = clientContext.Web.Lists.GetByTitle(documentListName);


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

                clientContext.Load(documentsList);
                clientContext.Load(listItems);

                clientContext.ExecuteQuery();
            }


            return listItems;
        }

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
