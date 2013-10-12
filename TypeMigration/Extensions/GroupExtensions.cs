//-----------------------------------------------------------------------
// <copyright file="GroupExtensions.cs" company="FH Wiener Neustadt">
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
    /// Extensions of the Group.
    /// </summary>
    internal static class GroupExtensions
    {
        /// <summary>
        /// Returns all titles of given Group as HashSet.
        /// </summary>
        /// <param name="groups">Group to read th titles</param>
        /// <returns>titles of Group</returns>
        internal static HashSet<string> GetAllTitles(this GroupCollection groups)
        {
            HashSet<string> titles = new HashSet<string>();

            foreach (var roleDefinition in groups)
            {
                titles.Add(roleDefinition.Title);
            }

            return titles;
        }
    }
}
