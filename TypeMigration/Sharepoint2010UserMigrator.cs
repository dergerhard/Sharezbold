//-----------------------------------------------------------------------
// <copyright file="Sharepoint2010UserMigrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.ElementsMigration
{
    using System;
    using System.Collections.Generic;
    using Extension;
    using Logging;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Migrates the SiteUser from the source SharePoint to the target SharePoint.
    /// </summary>
    internal class Sharepoint2010UserMigrator : AbstractMigrator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sharepoint2010UserMigrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of target SharePoint</param>
        /// <param name="logger">instance of the Logger</param>
        internal Sharepoint2010UserMigrator(ClientContext sourceClientContext, ClientContext targetClientContext, Logger logger)
            : base(sourceClientContext, targetClientContext, logger)
        {
        }

        /// <summary>
        /// Migrates the SiteUsers.
        /// </summary>
        /// <exception cref="ElementsMigrationException">If the migration fails</exception>
        public override void Migrate()
        {
            this.ImportNewUsers();
        }

        /// <summary>
        /// Imports the new SiteUsers to the target SharePoint.
        /// </summary>
        /// <exception cref="ElementsMigrationException">If the migration fails</exception>
        private void ImportNewUsers()
        {
            Console.WriteLine("import new Users...");
            Logger.AddMessage("import new Users...");
            UserCollection sourceUserCollection = this.GetAllUser(SourceClientContext);
            UserCollection targetUserCollection = this.GetAllUser(TargetClientContext);

            HashSet<string> targetUserNames = targetUserCollection.GetAllLoginNames();

            foreach (var sourceUser in sourceUserCollection)
            {
                if (!targetUserNames.Contains(this.GetTheUsername(sourceUser.LoginName)))
                {
                    Console.WriteLine("Import user '{0}'", sourceUser.LoginName);
                    Logger.AddMessage("import user '" + sourceUser.LoginName + "'");
                    UserCreationInformation creationObject = new UserCreationInformation();
                    creationObject.Email = sourceUser.Email;
                    creationObject.LoginName = sourceUser.LoginName;
                    creationObject.Title = sourceUser.Title;

                    User targetUser = targetUserCollection.Add(creationObject);

                    try
                    {
                        TargetClientContext.ExecuteQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception during importing the SiteUsers.", e);
                        Logger.AddMessage("Exception during importing the Users. Error = " + e.Message);
                        throw new ElementsMigrationException("Exception during importing the SiteUsers.", e);
                    }

                    try
                    {
                        targetUser.IsSiteAdmin = sourceUser.IsSiteAdmin;
                    }
                    catch (PropertyOrFieldNotInitializedException)
                    {
                    }

                    try
                    {
                        targetUser.Tag = sourceUser.Tag;
                    }
                    catch (PropertyOrFieldNotInitializedException)
                    {
                    }
                }
                else
                {
                    Console.WriteLine("user '{0}' is already on target server. nothing to import.", sourceUser.LoginName);
                    Logger.AddMessage("don't have to import user '" + sourceUser.LoginName + "'");
                }
            }

            try
            {
                TargetClientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during importing the SiteUsers.", e);
                Logger.AddMessage("Exception during importing the Users. Error = " + e.Message);
                throw new ElementsMigrationException("Exception during importing the SiteUsers.", e);
            }
        }

        /// <summary>
        /// Get all users of the given SharePoint.
        /// </summary>
        /// <param name="clientContext">clientContext of SharePoint</param>
        /// <returns>all users of given SharePoint</returns>
        /// <exception cref="ElementsMigrationException">If the SiteUsers could not be fetched</exception>
        private UserCollection GetAllUser(ClientContext clientContext)
        {
            Web web = clientContext.Web;

            GroupCollection groups = web.SiteGroups;

            try
            {
                clientContext.Load(groups);
                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during fetching the Groups.", e);
                Logger.AddMessage("Exception during fetching the Group. Error = " + e.Message);
                throw new ElementsMigrationException("Exception during fetching the Groups.", e);
            }

            UserCollection userCollection = null;

            foreach (var group in groups)
            {
                try
                {
                    clientContext.Load(group.Users);
                    clientContext.ExecuteQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception during fetching the Users.", e);
                    Logger.AddMessage("Exception during fetching the users. Error = " + e.Message);
                    throw new ElementsMigrationException("Exception during fetching the Users.", e);
                }

                if (group.Users == null || group.Users.Count == 0)
                {
                    continue;
                }

                if (userCollection == null)
                {
                    userCollection = group.Users;
                }
                else
                {
                    foreach (var user in group.Users)
                    {
                        userCollection.AddUser(user);
                    }
                }
            }

            return userCollection;
        }

        /// <summary>
        /// Returns the username without the domain.
        /// </summary>
        /// <param name="loginname">the full loginname</param>
        /// <returns>username without domain</returns>
        private string GetTheUsername(string loginname)
        {
            string username = loginname;
            if (username.Contains(@"\"))
            {
                username = username.Substring(username.IndexOf(@"\") + 1);
                username.Trim('\\');
            }

            return username;
        }
    }
}
