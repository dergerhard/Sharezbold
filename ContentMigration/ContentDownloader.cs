using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharezbold;

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
            SpTreeNode root = new SpTreeNode(context.ApplicationName);


            return root;
        }
    }
}
