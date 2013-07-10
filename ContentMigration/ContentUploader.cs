//-----------------------------------------------------------------------
// <copyright file="ContentUploader.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ContentMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Responsible for uploading content
    /// </summary>
    internal class ContentUploader
    {
        /// <summary>
        /// Source context of share point server
        /// </summary>
        private ClientContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentUploader"/> class.
        /// </summary>
        /// <param name="context">The ClientContext of the destination SharePoint.</param>
        public ContentUploader(ClientContext context)
        {
            this.context = context;
        }
    }
}
