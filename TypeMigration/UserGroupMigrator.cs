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
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// This class is responsible to migrate the Groups.
    /// </summary>
    internal class UserGroupMigrator : AbstractMigrator
    {/*
        /// <summary>
        /// ClientContext of source SharePoint.
        /// </summary>
        private ClientContext sourceClientContext;

        /// <summary>
        /// ClientContext of target SharePoint.
        /// </summary>
        private ClientContext targetClientContext;
        */
        /// <summary>
        /// Groups hold for adaption.
        /// </summary>
        private HashSet<string> groupsToAdapt;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupMigrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">clientcontext of source SharePoint</param>
        /// <param name="targetClientContext">clientcontext of target SharePoint</param>
        internal UserGroupMigrator(ClientContext sourceClientContext, ClientContext targetClientContext) : base(sourceClientContext, targetClientContext)
        {
            /*
            this.sourceClientContext = sourceClientContext;
            this.targetClientContext = targetClientContext;
            */
            this.groupsToAdapt = new HashSet<string>();
        }

        /// <summary>
        /// Migrates the missing UserGroups from the source SharePoint to the target SharePoint.
        /// </summary>
        public override void Migrate()
        {
            this.ImportNewGroups();
            this.AdaptGroups();
        }

        /// <summary>
        /// Imports the new groups.
        /// </summary>
        private void ImportNewGroups()
        {
            Console.WriteLine("Import new groups...");
            GroupCollection groupCollectionOnSourceServer = this.GetAllGroups(this.sourceClientContext);
            GroupCollection groupCollectoinOnTargetServer = this.GetAllGroups(this.targetClientContext);

            HashSet<string> titlesOfGroupsOnSourceServer = this.ReadAllTitles(groupCollectionOnSourceServer);

            foreach (var group in groupCollectionOnSourceServer)
            {
                if (!titlesOfGroupsOnSourceServer.Contains(group.Title))
                {
                    Console.WriteLine("import group '{0}'", group.Title);
                    GroupCreationInformation groupCreationInformation = new GroupCreationInformation();
                    groupCreationInformation.Description = group.Description;
                    groupCreationInformation.Title = group.Title;

                    groupCollectoinOnTargetServer.Add(groupCreationInformation);
                    this.groupsToAdapt.Add(group.Title);
                }
                else
                {
                    Console.WriteLine("don't have to migrate group with title '{0}'", group.Title);
                }
            }

            try
            {
                this.targetClientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during fetching the SiteGroups.", e);
                throw new ElementsMigrationException("Exception during fetching the SiteGroups.", e);
            }
        }

        /// <summary>
        /// Adapt the new groups.
        /// </summary>
        private void AdaptGroups()
        {
            Console.WriteLine("adapt new groups...");
            GroupCollection groupCollectionOnSourceServer = this.GetAllGroups(this.sourceClientContext);
            GroupCollection groupCollectoinOnTargetServer = this.GetAllGroups(this.targetClientContext);

            foreach (var title in this.groupsToAdapt)
            {
                Console.WriteLine("have to apadpt group '{0}'", title);
                Group sourceGroup = this.GetGroupByName(groupCollectionOnSourceServer, title);
                Group targetGroup = this.GetGroupByName(groupCollectoinOnTargetServer, title);

                targetGroup.AllowMembersEditMembership = sourceGroup.AllowMembersEditMembership;
                targetGroup.AllowRequestToJoinLeave = sourceGroup.AllowRequestToJoinLeave;
                targetGroup.AutoAcceptRequestToJoinLeave = sourceGroup.AutoAcceptRequestToJoinLeave;
            }

            try
            {
                this.targetClientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during adapting SiteGroups.", e);
                throw new ElementsMigrationException("Exception during adatping SiteGroups.", e);
            }
        }

        /// <summary>
        /// Returns the group with the given title (iterative search algorithm). 
        /// </summary>
        /// <param name="groupCollection">the groupcollection to search</param>
        /// <param name="title">title to search</param>
        /// <returns>found group with given title</returns>
        /// <exception cref="ElementsMigrationException">if group could not be found</exception>
        private Group GetGroupByName(GroupCollection groupCollection, string title)
        {
            foreach (var group in groupCollection)
            {
                if (group.Title == title)
                {
                    return group;
                }
            }

            throw new ElementsMigrationException("Could not find group for adaption! GroupTitle = " + title);
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
                throw new ElementsMigrationException("Exception during fetching the SiteGroups.", e);
            }

            return groupCollection;
        }

        /// <summary>
        /// Reads all titles out of given GroupCollection.
        /// </summary>
        /// <param name="groupCollection">GroupCollection to get the titles</param>
        /// <returns>read titles</returns>
        private HashSet<string> ReadAllTitles(GroupCollection groupCollection)
        {
            HashSet<string> titles = new HashSet<string>();

            foreach (var group in groupCollection)
            {
                titles.Add(group.Title);
            }

            return titles;
        }
    }
}
