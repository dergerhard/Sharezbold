
namespace Sharezbold.FileMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    internal class FileMigrationSpecification
    {
        internal ClientContext SourceClientContext { get; set; }
        internal ClientContext TargetClientContext { get; set; }
        internal Uri ServiceAddress { get; set; }
        internal int Bandwith { get; set; }
        internal int MaxFileSize { get; set; }
        internal List<string> BlockedFileExtensions { get; set; }

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
