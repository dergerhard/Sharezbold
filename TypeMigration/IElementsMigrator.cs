namespace Sharezbold.ElementsMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IElementsMigrator
    {
        void MigrateContentTypes();
        void MigrateSiteColumns();
        void MigrateUser();
        void MigrateGroup();
        void MigratePermissionlevels();
        void MigrateWorkflow();
    }
}
