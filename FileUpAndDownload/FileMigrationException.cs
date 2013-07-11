//-----------------------------------------------------------------------
// <copyright file="FileMigrationException.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using System;

    /// <summary>
    /// Exception whe migration of files fails.
    /// </summary>
    public class FileMigrationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileMigrationException"/> class.
        /// </summary>
        /// <param name="message">Message of exception</param>
        /// <param name="e">cause-exception</param>
        public FileMigrationException(string message, Exception e) : base(message, e)
        {
        }
    }
}
