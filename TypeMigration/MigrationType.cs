//-----------------------------------------------------------------------
// <copyright file="MigrationType.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.ElementsMigration
{
    /// <summary>
    /// Holds the different migration types.
    /// </summary>
    internal enum MigrationType
    {
        /// <summary>
        /// For migrate the ContentTypes from MOSS 2007 to 2013.
        /// </summary>
        SHAREPOINT2007_CONTENT_TYPES,

        /// <summary>
        /// For migrate the User from MOSS 2007 to 2013.
        /// </summary>
        SHAREPOINT2007_USER,

        /// <summary>
        /// For migrate the Group from MOSS 2007 to 2013.
        /// </summary>
        SHAREPOINT2007_GROUP,

        /// <summary>
        /// For migrate the SiteColumns from MOSS 2007 to 2013.
        /// </summary>
        SHAREPOINT2007_SITE_COLUMNS,

        /// <summary>
        /// For migrate the Permission from MOSS 2007 to 2013.
        /// </summary>
        SHAREPOINT2007_PERMISSION,

        /// <summary>
        /// For migrate the Workflow from MOSS 2007 to 2013.
        /// </summary>
        SHAREPOINT2007_WORKFLOW,

        /// <summary>
        /// For migrate the ContentTypes from Sharepoint 2010 or 2013 to 2013.
        /// </summary>
        SHAREPOINT2010_2013_CONTENT_TYPES,

        /// <summary>
        /// For migrate the User from MOSS Sharepoint or 2013 to 2013.
        /// </summary>
        SHAREPOINT2010_USER,

        /// <summary>
        /// For migrate the User from MOSS Sharepoint or 2013 to 2013.
        /// </summary>
        SHAREPOINT2013_USER,

        /// <summary>
        /// For migrate the Group from MOSS Sharepoint or 2013 to 2013.
        /// </summary>
        SHAREPOINT2010_2013_GROUP,

        /// <summary>
        /// For migrate the SiteColumns from Sharepoint 2010 or 2013 to 2013.
        /// </summary>
        SHAREPOINT2010_2013_SITE_COLUMNS,

        /// <summary>
        /// For migrate the Permission from Sharepoint 2010 or 2013 to 2013.
        /// </summary>
        SHAREPOINT2010_2013_PERMISSION,

        /// <summary>
        /// For migrate the Workflow from Sharepoint 2010 or 2013 to 2013.
        /// </summary>
        SHAREPOINT2010_2013_WORKFLOW
    }
}
