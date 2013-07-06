//-----------------------------------------------------------------------
// <copyright file="UserManagementDownloader.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ElementsMigration.UserManagement
{
    using System;
    using System.Linq;
    using System.Net;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// This class is responsible for downloading information about the users and usergroups of the source SharePoint.
    /// </summary>
    internal class UserManagementDownloader
    {
        /// <summary>
        /// The ClientContext of the source SharePoint.
        /// </summary>
        private ClientContext clientContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagementDownloader"/> class.
        /// </summary>
        /// <param name="clientContext">The ClientContext of the source SharePoint.</param>
        public UserManagementDownloader(ClientContext clientContext)
        {
            this.clientContext = clientContext;
        }

        /// <summary>
        /// Returns all groups of SharePoint.
        /// </summary>
        /// <returns>all sharepoint groups</returns>
        /// <exception cref="UserMigrationException">If the groups could not be loaded.</exception>
        internal GroupCollection GetAllGroups()
        {
            Web web = this.clientContext.Web;
            GroupCollection groupCollection = web.SiteGroups;

            try
            {
                this.clientContext.Load(groupCollection);
                this.clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                throw new ElementsMigrationException("Could not load the groups from the source SharePoint.", e);
            }

            return groupCollection;
        }

        /// <summary>
        /// Returns all SiteUsers of source-SharePoint.
        /// </summary>
        /// <returns>all SiteUsers </returns>
        /// <exception cref="UserMigrationException">If the SiteUsers could not be loaded.</exception>
        internal UserCollection GetAllUsers()
        {
            Web web = this.clientContext.Web;
            UserCollection userCollection = web.SiteUsers;

            try
            {
                this.clientContext.Load(userCollection);
                this.clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                throw new ElementsMigrationException("Could not load the SiteUsers from the source SharePoint.", e);
            }

            return userCollection;
        }

    }
}
