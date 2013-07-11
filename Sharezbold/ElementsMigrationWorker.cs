//-----------------------------------------------------------------------
// <copyright file="ElementsMigrationWorker.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using ElementsMigration;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// This class is the interface to the DLL "TypeMigration" and migrates the elements TypeContent, User, Group, Permission, Workflow and SiteColumns. 
    /// </summary>
    internal class ElementsMigrationWorker
    {
        //// TODO enum kind of sharepoint

        /// <summary>
        /// The ClientContext of the source SharePoint.
        /// </summary>
        private ClientContext sourceClientContext;

        /// <summary>
        /// The ClientContext of the target SharePoint.
        /// </summary>
        private ClientContext targetClientContext;

        /// <summary>
        /// The ListBox for log-output.
        /// </summary>
        private ListBox listBoxLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementsMigrationWorker"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of target SharePoint</param>
        internal ElementsMigrationWorker(ClientContext sourceClientContext, ClientContext targetClientContext, ListBox listBox)
        {
            this.sourceClientContext = sourceClientContext;
            this.targetClientContext = targetClientContext;
            this.listBoxLog = listBox;
        }

        /// <summary>
        /// Start the migration.
        /// </summary>
        /// <param name="migrateContentTypes">true if migrate ContentTypes</param>
        /// <param name="migrateUser">true if migrate User</param>
        /// <param name="migrateGroup">true if migrate Group</param>
        /// <param name="migrateSiteColumns">true if migrate SiteColumns</param>
        /// <param name="migratePermission">true if migrate PermissionLevel</param>
        /// <param name="migrateWorkflows">true if migrate Workflows</param>
        internal bool StartMigrationAsync(bool migrateContentTypes, bool migrateUser, bool migrateGroup, bool migrateSiteColumns, bool migratePermission, bool migrateWorkflows)
        //internal Task<bool> StartMigrationAsync(bool migrateContentTypes, bool migrateUser, bool migrateGroup, bool migrateSiteColumns, bool migratePermission, bool migrateWorkflows)
        {
            IElementsMigrator migrator = new Sharepoint2010Migrator(this.sourceClientContext, this.targetClientContext);
            DoMigration migrate = null;

            if (migrateContentTypes)
            {
                migrate = migrator.MigrateContentTypes;
                Migrate(migrate, "Content-Types");
            }

            if (migrateUser)
            {
                migrate = migrator.MigrateUser;
                Migrate(migrate, "User");
            }

            if (migrateGroup)
            {
                migrate = migrator.MigrateGroup;
                Migrate(migrate, "Group");
            }

            if (migratePermission)
            {
                migrate = migrator.MigratePermissionlevels;
                Migrate(migrate, "Permissionlevel");
            }

            if (migrateSiteColumns)
            {
                migrate = migrator.MigrateSiteColumns;
                Migrate(migrate, "SiteColumns");

            }

            if (migrateWorkflows)
            {
                migrate = migrator.MigrateWorkflow;
                Migrate(migrate, "Workflow");
            }

            return true;
        }

        private void Migrate(DoMigration method, string migrationType)
        {
            try
            {
                this.listBoxLog.Items.Add("=============== START MIGRATION OF " + migrationType + " =================== \n\r");
                this.listBoxLog.Update();
                LinkedList<string> log = method();

                foreach (var logEntry in log)
                {
                    this.listBoxLog.Items.Add(logEntry);
                }
                
                this.listBoxLog.Update();
            }
            catch (ElementsMigrationException e)
            {
                this.listBoxLog.Items.Add("ERROR during migrating " + migrationType + "\n\r");
                this.listBoxLog.Items.Add(e.Message + "\n\r");
                this.listBoxLog.Update();
                Console.WriteLine("ERROR during migrating {0}", migrationType);
                Console.WriteLine(e);
                this.listBoxLog.Update();
            }

            this.listBoxLog.Items.Add("=============== FINISHED MIGRATION OF " + migrationType + " =================== \n\r");
        }

        private delegate LinkedList<string> DoMigration();
    }
}
