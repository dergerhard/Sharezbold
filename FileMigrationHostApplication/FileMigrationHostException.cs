//-----------------------------------------------------------------------
// <copyright file="FileMigrationHostException.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration.Host
{
    using System;

    /// <summary>
    /// Exception for the migration process of files.
    /// </summary>
    internal class FileMigrationHostException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileMigrationHostException"/> class.
        /// </summary>
        /// <param name="message">message of the exception</param>
        public FileMigrationHostException(string message) : base(message)
        {
        }
    }
}
