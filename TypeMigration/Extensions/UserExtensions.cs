//-----------------------------------------------------------------------
// <copyright file="UserExtensions.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.ElementsMigration.Extension
{
    using System;
    using System.Collections.Generic;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Holds the extensions for the User.
    /// </summary>
    internal static class UserExtensions
    {
        /// <summary>
        /// Returns all login-names of given Users as HashSet.
        /// </summary>
        /// <param name="users">Users to read the login-names</param>
        /// <returns>login-names of Users</returns>
        internal static HashSet<string> GetAllLoginNames(this UserCollection users)
        {
            HashSet<string> names = new HashSet<string>();

            foreach (var user in users)
            {
                string username = user.LoginName;
                if (username.Contains(@"\"))
                {
                    username = username.Substring(username.IndexOf(@"\") + 1);
                }

                username.Trim('\\');
                names.Add(username);
            }

            return names;
        }
    }
}
