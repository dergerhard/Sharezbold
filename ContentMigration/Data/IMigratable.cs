

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
        /// Specifies whether to migrate the object or not
        /// </summary>
        bool Migrate
        {
            get;
            set;
        }

        /// <summary>
        /// Tells whether a object is configured to be migrated
        /// </summary>
        bool ReadyForMigration
        {
            get;
            set;
        }

        /// <summary>
        /// Name for the migration object
        /// </summary>
        string Name
        {
            get;
        }
    }
}
