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
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Abstract class for migrating files from source SharePoint to target SharePont.
    /// </summary>
    public class SharePoint2010And2013Migrator
    {
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

        /// <summary>
        /// Migrates the files of the given guid-List.
        /// </summary>
        /// <param name="guidList">list with guid of sites.</param>
        /// <exception cref="SharePointNotSupportedException">If the SharePoint-version is not supported.</exception>
        /// <exception cref="FileMigrationException">If the migration fails</exception>
        public void MigrateFiles(List<string> guidList)
        {
            if (this.sourceClientContext.ServerVersion.Major < 14)
            {
                throw new SharePointNotSupportedException("The source SharePoint is not supported (2010 or newer)!");
            }

            if (this.targetClientContext.ServerVersion.Major < 14)
            {
                throw new SharePointNotSupportedException("The target SharePoint is not supported (2010 or newer)!");
            }

            if (guidList == null || guidList.Count == 0)
            {
                Console.WriteLine("no guid-list given to migrate files");
                return;
            }

            foreach (var guidAsString in guidList)
            {
                Guid guid = new Guid(guidAsString);
                Stream stream = null;
                string documentListName = null;

                try
                {
                    this.GetListItemCollectionFromSharePoint(guid, out documentListName);

                    if (this.listItemList == null)
                    {
                        return;
                    }

                    foreach (ListItem item in this.listItemList)
                    {
                        this.MigrateFile(item, documentListName);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception during migration. Exception = ", e.Message);
                    throw new FileMigrationException("Could not migrate files for GUID '" + guid + "'.", e);
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Migrate the file.
        /// </summary>
        /// <param name="item">the ListItem to migrate</param>
        /// <param name="documentListName">given name of documentlist</param>
        private void MigrateFile(ListItem item, string documentListName)
        {
            SharePoint2010And2013Downloader downloader = new SharePoint2010And2013Downloader(this.sourceClientContext);
            SharePoint2010And2013Uploader uploader = new SharePoint2010And2013Uploader(this.targetClientContext);

            string relativeUrl = item["FileRef"].ToString();
            Stream stream = downloader.DownloadDocument(relativeUrl);
            if (stream != null)
            {
                uploader.UploadDocument(documentListName, relativeUrl, stream);
            }
        }

        /// <summary>
        /// Gets the ListItemCollection of files of the source SharePoint.
        /// </summary>
        /// <param name="guid">guid to search for files</param>
        /// <param name="documentListName">name of documentlist to write</param>
        private void GetListItemCollectionFromSharePoint(Guid guid, out string documentListName)
        {
            Console.WriteLine("GUID = {0}", guid.ToString());

            ListItemCollection listItems = null;
            List documentsList = this.sourceClientContext.Web.Lists.GetById(guid);
            
            listItems = documentsList.GetItems(CamlQuery.CreateAllItemsQuery());
            documentListName = documentsList.Title;

            this.sourceClientContext.Load(documentsList);
            this.sourceClientContext.Load(listItems);

            this.sourceClientContext.ExecuteQuery();

            this.FilterFilesOfListItems(documentsList, listItems);
        }

        /// <summary>
        /// Filters the files of given ListItemCollection.
        /// </summary>
        /// <param name="documentsList">documents list to search for files.</param>
        /// <param name="listItems">all ListItems to filter for files</param>
        private void FilterFilesOfListItems(List documentsList, ListItemCollection listItems)
        {
            if (listItems == null)
            {
                return;
            }

            int rowLimit = 1;
            string fileReference = "FileLeafRef";

            foreach (var item in listItems)
            {
                this.sourceClientContext.Load(item);
                this.sourceClientContext.ExecuteQuery();

                Microsoft.SharePoint.Client.File file = item.File;

                try
                {
                    this.sourceClientContext.Load(file);
                    this.sourceClientContext.ExecuteQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine("no file; e = " + e.Message);
                    return;
                }
                
                this.sourceClientContext.Load(item);
                this.sourceClientContext.ExecuteQuery();

                Console.WriteLine("item = {0}", item.Id);
                
                CamlQuery camlQuery = new CamlQuery();

                camlQuery.ViewXml =
                @"<View>

<Query>
<Where>

<Eq>
<FieldRef Name='" + fileReference + @"'/>
<Value Type='Text'>" + item.DisplayName + @"</Value>
</Eq>

</Where>                    
<RowLimit>" + rowLimit.ToString() + @"</RowLimit>

</Query>
</View>";

                ListItemCollection filteredListItems = documentsList.GetItems(camlQuery);

                this.sourceClientContext.Load(documentsList);
                this.sourceClientContext.Load(filteredListItems);

                this.sourceClientContext.ExecuteQuery();

                if (filteredListItems != null && filteredListItems.Count == 1)
                {
                    if (this.listItemList == null)
                    {
                        this.listItemList = new List<ListItem>();
                    }

                    this.listItemList.Add(filteredListItems[0]);
                }
            }
        }
    }
}
