//-----------------------------------------------------------------------
// <copyright file="AbstractMigrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Abstract class for migrating files from source SharePoint to target SharePont.
    /// </summary>
    public abstract class AbstractMigrator
    {
        /// <summary>
        /// ClientContext of the source SharePoint.
        /// </summary>
        protected ClientContext sourceClientContext;

        /// <summary>
        /// ClientContext of the target SharePoint.
        /// </summary>
        protected ClientContext targetClientContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractMigrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of target SharePoint</param>
        public AbstractMigrator(ClientContext sourceClientContext, ClientContext targetClientContext)
        {
            this.sourceClientContext = sourceClientContext;
            this.targetClientContext = targetClientContext;
        }


        /// <summary>
        /// Migrate the file from the source-SharePoint to the target-SharePoint.
        /// </summary>
        /// <param name="documentListName">name of documentlist</param>
        /// <param name="documentName">name of document</param>
        /// <exception cref="FileMigrationException">if the migration of the file fails.</exception>
        public abstract void MigrateFile(string documentListName, string documentName);
    }
}
