//-----------------------------------------------------------------------
// <copyright file="RoleMigrator.cs" company="FH Wiener Neustadt">
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
    /// This class migrates the RoleDefintion from the source SharePoint to the target SharePoint.
    /// </summary>
    internal class RoleMigrator : AbstractMigrator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleMigrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">clientContext of the source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of the target SharePoint</param>
        /// <param name="logger">instance of the Logger</param>
        internal RoleMigrator(ClientContext sourceClientContext, ClientContext targetClientContext, Logger logger) : base(sourceClientContext, targetClientContext, logger)
        {
        }

        /// <summary>
        /// Migrates the RoleDefinitions.
        /// </summary>
        /// <exception cref="ElementsMigrationException">If migration fails</exception>
        public override void Migrate()
        {
            this.ImportNewRoleDefinitions();
        }

        /// <summary>
        /// Imports the new RoleDefinitions.
        /// </summary>
        /// <exception cref="ElementsMigrationException">If migration fails</exception>
        private void ImportNewRoleDefinitions()
        {
            Console.WriteLine("import new RoleDefinitions...");
            Logger.AddMessage("import new RoleDefinitions...");
            RoleDefinitionCollection sourceRoleDefinitionCollection = this.GetAllRollDefinitions(SourceClientContext);
            RoleDefinitionCollection targetRoleDefinitionCollection = this.GetAllRollDefinitions(TargetClientContext);

            HashSet<string> targetRoleDefinitionNames = targetRoleDefinitionCollection.ReadNames();

            foreach (var sourceRoleDefinition in sourceRoleDefinitionCollection)
            {
                if (!targetRoleDefinitionNames.Contains(sourceRoleDefinition.Name))
                {
                    Console.WriteLine("import roleDefinition '{0}'", sourceRoleDefinition.Name);
                    Logger.AddMessage("import RoleDefinition '" + sourceRoleDefinition.Name + "'");

                    RoleDefinitionCreationInformation creationObject = new RoleDefinitionCreationInformation();
                    creationObject.BasePermissions = sourceRoleDefinition.BasePermissions;
                    creationObject.Description = sourceRoleDefinition.Description;
                    creationObject.Name = sourceRoleDefinition.Name;
                    creationObject.Order = sourceRoleDefinition.Order;

                    RoleDefinition targetRoleDefinition = targetRoleDefinitionCollection.Add(creationObject);
                    targetRoleDefinition.Tag = sourceRoleDefinition.Tag;
                }
                else
                {
                    Console.WriteLine("don't have to import '{0}'", sourceRoleDefinition.Name);
                    Logger.AddMessage("don't have to import '" + sourceRoleDefinition.Name + "'");
                }
            }

            try
            {
                TargetClientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during importing new RoleDefinition.", e);
                Logger.AddMessage("Exception during importing new RoleDefinition. Error = " + e.Message);
                throw new ElementsMigrationException("Exception during importing new RoleDefinition.", e);
            }
        }

        /// <summary>
        /// Returns all RoleDefinition of given SharePoint.
        /// </summary>
        /// <param name="clientContext">ClientContext of SharePoint</param>
        /// <returns>RoleDefinitions of given SharePoint</returns>
        /// <exception cref="ElementsMigrationException">If RoleDefinitions could not be fetched</exception>
        private RoleDefinitionCollection GetAllRollDefinitions(ClientContext clientContext)
        {
            Web web = clientContext.Web;
            RoleDefinitionCollection roleDefinitions = web.RoleDefinitions;

            try
            {
                clientContext.Load(roleDefinitions);
                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during fetching the RoleDefinitons.", e);
                Logger.AddMessage("Exception during fetching the RoleDefinitons. Error = " + e.Message);
                throw new ElementsMigrationException("Exception during fetching the RoleDefinitons.", e);
            }

            return roleDefinitions;
        }
    }
}
