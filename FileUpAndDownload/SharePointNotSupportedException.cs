//-----------------------------------------------------------------------
// <copyright file="SharePointNotSupportedException.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using System;

    /// <summary>
    /// Exception if SharePoint is not supported.
    /// </summary>
    public class SharePointNotSupportedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointNotSupportedException"/> class.
        /// </summary>
        /// <param name="message">Message of exception</param>
        public SharePointNotSupportedException(string message) : base(message)
        {
        }
    }
}
