//-----------------------------------------------------------------------
// <copyright file="StringValidator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.FileMigration.Host.Extensions
{
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
