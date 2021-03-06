﻿//-----------------------------------------------------------------------
// <copyright file="WorkflowExtensions.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.ElementsMigration.Extension
{
    using System;
    using System.Collections.Generic;
    using Microsoft.SharePoint.Client.Workflow;

    /// <summary>
    /// Holds the extensions for the Workflow.
    /// </summary>
    internal static class WorkflowExtensions
    {
        /// <summary>
        /// Returns all login-names of given Users as HashSet.
        /// </summary>
        /// <param name="workflowAssociationCollection">the collection of WorkflowAssociatoin</param>
        /// <returns>login-names of Users</returns>
        internal static HashSet<string> GetAllNames(this WorkflowAssociationCollection workflowAssociationCollection)
        {
            HashSet<string> names = new HashSet<string>();

            foreach (var workflowAssociation in workflowAssociationCollection)
            {
                names.Add(workflowAssociation.Name);
            }

            return names;
        }
    }
}
