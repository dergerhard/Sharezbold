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
    using System.IO;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Migrates files from SharePoint 2010 or 2013 to SharePoint 2013.
    /// </summary>
    internal class SharePoint2010And2013Downloader
    {

        public string RelativeUrl { get; set; }

        /// <summary>
        /// The ClientContext of the source SharePoint.
        /// </summary>
        private ClientContext sourceClientContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePoint2010And2013Downloader"/> class.
        /// </summary>
        /// <param name="sourceClientContext">clientContext of the source SharePoint</param>
        public SharePoint2010And2013Downloader(ClientContext sourceClientContext)
        {
            this.sourceClientContext = sourceClientContext;
        }

        /// <summary>
        /// Downloads the document from the SharePoint.
        /// </summary>
        /// <param name="documentListName">name of the documentList</param>
        /// <param name="documentName">document name to download</param>
        /// <returns>downloaded file as stream</returns>
        /// <exception cref="FileNotFoundException">if the file was not found</exception>
        internal Stream DownloadDocument(Guid guidOfList, out string documentName, out string relativeUrl)
        {
            Stream stream = null;
            ListItem item = GetDocumentFromSharePoint(guidOfList);
            if (item != null)
            {
                relativeUrl = item["FileRef"].ToString();
                documentName = item.DisplayName;
                FileInformation fileInformation = Microsoft.SharePoint.Client.File.OpenBinaryDirect(sourceClientContext, this.RelativeUrl);

                stream = fileInformation.Stream;
            }
            else
            {
                relativeUrl = null;
                documentName = null;
                Console.WriteLine("no files found in list with guid '{0}'.", guidOfList.ToString());
            }

            return stream;
        }

        /// <summary>
        /// Get the document from the SharePoint as ListItem.
        /// </summary>
        /// <param name="documentListName">name of the documentlist</param>
        /// <param name="documentName">name of the document</param>
        /// <returns>document from SharePoint as ListItem</returns>
        private ListItem GetDocumentFromSharePoint(Guid guidOfList)
        {
            ListItemCollection listItems = GetListItemCollectionFromSharePoint(guidOfList);

            return (listItems != null && listItems.Count == 1) ? listItems[0] : null;
        }

        /// <summary>
        /// Returns the ListItemCollection of the given SharePoint.
        /// </summary>
        /// <param name="documentListName">name of documentList</param>
        /// <param name="fileReference">name of field-ref for the query.</param>
        /// <param name="documentName">value of given type for the query</param>
        /// <param name="rowLimit">limit of the row for the query</param>
        /// <returns>ListItemCollection</returns>
        private ListItemCollection GetListItemCollectionFromSharePoint(Guid guidOfList)
        {
            int rowLimit = 1;
            string fileReference = "FileLeafRef";

            ListItemCollection listItems = null;
            List documentsList = sourceClientContext.Web.Lists.GetById(guidOfList);
            
            CamlQuery camlQuery = new CamlQuery();

            camlQuery.ViewXml =
            @"<View>

<Query>
<Where>

<Eq>
<FieldRef Name='" + fileReference + @"'/>
</Eq>

</Where>                    
<RowLimit>" + rowLimit.ToString() + @"</RowLimit>

</Query>
</View>";
            /*
            camlQuery.ViewXml =
            @"<View>

<Query>
<Where>

<Eq>
<FieldRef Name='" + fileReference + @"'/>

<Value Type='Text'>" + documentName + @"</Value>
</Eq>

</Where>                    
<RowLimit>" + rowLimit.ToString() + @"</RowLimit>

</Query>
</View>";
*/

            listItems = documentsList.GetItems(camlQuery);

            sourceClientContext.Load(documentsList);
            sourceClientContext.Load(listItems);

            sourceClientContext.ExecuteQuery();

            return listItems;
        }
    }
}
