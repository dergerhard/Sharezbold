
namespace Sharezbold.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Xml;
    using Microsoft.SharePoint.Client;
    using ContentMigration.Data;

    internal static class TreeNodeCollectionExtension
    {
        internal static HashSet<string> GetSelectedWebUrls(this TreeNodeCollection selection)
        {
            HashSet<string> webUrlList = new HashSet<string>();
            foreach (TreeNode item in selection)
            {
                IMigratable migratable = (IMigratable)item;
                if (migratable.GetType() == typeof(SSiteCollection))
                {
                    SSiteCollection siteCollection = (SSiteCollection)migratable;
                    if (siteCollection.Checked)
                    {
                        // is sitecollection is checked, add every page
                        foreach (SSite site in siteCollection.Sites)
                        {
                            XmlAttribute attr = site.XmlData.Attributes["Url"];
                            String url = attr.Value;
                            if (url != null)
                            {
                                webUrlList.Add(url);
                            }
                        }
                    }
                    else
                    {
                        foreach (SSite site in siteCollection.Sites)
                        {
                            if (site.Checked)
                            {
                                XmlAttribute attr = site.XmlData.Attributes["Url"];
                                String url = attr.Value;
                                if (url != null)
                                {
                                    webUrlList.Add(url);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //// other items are either site and could be in set already or they are list, which is not important for this query.
                }
                /*
                else if (migratable.GetType() == typeof(SSite))
                {
                    SSite site = (SSite)migratable;
                    if (site.Checked)
                    {
                        XmlAttribute attr = site.XmlData.Attributes["Url"];
                        String url = attr.Value;
                        if (url != null)
                        {
                            webUrlList.Add(url);
                        }
                    }
                }
                else if (migratable.GetType() == typeof(SList))
                {
                   // it's not a web!
                }
                 * */
                ////  Console.WriteLine("'{0}' is ready to migrate: {1}; type = {2}", migratable.Name, migratable.ReadyForMigration, migratable.GetType());
            }

            return webUrlList;
        }
    }
}
