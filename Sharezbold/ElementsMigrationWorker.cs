

namespace Sharezbold.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Sharezbold.ElementsMigration;
    using Microsoft.SharePoint.Client;

    internal class ElementsMigrationWorker
    {
        //// TODO enum kind of sharepoint
        //// TODO SharePoint-contextes

        /// <summary>
        /// 
        /// </summary>
        internal ElementsMigrationWorker()
        {

        }


        internal void StartMigration(bool migrateContentTypes, bool migrateUser, bool migrateGroup, bool migrateSiteColumns, bool migratePermission, bool migrateWorkflows)
        {
            ////IElementsMigrator migrator = new Sharepoint2013Migrator();
        }
    }
}
