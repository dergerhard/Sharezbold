//-----------------------------------------------------------------------
// <copyright file="Validator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration
{
    using System;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// This class is responsible for validation.
    /// </summary>
    internal class Validator
    {
        /// <summary>
        /// Validates, if the web exists.
        /// </summary>
        /// <param name="clientContext">current ClientContext</param>
        /// <param name="web">Web to check</param>
        /// <exception cref="ValidationException">if web does not exist</exception>
        internal static void ValidateIfWebExists(ClientContext clientContext, Web web)
        {
            try
            {
                clientContext.Load(web);
                clientContext.ExecuteQuery();
            }
            catch (Exception)
            {
                throw new ValidationException("Can't connect to the web '" + web.Title + "'.");
            }
        }
    }
}
