//-----------------------------------------------------------------------
// <copyright file="UserManagementUploader.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ElementsMigration.UserManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// This class is responsible to upload the user datas.
    /// </summary>
    internal class UserManagementUploader
    {
        /// <summary>
        /// The ClientContext of the target SharePoint.
        /// </summary>
        private ClientContext clientContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagementMigration"/> class.
        /// </summary>
        /// <param name="context">The ClientContext of the target SharePoint.</param>
        internal UserManagementUploader(ClientContext context)
        {
            this.clientContext = context;
        }

        /// <summary>
        /// Uploads the Usergroups with the users in it to the target SharePoint.
        /// </summary>
        /// <param name="groupCollection">UserGroups to upload</param>
        /// <exception cref="UserMigrationExcepion">if the migration did not work</exception>
        internal void UploadUserGroups(GroupCollection groupCollection)
        {
            if (groupCollection == null || groupCollection.Count == 0)
            {
                Console.WriteLine("No Group-Collection to upload!");
            }

            GroupCollection groupCollectionOnServer = this.clientContext.Web.SiteGroups;

            try
            {
                this.clientContext.Load(groupCollectionOnServer);
                this.clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                throw new ElementsMigrationException("Could not load UserGroups from target-server.", e);
            }

            HashSet<string> groupNamesOnServer = ReadAllUserGroups(groupCollectionOnServer);

            foreach (Group group in groupCollection)
            {
                if (!groupNamesOnServer.Contains(group.LoginName))
                {
                    GroupCreationInformation groupCreationInformation = new GroupCreationInformation();
                    groupCreationInformation.Description = group.Description;
                    groupCreationInformation.Title = group.Title;

                    groupCollectionOnServer.Add(groupCreationInformation);
                }
            }

            try
            {
                this.clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not upload usergroups");
                throw new ElementsMigrationException("Could not upload usergroups.", e);
            }
        }

        /// <summary>
        /// Uploads the users to the target-sharepoint.
        /// </summary>
        /// <param name="users">users to upload</param>
        /// <exception cref="UserMigrationExcepion">if the migration did not work</exception>
        internal void UploadUsers(UserCollection users)
        {
            if (users == null || users.Count == 0)
            {
                Console.WriteLine("No users to upload");
            }

            UserCollection usersOnServer = this.clientContext.Web.SiteUsers;

            try
            {
                this.clientContext.Load(usersOnServer);
                this.clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                throw new ElementsMigrationException("Could not load users from target-server.", e);
            }

            HashSet<string> userNamesOnServer = ReadAllUsernames(usersOnServer);

            foreach (User user in users)
            {
                if (!userNamesOnServer.Contains(user.LoginName)) 
                {
                    UserCreationInformation userCreationInformation = new UserCreationInformation();
                    userCreationInformation.Email = user.Email;
                    userCreationInformation.LoginName = user.LoginName;
                    userCreationInformation.Title = user.Title;

                    usersOnServer.Add(userCreationInformation);
                }
            }

            try
            {
                this.clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not upload users.");
                throw new ElementsMigrationException("Could not upload users", e);
            }
        }

        private HashSet<string> ReadAllUsernames(UserCollection userCollection)
        {
            HashSet<string> names = new HashSet<string>();

            foreach (var user in userCollection)
            {
                names.Add(user.LoginName);
            }

            return names;
        }

        private HashSet<string> ReadAllUserGroups(GroupCollection groupCollection)
        {
            HashSet<string> names = new HashSet<string>();

            foreach (var group in groupCollection)
            {
                names.Add(group.LoginName);
            }

            return names;
        }
    }
}
