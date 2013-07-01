//-----------------------------------------------------------------------
// <copyright file="UserManagementMigration.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.UserManagement
{
    using System;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// This class is responsible for the user migration and group migration.
    /// It is also the interface for the other components.
    /// </summary>
    public class UserManagementMigration
    {
        /// <summary>
        /// The downloader of the source SharePoint.
        /// </summary>
        private UserManagementDownloader downloader;

        /// <summary>
        /// The uploader of the target SharePoint.
        /// </summary>
        private UserManagementUploader uploader;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagementMigration"/> class.
        /// </summary>
        /// <param name="sourceSharePoint">The ClientContext of the source SharePoint.</param>
        /// <param name="targetSharePoint">The ClientContext of the target SharePoint.</param>
        public UserManagementMigration(ClientContext sourceSharePoint, ClientContext targetSharePoint)
        {
            this.downloader = new UserManagementDownloader(sourceSharePoint);
            this.uploader = new UserManagementUploader(targetSharePoint);
        }

        /// <summary>
        /// Migrates the users and groups.
        /// </summary>
        /// <exception cref="UserMigrationException">if the users and/or the groups could not be migrated.</exception>
        public void MigrateUserManagement()
        {
            GroupCollection groupCollection = this.downloader.GetAllGroups();
            this.uploader.UploadUserGroups(groupCollection);
        }
    }
}
