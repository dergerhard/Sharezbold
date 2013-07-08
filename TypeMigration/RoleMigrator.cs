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
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
        internal RoleMigrator(ClientContext sourceClientContext, ClientContext targetClientContext) : base(sourceClientContext, targetClientContext)
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
            RoleDefinitionCollection sourceRoleDefinitionCollection = this.GetAllRollDefinitions(sourceClientContext);
            RoleDefinitionCollection targetRoleDefinitionCollection = this.GetAllRollDefinitions(targetClientContext);

            HashSet<string> targetRoleDefinitionNames = this.ReadNames(targetRoleDefinitionCollection);

            foreach (var roleDefinition in sourceRoleDefinitionCollection)
            {
                if (!targetRoleDefinitionNames.Contains(roleDefinition.Name))
                {
                    Console.WriteLine("import roleDefinition '{0}'", roleDefinition.Name);

                    RoleDefinitionCreationInformation creationObject = new RoleDefinitionCreationInformation();
                    creationObject.BasePermissions = roleDefinition.BasePermissions;
                    creationObject.Description = roleDefinition.Description;
                    creationObject.Name = roleDefinition.Name;
                    creationObject.Order = roleDefinition.Order;

                    targetRoleDefinitionCollection.Add(creationObject);
                }
                else
                {
                    Console.WriteLine("don't have to import '{0}'", roleDefinition.Name);
                }
            }

            try
            {
                targetClientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during importing new RoleDefinition.", e);
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
                throw new ElementsMigrationException("Exception during fetching the RoleDefinitons.", e);
            }

            return roleDefinitions;
        }

        /// <summary>
        /// Returns all names of given RoleDefinitions as HashSet.
        /// </summary>
        /// <param name="roleDefinitions">RoleDefinitions to read th names</param>
        /// <returns>names of RoleDefinitions</returns>
        private HashSet<string> ReadNames(RoleDefinitionCollection roleDefinitions)
        {
            HashSet<string> names = new HashSet<string>();

            foreach (var roleDefinition in roleDefinitions)
            {
                names.Add(roleDefinition.Name);
            }

            return names;
        }
    }
}
