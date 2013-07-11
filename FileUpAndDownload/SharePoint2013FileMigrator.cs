//-----------------------------------------------------------------------
// <copyright file="SharePoint2013FileMigrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    public class SharePoint2013FileMigrator : AbstractMigrator
    {
        private SharePoint2010And2013FileMigrator migrator;

        public SharePoint2013FileMigrator(ClientContext sourceClientContext, ClientContext targetClientContext)
            : base(sourceClientContext, targetClientContext)
        {
            this.migrator = new SharePoint2010And2013FileMigrator(sourceClientContext, targetClientContext);
        }

        public override void MigrateFile(string documentListName, string documentName)
        {
            this.migrator.MigrateFile(documentListName, documentName);
        }
    }
}
