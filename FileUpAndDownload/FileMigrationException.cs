using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharezbold.FileMigration
{
    public class FileMigrationException : Exception
    {
        public FileMigrationException(string message, Exception e) : base(message, e)
        {
        }
    }
}
