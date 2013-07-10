using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Sharezbold.ContentMigration
{
    public class ContentDownloader
    {
        /// <summary>
        /// Source context of Sharepoint server
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
        private SpTreeNode generateMigrationTreeGetWeb(Web web, string title, bool includeListItems=true)
        {
            SpTreeNode root = new SpTreeNode(new MigrationObject(title, web));

            var listColl = new List<Microsoft.SharePoint.Client.List>();
            Web currentWeb = web;

            /// load 
            var query = from list in currentWeb.Lists
                        where list.BaseType == BaseType.DocumentLibrary || list.BaseType == BaseType.GenericList
                        //orderby list.BaseType
                        select list;
            var AllLists = currentWeb.Lists;
            var listCollection = context.LoadQuery(query.Include(myList => myList.Title,
                               myList => myList.Id,
                               myList => myList.BaseType,
                               myList => myList.RootFolder.ServerRelativeUrl,
                               myList => myList.ParentWebUrl,
                               myList => myList.Hidden,
                               myList => myList.IsApplicationList));
            context.ExecuteQuery();

            listColl.AddRange(from list in listCollection
                              where !list.Hidden
                              select list);


            foreach (List li in listColl)
            {
                var spList = new SpTreeNode(new MigrationObject(li.Title, li));
                if (li.BaseType == BaseType.DocumentLibrary)
                {
                    spList.ImageIndex = 3;
                    spList.SelectedImageIndex = 3;
                }
                else
                {
                    spList.ImageIndex = 2;
                    spList.SelectedImageIndex = 2;
                }

                if (includeListItems)
                {
                    CamlQuery cq = new CamlQuery();
                    cq.ViewXml = "<Query><OrderBy><FieldRef Name='Id' /></OrderBy></Query>";

                    ListItemCollection listItemColl = li.GetItems(cq);
                    context.Load(listItemColl, items => items.Include(
                        item => item.Id,
                        item => item.DisplayName));

                    //context.Load(listItemColl);
                    context.ExecuteQuery();

                    foreach (ListItem lii in listItemColl)
                    {
                        context.Load(lii);
                        context.ExecuteQuery();

                        var spListItem = new SpTreeNode(new MigrationObject(lii.DisplayName + "", lii));
                        /*
                        var fieldValues = lii.FieldValues;

                        foreach (KeyValuePair<string, object> entry in fieldValues)
                        {
                            if (!entry.Key.Contains("_"))
                            {
                                spListItem.Nodes.Add(new SpTreeNode(new MigrationObject(entry.Key + ": " + entry.Value, entry)));
                            }
                        }
                        */
                        spList.Nodes.Add(spListItem);
                    }
                }
                root.Nodes.Add(spList);
            }
            return root;
        }

        /// <summary>
        /// Generates a root tree node of the selected site collection and populates it with its child elements
        /// </summary>
        /// <returns>The root tree node</returns>
        public SpTreeNode GenerateMigrationTree(bool includeListItems=true)
        {
            /*
             * 	Client			|	Server
             * 	----------------------------------
             * 	ClientContext	|	SPContext
             * 	Site			|	SPSite			+ = Site Collection
             * 	Web				|	SPWeb			+ = Web(site)
             * 	List			|	SPList			+
             * 	ListItem		|	SPListItem		+
             * 	Field			|	SPField			+
             */

            /// First the site collection is being loaded.
            /// Infor: According to various sites, a list of all "site collections" cannot be loaded with the client object model
            /// somehow the client object model treats the site collection as a web
            Web web = context.Web;
            context.Load(web);
            context.ExecuteQuery();

            string url="";

            /// URL is not supported in SP2010
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
                //add the site collection itself
                root.Nodes.Add(this.generateMigrationTreeGetWeb(web, web.Title, includeListItems));

                //retreive all webs in the site collection
                var query1 = from webs in web.Webs
                             select webs;

                var webColl = context.LoadQuery(query1.Include(webs => webs.Title));
                context.ExecuteQuery();

                //add the webs to the root node
                foreach (Web w in webColl)
                {
                    root.Nodes.Add(this.generateMigrationTreeGetWeb(w, w.Title, includeListItems));
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
