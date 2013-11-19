//-----------------------------------------------------------------------
// <copyright file="IMigratable.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ContentMigration.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    /// <summary>
    /// Used for migratable objects
    /// </summary>
    public interface IMigratable
    {
        /// <summary>
        /// Gets or sets a value indicating whether to migrate the object or not
        /// </summary>
        bool Migrate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object is configured to be migrated
        /// </summary>
        bool ReadyForMigration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name for the migration object
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets or sets the parent object
        /// </summary>
        IMigratable ParentObject { get; set; }
    }
}
