//-----------------------------------------------------------------------
// <copyright file="MigrationFile.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Information about the file to migrate.
    /// </summary>
    internal class MigrationFile
    {
        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        internal File File { get; set; }

        /// <summary>
        /// Gets or sets the content of the file.
        /// </summary>
        internal byte[] Content { get; set; }
    }
}
