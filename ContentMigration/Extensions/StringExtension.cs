

namespace Sharezbold.ContentMigration.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    internal static class StringExtension
    {
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
