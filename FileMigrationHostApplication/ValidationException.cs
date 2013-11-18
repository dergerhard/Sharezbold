//-----------------------------------------------------------------------
// <copyright file="ValidationException.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration.Host
{
    using System;

    internal class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {}
    }
}
