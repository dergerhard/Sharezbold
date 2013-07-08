//-----------------------------------------------------------------------
// <copyright file="IElementsMigrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ElementsMigration
{
    /// <summary>
    /// Interface for migrate between different SharePoint-Versions.
    /// To use the concret SharePoint version use the class <see cref="Sharepoint2010And2013Migrator"/> for SharePoint 2010 and 2013.
    /// </summary>
    public interface IElementsMigrator
    {
        /// <summary>
        /// Migrates the ContentTypes.
        /// </summary>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        void MigrateContentTypes();

        /// <summary>
        /// Migrates the SiteColumns.
        /// </summary>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        void MigrateSiteColumns();

        /// <summary>
        /// Migrates the User.
        /// </summary>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        void MigrateUser();

        /// <summary>
        /// Migrates the Group.
        /// </summary>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        void MigrateGroup();

        /// <summary>
        /// Migrates the PermissionLevels (Role).
        /// </summary>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        void MigratePermissionlevels();

        /// <summary>
        /// Migrates the Workflow.
        /// </summary>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        void MigrateWorkflow();
    }
}
