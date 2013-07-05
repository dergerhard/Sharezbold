using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharezbold;
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

            
            Web web = context.Web;
            context.Load(web);
            context.ExecuteQuery();

            SpTreeNode root = new SpTreeNode(web.Url, web);
            
            ListCollection listColl = web.Lists;
            context.Load(listColl);
            context.ExecuteQuery();

            Debug.WriteLine("listcoll.count {0}", listColl.Count);
            foreach (List li in listColl)
            {
                var spList = new SpTreeNode(li.Title, li);
                CamlQuery cq = new CamlQuery();
                cq.ViewXml = "<Query><OrderBy><FieldRef Name='ID' /></OrderBy></Query>";

                ListItemCollection listItemColl = li.GetItems(cq);
                context.Load(listItemColl);
                context.ExecuteQuery();

                foreach (ListItem lii in listItemColl)
                {
                    var spListItem = new SpTreeNode(lii.Id + "", lii);
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
