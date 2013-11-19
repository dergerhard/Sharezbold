//-----------------------------------------------------------------------
// <copyright file="StringValidator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.FileMigration.Host.Extensions
{
    /// <summary>
    /// Extension for the strings to validate them.
    /// </summary>
    internal static class StringValidator
    {
        /// <summary>
        /// Validates, if string is null or empty.
        /// </summary>
        /// <param name="str">string to validate</param>
        internal static void IsNullOrEmpty(this string str)
        {
            if (str == null || str.Length == 0)
            {
                throw new ValidationException("string must not be null or empty");
            }
        }

        /// <summary>
        /// Validates if string is numeric.
        /// </summary>
        /// <param name="str">string to validate</param>
        internal static void IsNumeric(this string str)
        {
            int number;
            if (!int.TryParse(str, out number))
            {
                throw new ValidationException("string is not a number");
            }
        }
    }
}
