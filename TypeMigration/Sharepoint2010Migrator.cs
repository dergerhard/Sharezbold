//-----------------------------------------------------------------------
// <copyright file="Sharepoint2010Migrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.ElementsMigration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Migrator for Sharepoint 2010 to Sharepoint 2013 and Sharepoint 2013 to Sharepoint 2010.
    /// </summary>
    public class Sharepoint2010Migrator : IElementsMigrator
    {
        /// <summary>
        /// Holds the different migrator classes.
        /// </summary>
        private LinkedList<KeyValuePair<MigrationType, AbstractMigrator>> migrators;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sharepoint2010Migrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of the source SharePoint (2010)</param>
        /// <param name="targetClientContext">ClientContext of the target SharePoint (2013)</param>
        public Sharepoint2010Migrator(ClientContext sourceClientContext, ClientContext targetClientContext)
        {
            this.migrators = new LinkedList<KeyValuePair<MigrationType, AbstractMigrator>>();

            this.migrators.AddFirst(new KeyValuePair<MigrationType, AbstractMigrator>(MigrationType.SHAREPOINT2010_2013_CONTENT_TYPES, new ContentTypesMigrator(sourceClientContext, targetClientContext)));
            this.migrators.AddFirst(new KeyValuePair<MigrationType, AbstractMigrator>(MigrationType.SHAREPOINT2010_2013_GROUP, new UserGroupMigrator(sourceClientContext, targetClientContext)));
            this.migrators.AddFirst(new KeyValuePair<MigrationType, AbstractMigrator>(MigrationType.SHAREPOINT2010_2013_PERMISSION, new RoleMigrator(sourceClientContext, targetClientContext)));
            this.migrators.AddFirst(new KeyValuePair<MigrationType, AbstractMigrator>(MigrationType.SHAREPOINT2010_USER, new Sharepoint2010UserMigrator(sourceClientContext, targetClientContext)));
            this.migrators.AddFirst(new KeyValuePair<MigrationType, AbstractMigrator>(MigrationType.SHAREPOINT2010_2013_SITE_COLUMNS, new SiteColumsMigrator(sourceClientContext, targetClientContext)));
            this.migrators.AddFirst(new KeyValuePair<MigrationType, AbstractMigrator>(MigrationType.SHAREPOINT2010_2013_WORKFLOW, new WorkflowMigrator(sourceClientContext, targetClientContext)));
        }

        /// <summary>
        /// Migrates the ContentTypes.
        /// </summary>
        /// <returns>Log as LinkedList</returns>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        public LinkedList<string> MigrateContentTypes()
        {
            AbstractMigrator contentTypeMigrator = null;

            foreach (var migrator in this.migrators)
            {
                if (migrator.Key == MigrationType.SHAREPOINT2010_2013_CONTENT_TYPES)
                {
                    contentTypeMigrator = migrator.Value;
                    break;
                }
            }

            contentTypeMigrator.Migrate();
            return contentTypeMigrator.Log;
        }

        /// <summary>
        /// Migrates the User.
        /// </summary>
        /// <returns>Log as LinkedList</returns>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        public LinkedList<string> MigrateUser()
        {
            AbstractMigrator userMigrator = null;

            foreach (var migrator in this.migrators)
            {
                if (migrator.Key == MigrationType.SHAREPOINT2010_USER)
                {
                    userMigrator = migrator.Value;
                    break;
                }
            }

            userMigrator.Migrate();
            return userMigrator.Log;
        }

        /// <summary>
        /// Migrates the Groups.
        /// </summary>
        /// <returns>Log as LinkedList</returns>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        public LinkedList<string> MigrateGroup()
        {
            AbstractMigrator groupMigrator = null;

            foreach (var migrator in this.migrators)
            {
                if (migrator.Key == MigrationType.SHAREPOINT2010_2013_GROUP)
                {
                    groupMigrator = migrator.Value;
                    break;
                }
            }

            groupMigrator.Migrate();
            return groupMigrator.Log;
        }

        /// <summary>
        /// Migrates the PermissionLevels (Roles).
        /// </summary>
        /// <returns>Log as LinkedList</returns>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        public LinkedList<string> MigratePermissionlevels()
        {
            AbstractMigrator roleMigrator = null;

            foreach (var migrator in this.migrators)
            {
                if (migrator.Key == MigrationType.SHAREPOINT2010_2013_PERMISSION)
                {
                    roleMigrator = migrator.Value;
                    break;
                }
            }

            roleMigrator.Migrate();
            return roleMigrator.Log;
        }

        /// <summary>
        /// Migrates the SiteColumns.
        /// </summary>
        /// <returns>Log as LinkedList</returns>        
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        public LinkedList<string> MigrateSiteColumns()
        {
            AbstractMigrator siteColumsMigrator = null;

            foreach (var migrator in this.migrators)
            {
                if (migrator.Key == MigrationType.SHAREPOINT2010_2013_SITE_COLUMNS)
                {
                    siteColumsMigrator = migrator.Value;
                    break;
                }
            }

            siteColumsMigrator.Migrate();
            return siteColumsMigrator.Log;
        }

        /// <summary>
        /// Migrates the Workflow.
        /// </summary>
        /// <returns>Log as LinkedList</returns>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        public LinkedList<string> MigrateWorkflow()
        {
            AbstractMigrator workflowMigrator = null;

            foreach (var migrator in this.migrators)
            {
                if (migrator.Key == MigrationType.SHAREPOINT2010_2013_WORKFLOW)
                {
                    workflowMigrator = migrator.Value;
                    break;
                }
            }

            workflowMigrator.Migrate();
            return workflowMigrator.Log;
        }
    }
}
