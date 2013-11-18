//-----------------------------------------------------------------------
// <copyright file="StringExtension.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ContentMigration.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// The class StringExtension
    /// </summary>
    internal static class StringExtension
    {
        /// <summary>
        /// Gets the relative url of the web
        /// </summary>
        /// <param name="url">the url</param>
        /// <param name="sourceClientContext">the source client context</param>
        /// <param name="targetClientContext">the traget client context</param>
        /// <returns>A dictionary of relative urls</returns>
        internal static Dictionary<Web, Web> GetRelativeUrlOfWeb(this string url, ClientContext sourceClientContext, ClientContext targetClientContext)
        {
            Web targetWeb;
            Web sourceWeb;

            string relativeUrl = url.Substring("http://".Length);

            if (relativeUrl.IndexOf("/") > 0)
            {
                relativeUrl = url.Substring(relativeUrl.IndexOf("/"));

                sourceWeb = sourceClientContext.Site.OpenWeb(relativeUrl);
                targetWeb = targetClientContext.Site.OpenWeb(relativeUrl);
            }
            else
            {
                sourceWeb = sourceClientContext.Web;
                targetWeb = targetClientContext.Web;
            }

            Dictionary<Web, Web> sourceTargetWebDictionary = new Dictionary<Web, Web>();
            sourceTargetWebDictionary.Add(sourceWeb, targetWeb);

            return sourceTargetWebDictionary;
        }
    }
}
