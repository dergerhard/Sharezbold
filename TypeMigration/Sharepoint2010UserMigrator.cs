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
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
        internal Sharepoint2010UserMigrator(ClientContext sourceClientContext, ClientContext targetClientContext)
            : base(sourceClientContext, targetClientContext)
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
            Console.WriteLine("import new ContentTypes...");
            UserCollection sourceUserCollection = this.GetAllUser(sourceClientContext);
            UserCollection targetUserCollection = this.GetAllUser(targetClientContext);

            HashSet<string> targetUserNames = this.GetLoginNames(targetUserCollection);

            foreach (var user in sourceUserCollection)
            {
                if (!targetUserNames.Contains(user.LoginName))
                {
                    Console.WriteLine("Import user '{0}'", user.LoginName);
                    UserCreationInformation creationObject = new UserCreationInformation();
                    creationObject.Email = user.Email;
                    creationObject.LoginName = user.LoginName;
                    creationObject.Title = user.Title;

                    targetUserCollection.Add(creationObject);
                }
                else
                {
                    Console.WriteLine("user '{0}' is already on target server. nothing to import.", user.LoginName);
                }
            }

            try
            {
                targetClientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during importing the SiteUsers.", e);
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
        /// Returns the loginNames of the given UserCollection.
        /// </summary>
        /// <param name="userCollection">UserCollection with LoginNames</param>
        /// <returns>the loginNames</returns>
        private HashSet<string> GetLoginNames(UserCollection userCollection)
        {
            HashSet<string> loginNames = new HashSet<string>();

            foreach (var user in userCollection)
            {
                loginNames.Add(user.LoginName);
            }

            return loginNames;
        }
    }
}
