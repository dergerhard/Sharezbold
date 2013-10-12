//-----------------------------------------------------------------------
// <copyright file="Sharepoint2013UserMigrator.cs" company="FH Wiener Neustadt">
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
    using Extension;

    /// <summary>
    /// Migrates the SiteUser from the source SharePoint to the target SharePoint.
    /// </summary>
    internal class Sharepoint2013UserMigrator : AbstractMigrator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sharepoint2013UserMigrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of target SharePoint</param>
        internal Sharepoint2013UserMigrator(ClientContext sourceClientContext, ClientContext targetClientContext) : base(sourceClientContext, targetClientContext)
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
            Log.AddLast("import new Users...");
            UserCollection sourceUserCollection = this.GetAllUser(sourceClientContext);
            UserCollection targetUserCollection = this.GetAllUser(targetClientContext);

            HashSet<string> targetUserNames = targetUserCollection.GetAllLoginNames();

            foreach (var sourceUser in sourceUserCollection)
            {
                if (!targetUserNames.Contains(sourceUser.LoginName))
                {
                    Console.WriteLine("Import user '{0}'", sourceUser.LoginName);
                    Log.AddLast("import User '" + sourceUser.LoginName + "'");

                    UserCreationInformation creationObject = new UserCreationInformation();
                    creationObject.Email = sourceUser.Email;
                    creationObject.LoginName = sourceUser.LoginName;
                    creationObject.Title = sourceUser.Title;

                    User targetUser = targetUserCollection.Add(creationObject);
                    targetUser.IsSiteAdmin = sourceUser.IsSiteAdmin;
                    targetUser.Tag = sourceUser.Tag;
                }
                else
                {
                    Console.WriteLine("user '{0}' is already on target server. nothing to import.", sourceUser.LoginName);
                    Log.AddLast("don't have to import user '" + sourceUser.LoginName + "'");
                }
            }

            try
            {
                targetClientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during importing the SiteUsers.", e);
                Log.AddLast("Exception during importing the Users. Error = " + e.Message);
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
            UserCollection userCollection = web.SiteUsers;

            try
            {
                clientContext.Load(userCollection);
                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during fetching the SiteUsers.", e);
                Log.AddLast("Exception during fetching the SiteUsers. Error = " + e.Message);
                throw new ElementsMigrationException("Exception during fetching the SiteUsers.", e);
            }

            return userCollection;
        }
    }
}
