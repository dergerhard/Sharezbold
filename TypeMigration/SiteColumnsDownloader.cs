using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint;

namespace TypeMigration
{
    internal class SiteColumnsDownloader
    {
        private ClientContext clientContext;

        public SiteColumnsDownloader(ClientContext clientContext)
        {
            this.clientContext = clientContext;
        }

    }
}
