//-----------------------------------------------------------------------
// <copyright file="FolderName.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.FileMigration
{
    /// <summary>
    /// Represents a constant class.
    /// </summary>
    internal class FolderName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderName"/> class.
        /// </summary>
        internal FolderName()
        {
            this.SharedDocumentsFoldername = "Shared Documents";
        }

        /// <summary>
        /// Gets the name of the shared documents folder.
        /// </summary>
        internal string SharedDocumentsFoldername { get; private set; }
    }
}
