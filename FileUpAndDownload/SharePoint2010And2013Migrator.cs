//-----------------------------------------------------------------------
// <copyright file="AbstractMigrator.cs" company="FH Wiener Neustadt">
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
    /// Abstract class for migrating files from source SharePoint to target SharePont.
    /// </summary>
    public class SharePoint2010And2013Migrator
    {
        /// <summary>
        /// Instance of the real migrater.
        /// </summary>
        

        private ClientContext sourceClientContext;
        private ClientContext targetClientContext;

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

        public void MigrateFiles(List<string> guidList)
        {
            if (sourceClientContext.ServerVersion.Major < 14)
            {
                throw new SharePointNotSupportedException("The source SharePoint is not supported (2010 or newer)!");
            }

            if (targetClientContext.ServerVersion.Major < 14)
            {
                throw new SharePointNotSupportedException("The target SharePoint is not supported (2010 or newer)!");
            }

            if (guidList == null || guidList.Count == 0)
            {
                Console.WriteLine("no guid-list given to migrate files");
            }

            foreach (var guidAsString in guidList)
            {
                Guid guid = new Guid(guidAsString);

                MigrateFile(guid);
            }
        }

        private void MigrateFile(Guid guidOfList)
        {
            

            SharePoint2010And2013Downloader downloader = new SharePoint2010And2013Downloader(sourceClientContext);
            SharePoint2010And2013Uploader uploader = new SharePoint2010And2013Uploader(targetClientContext); ;

            Stream stream = null;
            string relativeUrl = null;
            string documentListName = null;

            try
            {

                this.GetListItemCollectionFromSharePoint(guidOfList);

                if (this.listItemList == null)
                {
                    return;
                }

                foreach (var document in this.listItemList)
                {
                    stream = downloader.DownloadDocument(guidOfList, out documentListName, out relativeUrl);
                    if (stream != null)
                    {
                        uploader.UploadDocument(documentListName, relativeUrl, stream);
                    }
                }

                
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during migration. Exception = ", e.Message);
                // throw new FileMigrationException("Could not migrate file with realtive url '" + relativeUrl + "'.", e);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        /// <summary>
        /// Returns the ListItemCollection of the given SharePoint.
        /// </summary>
        /// <param name="documentListName">name of documentList</param>
        /// <param name="fileReference">name of field-ref for the query.</param>
        /// <param name="documentName">value of given type for the query</param>
        /// <param name="rowLimit">limit of the row for the query</param>
        /// <returns>ListItemCollection</returns>
        private void GetListItemCollectionFromSharePoint(Guid guid)
        {
            Console.WriteLine("GUID = {0}", guid.ToString());
            if (guid.ToString().Equals("bd293c00-bfa9-4282-b824-f109900ced64"))
            {
                Console.WriteLine("debug");
            }

            ListItemCollection listItems = null;
            List documentsList = sourceClientContext.Web.Lists.GetById(guid);
            
            listItems = documentsList.GetItems(CamlQuery.CreateAllItemsQuery());

            sourceClientContext.Load(documentsList);
            sourceClientContext.Load(listItems);

            sourceClientContext.ExecuteQuery();

            GetListItemCollectionFromSharePoint(documentsList, listItems);
        }

        private void GetListItemCollectionFromSharePoint(List documentsList, ListItemCollection listItems)
        {
            if (listItems == null)
            {
                return;
            }

            int rowLimit = 1;
            string fileReference = "FileLeafRef";

            foreach (var item in listItems)
            {
                sourceClientContext.Load(item);
                sourceClientContext.ExecuteQuery();

                Microsoft.SharePoint.Client.File file = item.File;

                try
                {
                    sourceClientContext.Load(file);
                    sourceClientContext.ExecuteQuery();
                    Console.WriteLine("hurra! file = {0}", file.Name);
                }
                catch (Exception e)
                {
                    Console.WriteLine("no file; e = " + e.Message);
                    return;
                }
                
                sourceClientContext.Load(item);
                sourceClientContext.ExecuteQuery();

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

                sourceClientContext.Load(documentsList);
                sourceClientContext.Load(filteredListItems);

                sourceClientContext.ExecuteQuery();

                if (filteredListItems != null && filteredListItems.Count == 1)
                {
                    if (listItemList == null)
                    {
                        listItemList = new List<ListItem>();
                    }
                    listItemList.Add(filteredListItems[0]);
                }
            }
        }
    }
}
