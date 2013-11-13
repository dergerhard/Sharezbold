

namespace Sharezbold.FileMigration.Host
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using System.Threading.Tasks;
    internal class FileMigrationHostException : Exception
    {
        public FileMigrationHostException(string message) : base(message)
        {
        }
    }
}
