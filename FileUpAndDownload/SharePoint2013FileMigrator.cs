﻿//-----------------------------------------------------------------------
// <copyright file="SharePoint2013FileMigrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using System;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Migrates the files from SharePoint 2013 to SharePoint 2013.
    /// </summary>
    public class SharePoint2013FileMigrator : AbstractMigrator
    {
        /// <summary>
        /// Instance of the real migrater.
        /// </summary>
        private SharePoint2010And2013FileMigrator migrator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePoint2013FileMigrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of target SharePoint</param>
        public SharePoint2013FileMigrator(ClientContext sourceClientContext, ClientContext targetClientContext)
            : base(sourceClientContext, targetClientContext)
        {
            this.migrator = new SharePoint2010And2013FileMigrator(sourceClientContext, targetClientContext);
        }

        /// <summary>
        /// Migrate the file from the source-SharePoint to the target-SharePoint.
        /// </summary>
        /// <param name="documentListName">name of documentlist</param>
        /// <param name="documentName">name of document</param>
        /// <exception cref="FileMigrationException">if the migration of the file fails.</exception>
        public override void MigrateFile(string documentListName, string documentName)
        {
            this.migrator.MigrateFile(documentListName, documentName);
        }
    }
}