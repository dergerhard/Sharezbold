
namespace Sharezbold.FileMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;
    using Logging;

    internal class FileMigrationSpecification
    {
        public FileMigrationSpecification()
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

        internal ClientContext SourceClientContext { get; set; }
        internal ClientContext TargetClientContext { get; set; }
        internal Uri ServiceAddress { get; set; }
        internal int Bandwith { get; set; }
        internal int NumberOfThreads { get; set; }
        internal int MaxFileSize { get; set; }
        internal List<string> BlockedFileExtensions { get; set; }
        internal Logger Logger { get; set; }

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
