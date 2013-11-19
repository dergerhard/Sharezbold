//-----------------------------------------------------------------------
// <copyright file="MigrationData.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ContentMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using FileMigration;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// The class MigrationData
    /// </summary>
    public class MigrationData
    {
        /// <summary>
        /// Gets or sets the file migrator
        /// </summary>
        public SharePoint2010And2013Migrator FileMigrator { get; set; }

        /// <summary>
        /// Gets or sets the WebUrlsToMigrate
        /// </summary>
        public HashSet<string> WebUrlsToMigrate { get; set; }

        /// <summary>
        /// Gets or sets the SourceClientContext
        /// </summary>
        public ClientContext SourceClientContext { get; set; }

        /// <summary>
        /// Gets or sets the TargetClientContext 
        /// </summary>
        public ClientContext TargetClientContext { get; set; }
    }
}
