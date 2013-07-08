using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Sharezbold.ContentMigration
{
    internal class ContentDownloader
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

        public SpTreeNode GenerateMigrationTree()
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

/*            Site site = context.Site;
            context.Load(site);
            Site
            */
            /// according to various sites, "site collections" (Site objects) cannot be loaded with client object model
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

            SpTreeNode root = new SpTreeNode(web.Title + url, web);
            root.ImageIndex = 1;
            root.SelectedImageIndex = 1;
            
            //test
            var listColl = new List<Microsoft.SharePoint.Client.List>();
            try
            {
                Web currentWeb = web;//context.Site.OpenWeb(webURL);

                /// load 
                var query = from list in currentWeb.Lists
                            where list.BaseType == BaseType.DocumentLibrary || list.BaseType==BaseType.GenericList 
                            orderby list.BaseType
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.ToString());
            }
 


            /*
            ListCollection listColl = web.Lists;
            context.Load(listColl);
            context.ExecuteQuery();

            Debug.WriteLine("listcoll.count {0}", listColl.Count);*/
            foreach (List li in listColl)
            {
                var spList = new SpTreeNode(li.Title, li);
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
                    var spListItem = new SpTreeNode(lii.DisplayName + "", lii);
                    var fieldValues = lii.FieldValues;
                    
                    foreach (KeyValuePair<string, object> entry in fieldValues)
                        spListItem.Nodes.Add(new SpTreeNode(entry.Key, entry));

                    spList.Nodes.Add(spListItem);
                }


                root.Nodes.Add(spList);
            }
            
            /*
            foreach (Site site in context.Site)
            {
                                // Print the url and the number of webs in this SPSite.
                                Console.WriteLine("\t\tPrimaryUri: {0}", site.PrimaryUri);
                                Console.WriteLine("\t\tWebCount: {0}", site.AllWebs.Count);

                                // Iterate trough each web.
                                foreach (SPWeb web in site.AllWebs)
                                {
                                    // Print some information about the web.
                                    Console.WriteLine("\t\t\tIsRootWeb: {0}", web.IsRootWeb);
                                    Console.WriteLine("\t\t\tTitle: {0}", web.Title);
                                    Console.WriteLine("\t\t\tListCount: {0}", web.Lists.Count);

                                    // Iterate trough each list in the web.
                                    foreach (SPList list in web.Lists)
                                    {
                                        // Print the title of the list.
                                        Console.WriteLine("\t\t\t\tList Name:{0}", list.Title);
                                    }
                                }
                            }*/
                            




            return root;
        }
    }
}
