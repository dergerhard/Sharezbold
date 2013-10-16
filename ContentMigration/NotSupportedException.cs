//-----------------------------------------------------------------------
// <copyright file="LoadingElementsException.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ContentMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Exception to indicate that loading some elements failed
    /// </summary>
    public class NotSupportedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotSupportedException"/> class.
        /// </summary>
        /// <param name="message">The exception-message.</param>
        public NotSupportedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotSupportedException"/> class.
        /// </summary>
        /// <param name="message">The exception-message.</param>
        /// <param name="exception">The inner-exception.</param>
        public NotSupportedException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
