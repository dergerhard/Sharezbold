//-----------------------------------------------------------------------
// <copyright file="FileMigrationException.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration.Host
{
    using System;

    internal class FileMigrationHostException : Exception
    {
        public FileMigrationHostException(string message) : base(message)
        {
        }
    }
}
