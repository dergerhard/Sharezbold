//-----------------------------------------------------------------------
// <copyright file="RoleExtensions.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.ElementsMigration.Extension
{
    using System;
    using System.Collections.Generic;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Holds the extensions for the Roles.
    /// </summary>
    internal static class RoleExtensions
    {
        /// <summary>
        /// Returns all names of given RoleDefinitions as HashSet.
        /// </summary>
        /// <param name="roleDefinitions">RoleDefinitions to read th names</param>
        /// <returns>names of RoleDefinitions</returns>
        internal static HashSet<string> ReadNames(this RoleDefinitionCollection roleDefinitions)
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
