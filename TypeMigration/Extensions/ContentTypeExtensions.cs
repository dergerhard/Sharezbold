//-----------------------------------------------------------------------
// <copyright file="ContentTypeExtensions.cs" company="FH Wiener Neustadt">
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
    /// Holds the extensions for the ContentTypes.
    /// </summary>
    internal static class ContentTypeExtensions
    {
        /// <summary>
        /// Extension to get the names of a ContentTypeCollection.
        /// </summary>
        /// <param name="collection">the ContentTypeCollection</param>
        /// <returns>names of ContentTypeCollection</returns>
        internal static HashSet<string> GetNames(this ContentTypeCollection collection)
        {
            HashSet<string> names = new HashSet<string>();

            foreach (ContentType contentType in collection)
            {
                names.Add(contentType.Name);
            }

            return names;
        }
    }
}
