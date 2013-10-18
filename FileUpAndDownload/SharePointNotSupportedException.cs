

namespace Sharezbold.FileMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SharePointNotSupportedException : Exception
    {
        public SharePointNotSupportedException(string message) : base(message)
        {
        }
    }
}
