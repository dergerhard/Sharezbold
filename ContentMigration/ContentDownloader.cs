//-----------------------------------------------------------------------
// <copyright file="ContentDownloader.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ContentMigration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Responsible for downloading data from share point
    /// </summary>
    public class ContentDownloader
    {
        /// <summary>
        /// Source context of Share point server
        /// </summary>
        private ClientContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentDownloader"/> class.
        /// </summary>
        /// <param name="context">The ClientContext of the source SharePoint.</param>
        public ContentDownloader(ClientContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Generates the web sub-tree
        /// </summary>
        /// <param name="web">web to start from</param>
        /// <param name="title">title to set to the node</param>
        /// <returns>a TreeNode for the TreeView</returns>
        private SpTreeNode GenerateMigrationTreeGetWeb(Web web, string title, bool includeListItems = true)
        {
            SpTreeNode root = new SpTreeNode(new MigrationObject(title, web));

            var listColl = new List<Microsoft.SharePoint.Client.List>();
            Web currentWeb = web;

            // load 
            var query = from list in currentWeb.Lists
                        where list.BaseType == BaseType.DocumentLibrary || list.BaseType == BaseType.GenericList
                        select list;

            var listCollection = this.context.LoadQuery(query.Include(myList => myList.Title,
                               myList => myList.Id,
                               myList => myList.BaseType,
                               myList => myList.RootFolder.ServerRelativeUrl,
                               myList => myList.ParentWebUrl,
                               myList => myList.Hidden,
                               myList => myList.IsApplicationList,
                               myList => myList.Fields,
                               myList => myList.BaseTemplate));
            this.context.ExecuteQuery();

            listColl.AddRange(from list in listCollection
                              where !list.Hidden
                              select list);

            foreach (List li in listColl)
            {
                var sharepointList = new SpTreeNode(new MigrationObject(li.Title, li));
                if (li.BaseType == BaseType.DocumentLibrary)
                {
                    sharepointList.ImageIndex = 3;
                    sharepointList.SelectedImageIndex = 3;
                }
                else
                {
                    sharepointList.ImageIndex = 2;
                    sharepointList.SelectedImageIndex = 2;
                }

                
                        foreach (Field entry in li.Fields)
                        {
                            Debug.WriteLine("defaultValue:      " + entry.DefaultValue);
                            Debug.WriteLine("description:       " + entry.Description);
                            Debug.WriteLine(entry.Direction);

                        }

                if (includeListItems)
                {
                    CamlQuery cq = new CamlQuery();
                    cq.ViewXml = "<Query><OrderBy><FieldRef Name='Id' /></OrderBy></Query>";

                    ListItemCollection listItemColl = li.GetItems(cq);
                    this.context.Load(
                        listItemColl, 
                        items => items.Include(
                            item => item.Id,
                            item => item.DisplayName));

                    this.context.ExecuteQuery();

                    foreach (ListItem lii in listItemColl)
                    {
                        this.context.Load(lii);
                        this.context.ExecuteQuery();

                        var sharepointListItem = new SpTreeNode(new MigrationObject(lii.DisplayName, lii));
                        /*
                        var fieldValues = lii.FieldValues;

                        Debug.WriteLine("HERE COME THE FIELDS OF {0}", lii.DisplayName);
                        foreach (KeyValuePair<string, object> entry in fieldValues)
                        {
                            if (!entry.Key.Contains("_"))
                            {
                                Debug.WriteLine("{0}:       {1}", entry.Key, entry.Value);
                                //sharepointListItem.Nodes.Add(new SpTreeNode(new MigrationObject(entry.Key + ": " + entry.Value, entry)));
                            }
                        }*/
                        
                        sharepointList.Nodes.Add(sharepointListItem);
                    }
                }

                root.Nodes.Add(sharepointList);
            }

            return root;
        }

        /// <summary>
        /// Generates a root tree node of the selected site collection and populates it with its child elements
        /// </summary>
        /// <param name="includeListItems">defines if list items should be loaded as well</param>
        /// <returns>the root node</returns>
        public SpTreeNode GenerateMigrationTree(bool includeListItems = true)
        {
            // First the site collection is being loaded.
            // Infor: According to various sites, a list of all "site collections" cannot be loaded with the client object model
            // somehow the client object model treats the site collection as a web
            Web web = this.context.Web;
            this.context.Load(web);
            this.context.ExecuteQuery();

            string url = string.Empty;

            // URL is not supported in SP2010
            try 
            { 
                url = ", " + web.Url; 
            }
            catch (Exception e) 
            { 
                Debug.WriteLine(e.ToString()); 
            }

            SpTreeNode root = new SpTreeNode(new MigrationObject(web.Title + url, web));
            root.ImageIndex = 1;
            root.SelectedImageIndex = 1;

            try
            {
                // add the site collection itself
                root.Nodes.Add(this.GenerateMigrationTreeGetWeb(web, web.Title, includeListItems));

                // retreive all webs in the site collection
                var query1 = from webs in web.Webs
                             select webs;

                var webColl = this.context.LoadQuery(query1.Include(webs => webs.Title));
                this.context.ExecuteQuery();

                // add the webs to the root node
                foreach (Web w in webColl)
                {
                    root.Nodes.Add(this.GenerateMigrationTreeGetWeb(w, w.Title, includeListItems));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.ToString());
                throw new LoadingElementsException("Error while loading the source elements. Original message: " + ex.Message);
            }

            return root;
        }
    }
}
