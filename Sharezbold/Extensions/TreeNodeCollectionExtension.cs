//-----------------------------------------------------------------------
// <copyright file="TreeNodeCollectionExtension.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan</author>
//-----------------------------------------------------------------------
namespace Sharezbold.Extensions
{
    using System.Collections.Generic;
    using System.Windows.Forms;
    using System.Xml;
    using ContentMigration.Data;

    /// <summary>
    /// Extension class for the TreeNodeCollection.
    /// </summary>
    internal static class TreeNodeCollectionExtension
    {
        /// <summary>
        /// Returns the web urls of selected items in a TreeNodeCollection.
        /// </summary>
        /// <param name="selection">the TreeNodeCollection to check</param>
        /// <returns>selected web urls as string</returns>
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
                        //// is sitecollection is checked, add every page
                        foreach (SSite site in siteCollection.Sites)
                        {
                            XmlAttribute attr = site.XmlData.Attributes["Url"];
                            string url = attr.Value;
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
                                string url = attr.Value;
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
            }

            return webUrlList;
        }
    }
}
