//-----------------------------------------------------------------------
// <copyright file="UserMigrationException.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.UserManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// If the migration of users or the groups did not work.
    /// </summary>
    public class UserMigrationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserMigrationException"/> class.
        /// </summary>
        /// <param name="message">The exception-message.</param>
        public UserMigrationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMigrationException"/> class.
        /// </summary>
        /// <param name="message">The exception-message.</param>
        /// <param name="exception">The inner-exception.</param>
        public UserMigrationException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
