
namespace Sharezbold.FileMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;
    using Logging;

    public class FileMigrationBuilder
    {
        private FileMigrationSpecification specification;

        private FileMigrationBuilder()
        {
            this.specification = new FileMigrationSpecification();
        }

        public static FileMigrationBuilder GetNewFileMigrationBuilder()
        {
            return new FileMigrationBuilder();
        }

        public FileMigrationBuilder WithSourceClientContext(ClientContext sourceClientContext) {
            this.specification.SourceClientContext = sourceClientContext;
            return this;
        }

        public FileMigrationBuilder WithTargetClientContext(ClientContext targetClientContext) {
            this.specification.TargetClientContext = targetClientContext;
            return this;
        }

        public FileMigrationBuilder WithServiceAddress(Uri serviceAddress)
        {
            this.specification.ServiceAddress = serviceAddress;
            return this;
        }

        public FileMigrationBuilder WithBandwith(int bandwith)
        {
            this.specification.Bandwith = bandwith;
            return this;
        }

        public FileMigrationBuilder WithNumberOfThreads(int number)
        {
            this.specification.NumberOfThreads = number;
            return this;
        }

        public FileMigrationBuilder WithLogger(Logger logger)
        {
            this.specification.Logger = logger;
            return this;
        }

        public SharePoint2010And2013Migrator CreateMigrator()
        {
            this.specification.Validate();

            return new SharePoint2010And2013Migrator(this.specification);
        }
    }
}
