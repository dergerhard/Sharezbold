//-----------------------------------------------------------------------
// <copyright file="AbstractMigrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.ElementsMigration
{
    using System.Collections.Generic;
    using Logging;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Abstract class for elements migration.
    /// </summary>
    public abstract class AbstractMigrator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractMigrator"/> class.
        /// </summary>
        protected AbstractMigrator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractMigrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of target SharePoint</param>
        /// <param name="logger">Instance of the Logger</param>
        protected AbstractMigrator(ClientContext sourceClientContext, ClientContext targetClientContext, Logger logger)
        {
            this.SourceClientContext = sourceClientContext;
            this.TargetClientContext = targetClientContext;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets Holds the log of the migration.
        /// </summary>
        internal Logger Logger { get; private set; }

        /// <summary>
        /// Gets ClientContext of source SharePoint.
        /// </summary>
        internal ClientContext SourceClientContext { get; private set; }

        /// <summary>
        /// Gets ClientContext of target SharePoint.
        /// </summary>
        internal ClientContext TargetClientContext { get; private set; }

        /// <summary>
        /// Migrates the datas.
        /// </summary>
        /// <exception cref="ElementsMigrationException">if migration fails</exception>
        public abstract void Migrate();
    }
}
