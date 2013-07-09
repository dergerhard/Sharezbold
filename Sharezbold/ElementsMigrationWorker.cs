

namespace Sharezbold
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net;
    using Sharezbold.ElementsMigration;
    using Microsoft.SharePoint.Client;


    internal class ElementsMigrationWorker
    {
        //// TODO enum kind of sharepoint

        private ClientContext sourceClientContext;
        private ClientContext targetClientContext;

        /// <summary>
        /// 
        /// </summary>
        internal ElementsMigrationWorker(ClientContext sourceClientContext, ClientContext targetClientContext)
        {
            this.sourceClientContext = sourceClientContext;
            this.targetClientContext = targetClientContext;
        }

        internal void StartMigration(bool migrateContentTypes, bool migrateUser, bool migrateGroup, bool migrateSiteColumns, bool migratePermission, bool migrateWorkflows)
        {
            IElementsMigrator migrator = new Sharepoint2010Migrator(sourceClientContext, targetClientContext);

            if (migrateContentTypes)
            {
                try
                {
                    migrator.MigrateContentTypes();
                }
                catch (ElementsMigrationException e)
                {
                    Console.WriteLine("ERROR during migrating Content-Types");
                    Console.WriteLine(e);
                }
            }

            if (migrateUser)
            {
                try
                {
                    migrator.MigrateUser();
                }
                catch (ElementsMigrationException e)
                {
                    Console.WriteLine("ERROR during migrating User");
                    Console.WriteLine(e);
                }
            }

            if (migrateGroup)
            {
                try
                {
                    migrator.MigrateGroup();
                }
                catch (ElementsMigrationException e)
                {
                    Console.WriteLine("ERROR during migrating Group");
                    Console.WriteLine(e);
                }
            }

            if (migratePermission)
            {
                try
                {
                    migrator.MigratePermissionlevels();
                }
                catch (ElementsMigrationException e)
                {
                    Console.WriteLine("ERROR during migrating Permissionlevels");
                    Console.WriteLine(e);
                }
            }

            if (migrateSiteColumns)
            {
                try
                {
                    migrator.MigrateSiteColumns();
                }
                catch (ElementsMigrationException e)
                {
                    Console.WriteLine("ERROR during migrating SiteColumns:");
                    Console.WriteLine(e);
                }
            }

            if (migrateWorkflows)
            {
                try
                {
                    migrator.MigrateWorkflow();
                }
                catch (ElementsMigrationException e)
                {
                    Console.WriteLine("ERROR during migrating Workflow");
                    Console.WriteLine(e);
                }
            }
        }
    }
}
