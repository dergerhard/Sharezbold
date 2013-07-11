//-----------------------------------------------------------------------
// <copyright file="AbstractMigrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.ElementsMigration
{
    using System.Collections.Generic;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Abstract class for elements migration.
    /// </summary>
    abstract class AbstractMigrator
    {
        

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractMigrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of target SharePoint</param>
        protected AbstractMigrator(ClientContext sourceClientContext, ClientContext targetClientContext)
        {
            this.sourceClientContext = sourceClientContext;
            this.targetClientContext = targetClientContext;

            this.Log = new LinkedList<string>();
        }

        /// <summary>
        /// ClientContext of source SharePoint.
        /// </summary>
        internal ClientContext sourceClientContext { get; private set; }

        /// <summary>
        /// ClientContext of target SharePoint.
        /// </summary>
        internal ClientContext targetClientContext { get; private set; }

        protected AbstractMigrator()
        {
        }

        /// <summary>
        /// Migrates the datas.
        /// </summary>
        /// <exception cref="ElementsMigrationException">if migration fails</exception>
        public abstract void Migrate();

        /// <summary>
        /// Holds the log of the migration.
        /// </summary>
        internal LinkedList<string> Log { get; private set; }
    }
}
