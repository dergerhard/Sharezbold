using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharezbold.ContentMigration
{
    internal class ContentUploader
    {
        /// <summary>
        /// Source context of Sharepoint server
        /// </summary>
        private ClientContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentUploader"/> class.
        /// </summary>
        /// <param name="context">The ClientContext of the destination SharePoint.</param>
        public ContentUploader(ClientContext context)
        {
            this.context = context;
        }
    }
}
