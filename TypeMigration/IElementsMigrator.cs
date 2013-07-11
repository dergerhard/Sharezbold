//-----------------------------------------------------------------------
// <copyright file="IElementsMigrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ElementsMigration
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for migrate between different SharePoint-Versions.
    /// To use the concret SharePoint version use the class <see cref="Sharepoint2013Migrator"/> for SharePoint 2010 and 2013.
    /// </summary>
    public interface IElementsMigrator
    {
        /// <summary>
        /// Migrates the ContentTypes.
        /// </summary>
        /// <returns>Log as LinkedList</returns>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        LinkedList<string> MigrateContentTypes();

        /// <summary>
        /// Migrates the SiteColumns.
        /// </summary>
        /// <returns>Log as LinkedList</returns>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        LinkedList<string> MigrateSiteColumns();

        /// <summary>
        /// Migrates the User.
        /// </summary>
        /// <returns>Log as LinkedList</returns>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        LinkedList<string> MigrateUser();

        /// <summary>
        /// Migrates the Group.
        /// </summary>
        /// <returns>Log as LinkedList</returns>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        LinkedList<string> MigrateGroup();

        /// <summary>
        /// Migrates the PermissionLevels (Role).
        /// </summary>
        /// <returns>Log as LinkedList</returns>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        LinkedList<string> MigratePermissionlevels();

        /// <summary>
        /// Migrates the Workflow.
        /// </summary>
        /// <returns>Log as LinkedList</returns>
        /// <exception cref="ElementsMigrationException">If the migration fails.</exception>
        LinkedList<string> MigrateWorkflow();
    }
}
