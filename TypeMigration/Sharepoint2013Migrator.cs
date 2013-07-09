//-----------------------------------------------------------------------
// <copyright file="Sharepoint2013Migrator.cs" company="FH Wiener Neustadt">
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
    public class Sharepoint2013Migrator : IElementsMigrator
    {
        /// <summary>
        /// Holds the different migrator classes.
        /// </summary>
        private LinkedList<KeyValuePair<MigrationType, AbstractMigrator>> migrators;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sharepoint2013Migrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of the source SharePoint (2010/2013)</param>
        /// <param name="targetClientContext">ClientContext of the target SharePoint (2013)</param>
        public Sharepoint2013Migrator(ClientContext sourceClientContext, ClientContext targetClientContext)
        {
            this.migrators = new LinkedList<KeyValuePair<MigrationType, AbstractMigrator>>();

            this.migrators.AddFirst(new KeyValuePair<MigrationType, AbstractMigrator>(MigrationType.SHAREPOINT2010_2013_CONTENT_TYPES, new ContentTypesMigrator(sourceClientContext, targetClientContext)));
            this.migrators.AddFirst(new KeyValuePair<MigrationType, AbstractMigrator>(MigrationType.SHAREPOINT2010_2013_GROUP, new UserGroupMigrator(sourceClientContext, targetClientContext)));
            this.migrators.AddFirst(new KeyValuePair<MigrationType, AbstractMigrator>(MigrationType.SHAREPOINT2010_2013_PERMISSION, new RoleMigrator(sourceClientContext, targetClientContext)));
            this.migrators.AddFirst(new KeyValuePair<MigrationType, AbstractMigrator>(MigrationType.SHAREPOINT2013_USER, new Sharepoint2013UserMigrator(sourceClientContext, targetClientContext)));
            this.migrators.AddFirst(new KeyValuePair<MigrationType, AbstractMigrator>(MigrationType.SHAREPOINT2010_2013_SITE_COLUMNS, new SiteColumsMigrator(sourceClientContext, targetClientContext)));
            //// migrators.AddFirst(new KeyValuePair<MigrationType, AbstractMigrator>(MigrationType.SHAREPOINT2010_2013_WORKFLOW, new ContentTypesMigrator(clientContextSource, clientContextTarget)));
        }

        /// <summary>
        /// Migrates the ContentTypes.
        /// </summary>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        public void MigrateContentTypes()
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
        }

        /// <summary>
        /// Migrates the User.
        /// </summary>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        public void MigrateUser()
        {
            AbstractMigrator userMigrator = null;

            foreach (var migrator in this.migrators)
            {
                if (migrator.Key == MigrationType.SHAREPOINT2013_USER)
                {
                    userMigrator = migrator.Value;
                    break;
                }
            }

            userMigrator.Migrate();
        }

        /// <summary>
        /// Migrates the Groups.
        /// </summary>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        public void MigrateGroup()
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
        }

        /// <summary>
        /// Migrates the PermissionLevels (Roles).
        /// </summary>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        public void MigratePermissionlevels()
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
        }

        /// <summary>
        /// Migrates the SiteColumns.
        /// </summary>
        /// <exception cref="NotImplementedException">Not implemented till now.</exception>
        public void MigrateSiteColumns()
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
        }

        /// <summary>
        /// Migrates the Workflow.
        /// </summary>
        /// <exception cref="NotImplementedException">Not implemented till now.</exception>
        public void MigrateWorkflow()
        {
            throw new NotImplementedException();
        }
    }
}
