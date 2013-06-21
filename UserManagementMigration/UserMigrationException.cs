using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharezbold.UserManagement
{
    public class UserMigrationException : Exception
    {
        public UserMigrationException(string message) : base(message)
        {
        }

        public UserMigrationException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
