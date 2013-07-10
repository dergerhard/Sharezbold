//-----------------------------------------------------------------------
// <copyright file="ElementsMigrationWorker.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold
{
    using System;
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

            if (migrateContentTypes)
            {
                try
                {
                    migrator.MigrateContentTypes();
                }
                catch (ElementsMigrationException e)
                {
                    this.listBoxLog.Text = this.listBoxLog.Text + "\n" + "ERROR during migrating Content-Types:";
                    this.listBoxLog.Text += e.Message + "\n";
                    Console.WriteLine("ERROR during migrating Content-Types");
                    Console.WriteLine(e);
                    this.listBoxLog.Refresh();
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

            return true;
        }
    }
}
