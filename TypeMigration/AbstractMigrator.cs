//-----------------------------------------------------------------------
// <copyright file="AbstractMigrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.ElementsMigration
{
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Abstract class for elements migration.
    /// </summary>
    abstract class AbstractMigrator
    {
        /// <summary>
        /// ClientContext of source SharePoint.
        /// </summary>
        protected ClientContext sourceClientContext;

        /// <summary>
        /// ClientContext of target SharePoint.
        /// </summary>
        protected ClientContext targetClientContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractMigrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of target SharePoint</param>
        protected AbstractMigrator(ClientContext sourceClientContext, ClientContext targetClientContext)
        {
            this.sourceClientContext = sourceClientContext;
            this.targetClientContext = targetClientContext;
        }

        /// <summary>
        /// Migrates the datas.
        /// </summary>
        public abstract void Migrate();
    }
}
