//-----------------------------------------------------------------------
// <copyright file="FileMigrationBuilder.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using System;
    using Logging;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Builds the FileMigrator.
    /// </summary>
    public class FileMigrationBuilder
    {
        /// <summary>
        /// The FileMigrationSpecification.
        /// </summary>
        private FileMigrationSpecification specification;

        /// <summary>
        /// Prevents a default instance of the <see cref="FileMigrationBuilder" /> class from being created.
        /// </summary>
        private FileMigrationBuilder()
        {
            this.specification = new FileMigrationSpecification();
        }

        /// <summary>
        /// Returns an instance of the FileMigrationBuilder.
        /// </summary>
        /// <returns>instance of the FileMigrationBuilder</returns>
        public static FileMigrationBuilder GetNewFileMigrationBuilder()
        {
            return new FileMigrationBuilder();
        }

        /// <summary>
        /// Sets the source ClientContext.
        /// </summary>
        /// <param name="sourceClientContext">the source ClientContext</param>
        /// <returns>instance of the FileMigrationBuilder</returns>
        public FileMigrationBuilder WithSourceClientContext(ClientContext sourceClientContext)
        {
            this.specification.SourceClientContext = sourceClientContext;
            return this;
        }

        /// <summary>
        /// Sets the target ClientContext.
        /// </summary>
        /// <param name="targetClientContext">the target ClientContext</param>
        /// <returns>instance of the FileMigrationBuilder</returns>
        public FileMigrationBuilder WithTargetClientContext(ClientContext targetClientContext)
        {
            this.specification.TargetClientContext = targetClientContext;
            return this;
        }

        /// <summary>
        /// Sets the service address.
        /// </summary>
        /// <param name="serviceAddress">the service address</param>
        /// <returns>instance of the FileMigrationBuilder</returns>
        public FileMigrationBuilder WithServiceAddress(Uri serviceAddress)
        {
            this.specification.ServiceAddress = serviceAddress;
            return this;
        }

        /// <summary>
        /// Sets the bandwith.
        /// </summary>
        /// <param name="bandwith">the bandwith</param>
        /// <returns>instance of the FileMigrationBuilder</returns>
        public FileMigrationBuilder WithBandwith(int bandwith)
        {
            this.specification.Bandwith = bandwith;
            return this;
        }

        /// <summary>
        /// Sets the number of threads.
        /// </summary>
        /// <param name="number">the number of threads</param>
        /// <returns>instance of the FileMigrationBuilder</returns>
        public FileMigrationBuilder WithNumberOfThreads(int number)
        {
            this.specification.NumberOfThreads = number;
            return this;
        }

        /// <summary>
        /// Sets the logger.
        /// </summary>
        /// <param name="logger">the logger</param>
        /// <returns>instance of the FileMigrationBuilder</returns>
        public FileMigrationBuilder WithLogger(Logger logger)
        {
            this.specification.Logger = logger;
            return this;
        }

        /// <summary>
        /// Creates the migrator.
        /// </summary>
        /// <returns>instance of the migrator</returns>
        public SharePoint2010And2013Migrator CreateMigrator()
        {
            this.specification.Validate();

            return new SharePoint2010And2013Migrator(this.specification);
        }
    }
}
