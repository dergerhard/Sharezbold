

namespace Sharezbold.FileMigration.Host.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal static class StringValidator
    {
        internal static void IsNullOrEmpty(this string str)
        {
            if (str == null || str.Length == 0)
            {
                throw new ValidationException("string must not be null or empty");
            }
        }

        internal static void IsNumberic(this string str)
        {
            int number;
            if (!int.TryParse(str, out number))
            {
                throw new ValidationException("string is not a number");
            }
        }
    }
}
