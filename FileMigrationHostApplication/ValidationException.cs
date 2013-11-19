//-----------------------------------------------------------------------
// <copyright file="ValidationException.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration.Host
{
    using System;

    /// <summary>
    /// Exception if the validation fails.
    /// </summary>
    internal class ValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="message">message of the exception</param>
        public ValidationException(string message) : base(message)
        {
        }
    }
}
