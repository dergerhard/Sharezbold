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
    using Logging;

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

        private Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementsMigrationWorker"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of target SharePoint</param>
        /// <param name="logger">instance of the logger</param>
        internal ElementsMigrationWorker(ClientContext sourceClientContext, ClientContext targetClientContext, Logger logger)
        {
            this.sourceClientContext = sourceClientContext;
            this.targetClientContext = targetClientContext;
            this.logger = logger;
        }

        /// <summary>
        /// Delegates the migration-operation.
        /// </summary>
        /// <returns>log-output as LinkedList</returns>
        /// <exception cref="ElementsMigrationException">if migration fails</exception>
        private delegate void MigrationDelegation();

        /// <summary>
        /// Migrates the elements in a thread.
        /// </summary>
        /// <param name="migrateContentTypes">true if migrate ContentTypes</param>
        /// <param name="migrateUser">true if migrate User</param>
        /// <param name="migrateGroup">true if migrate Group</param>
        /// <param name="migrateSiteColumns">true if migrate SiteColumns</param>
        /// <param name="migratePermission">true if migrate PermissionLevel</param>
        /// <param name="migrateWorkflows">true if migrate Workflows</param>
        /// <returns>true when migration finished</returns>
        internal Task<bool> StartMigrationAsync(bool migrateContentTypes, bool migrateUser, bool migrateGroup, bool migrateSiteColumns, bool migratePermission, bool migrateWorkflows)
        {
            return Task.Run<bool>(() => this.StartMigration(migrateContentTypes, migrateUser, migrateGroup, migrateSiteColumns, migratePermission, migrateWorkflows));
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
        /// <returns>true when migration finished</returns>
        internal bool StartMigration(bool migrateContentTypes, bool migrateUser, bool migrateGroup, bool migrateSiteColumns, bool migratePermission, bool migrateWorkflows)
        {
            IElementsMigrator migrator = new Sharepoint2010Migrator(this.sourceClientContext, this.targetClientContext, this.logger);
            MigrationDelegation migrate = null;

            if (migrateContentTypes)
            {
                migrate = migrator.MigrateContentTypes;
                this.Migrate(migrate, "Content-Types");
            }

            if (migrateUser)
            {
                migrate = migrator.MigrateUser;
                this.Migrate(migrate, "User");
            }

            if (migrateGroup)
            {
                migrate = migrator.MigrateGroup;
                this.Migrate(migrate, "Group");
            }

            if (migratePermission)
            {
                migrate = migrator.MigratePermissionlevels;
                this.Migrate(migrate, "Permissionlevel");
            }

            if (migrateSiteColumns)
            {
                migrate = migrator.MigrateSiteColumns;
                this.Migrate(migrate, "SiteColumns");
            }

            if (migrateWorkflows)
            {
                migrate = migrator.MigrateWorkflow;
                this.Migrate(migrate, "Workflow");
            }

            return true;
        }

        /// <summary>
        /// Executes the delegation object to migrate the type.
        /// </summary>
        /// <param name="method">delegation method to execute</param>
        /// <param name="migrationType">type of migration for the log-output</param>
        private void Migrate(MigrationDelegation method, string migrationType)
        {
            try
            {
                this.logger.AddMessage("=============== START MIGRATION OF " + migrationType + " =================== \n\r");
                method();
            }
            catch (ElementsMigrationException e)
            {
                this.logger.AddMessage("ERROR during migrating " + migrationType + "\n\r");
                this.logger.AddMessage(e.Message + "\n\r");
                Console.WriteLine("ERROR during migrating {0}", migrationType);
                Console.WriteLine(e);
            }

            this.logger.AddMessage("=============== FINISHED MIGRATION OF " + migrationType + " =================== \n\r");
        }
    }
}
