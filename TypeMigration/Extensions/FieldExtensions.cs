//-----------------------------------------------------------------------
// <copyright file="FieldExtensions.cs" company="FH Wiener Neustadt">
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
    /// Holds the extensions for the Fields.
    /// </summary>
    internal static class FieldExtensions
    {
        /// <summary>
        /// Returns all titles of given Fields as HashSet.
        /// </summary>
        /// <param name="roleDefinitions">Fields to read the titles</param>
        /// <returns>titles of Field</returns>
        internal static HashSet<string> GetAllTitles(this FieldCollection fields)
        {
            HashSet<string> names = new HashSet<string>();

            foreach (var field in fields)
            {
                names.Add(field.Title);
            }

            return names;
        }
    }
}
