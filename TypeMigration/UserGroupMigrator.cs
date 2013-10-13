//-----------------------------------------------------------------------
// <copyright file="UserGroupMigrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.ElementsMigration
{
    using System;
    using System.Collections.Generic;
    using Extension;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// This class is responsible to migrate the Groups.
    /// </summary>
    internal class UserGroupMigrator : AbstractMigrator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupMigrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">clientcontext of source SharePoint</param>
        /// <param name="targetClientContext">clientcontext of target SharePoint</param>
        internal UserGroupMigrator(ClientContext sourceClientContext, ClientContext targetClientContext)
            : base(sourceClientContext, targetClientContext)
        {
        }

        /// <summary>
        /// Migrates the missing UserGroups from the source SharePoint to the target SharePoint.
        /// </summary>
        public override void Migrate()
        {
            this.ImportNewGroups();
        }

        /// <summary>
        /// Imports the new groups.
        /// </summary>
        private void ImportNewGroups()
        {
            Console.WriteLine("Import new groups...");
            Log.AddLast("import new Groups...");
            GroupCollection groupCollectionOnSourceServer = this.GetAllGroups(this.SourceClientContext);
            GroupCollection groupCollectoinOnTargetServer = this.GetAllGroups(this.TargetClientContext);

            HashSet<string> titlesOfGroupsOnTargetServer = groupCollectoinOnTargetServer.GetAllTitles();

            foreach (var sourceGroup in groupCollectionOnSourceServer)
            {
                if (!titlesOfGroupsOnTargetServer.Contains(sourceGroup.Title))
                {
                    Console.WriteLine("import group '{0}'", sourceGroup.Title);
                    Log.AddLast("import group '" + sourceGroup.Title + "'");
                    GroupCreationInformation groupCreationInformation = new GroupCreationInformation();
                    groupCreationInformation.Description = sourceGroup.Description;
                    groupCreationInformation.Title = sourceGroup.Title;

                    Group targetGroup = groupCollectoinOnTargetServer.Add(groupCreationInformation);
                    targetGroup.AllowMembersEditMembership = sourceGroup.AllowMembersEditMembership;
                    targetGroup.AllowRequestToJoinLeave = sourceGroup.AllowRequestToJoinLeave;
                    targetGroup.AutoAcceptRequestToJoinLeave = sourceGroup.AutoAcceptRequestToJoinLeave;
                }
                else
                {
                    Console.WriteLine("don't have to migrate group with title '{0}'", sourceGroup.Title);
                    Log.AddLast("don't have to import group '" + sourceGroup.Title + "'");
                }
            }

            try
            {
                this.TargetClientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during importing new SiteGroups.", e);
                Log.AddLast("Exception during importing new SiteGroups. Error = " + e.Message);
                throw new ElementsMigrationException("Exception during importing new SiteGroups.", e);
            }
        }

        /// <summary>
        /// Returns all groups of given SharePoint-clientcontext.
        /// </summary>
        /// <param name="clientContext">clientcontext of SharePoint</param>
        /// <returns>all groups of sharepoint</returns>
        /// <exception cref="ElementsMigrationException">if group could not be loaded</exception>
        private GroupCollection GetAllGroups(ClientContext clientContext)
        {
            Web web = clientContext.Web;
            GroupCollection groupCollection = web.SiteGroups;

            try
            {
                clientContext.Load(groupCollection);
                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during fetching the SiteGroups.", e);
                Log.AddLast("Exception during fetching the SiteGroups. Error = " + e.Message);
                throw new ElementsMigrationException("Exception during fetching the SiteGroups.", e);
            }

            return groupCollection;
        }
    }
}
