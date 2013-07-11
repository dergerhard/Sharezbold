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
        /// Reference to the MainForm (UI).
        /// </summary>
        private MainForm mainForm;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementsMigrationWorker"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of target SharePoint</param>
        internal ElementsMigrationWorker(ClientContext sourceClientContext, ClientContext targetClientContext, MainForm mainForm)
        {
            this.sourceClientContext = sourceClientContext;
            this.targetClientContext = targetClientContext;
            this.mainForm = mainForm;
        }

        internal Task<bool> StartMigrationAsync(bool migrateContentTypes, bool migrateUser, bool migrateGroup, bool migrateSiteColumns, bool migratePermission, bool migrateWorkflows)
        {
            return Task.Run<bool>(() => StartMigration(migrateContentTypes, migrateUser, migrateGroup, migrateSiteColumns, migratePermission, migrateWorkflows));
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
        internal bool StartMigration(bool migrateContentTypes, bool migrateUser, bool migrateGroup, bool migrateSiteColumns, bool migratePermission, bool migrateWorkflows)
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
                this.mainForm.UpdateProgressLog("=============== START MIGRATION OF " + migrationType + " =================== \n\r");
                LinkedList<string> log = method();

                foreach (var logEntry in log)
                {
                    this.mainForm.UpdateProgressLog(logEntry);
                }
            }
            catch (ElementsMigrationException e)
            {
                this.mainForm.UpdateProgressLog("ERROR during migrating " + migrationType + "\n\r");
                this.mainForm.UpdateProgressLog(e.Message + "\n\r");
                Console.WriteLine("ERROR during migrating {0}", migrationType);
                Console.WriteLine(e);
            }

            this.mainForm.UpdateProgressLog("=============== FINISHED MIGRATION OF " + migrationType + " =================== \n\r");
        }

        private delegate LinkedList<string> DoMigration();
    }
}
