//-----------------------------------------------------------------------
// <copyright file="FileMigrationSpecification.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.FileMigration
{
    using System;
    using System.Collections.Generic;
    using Logging;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Holds the information about the migration specification.
    /// </summary>
    internal class FileMigrationSpecification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileMigrationSpecification"/> class.
        /// </summary>
        internal FileMigrationSpecification()
        {
            this.SourceClientContext = null;
            this.TargetClientContext = null;
            this.ServiceAddress = null;
            this.Bandwith = 100;
            this.NumberOfThreads = 1;
            this.BlockedFileExtensions = null;
            this.MaxFileSize = int.MaxValue;
            this.Logger = null;
        }

        /// <summary>
        /// Gets or sets the source client context.
        /// </summary>
        internal ClientContext SourceClientContext { get; set; }

        /// <summary>
        /// Gets or sets the target client context.
        /// </summary>
        internal ClientContext TargetClientContext { get; set; }

        /// <summary>
        /// Gets or sets the service address.
        /// </summary>
        internal Uri ServiceAddress { get; set; }

        /// <summary>
        /// Gets or sets the bandwith.
        /// </summary>
        internal int Bandwith { get; set; }

        /// <summary>
        /// Gets or sets the number of threads.
        /// </summary>
        internal int NumberOfThreads { get; set; }

        /// <summary>
        /// Gets or sets the max file size.
        /// </summary>
        internal int MaxFileSize { get; set; }

        /// <summary>
        /// Gets or sets the blocked file extensions.
        /// </summary>
        internal List<string> BlockedFileExtensions { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        internal Logger Logger { get; set; }

        /// <summary>
        /// Validates the specifications.
        /// </summary>
        /// <exception cref="ValidationException">if any type is not valid</exception>
        internal void Validate()
        {
            if (this.SourceClientContext == null)
            {
                throw new ValidationException("ClientContext of the source SharePoint must not be null!");
            }

            if (this.TargetClientContext == null)
            {
                throw new ValidationException("ClientContext of the target SharePoint must not be null!");
            }

            if (this.ServiceAddress == null)
            {
                throw new ValidationException("Service address must not be null!");
            }
        }
    }
}
