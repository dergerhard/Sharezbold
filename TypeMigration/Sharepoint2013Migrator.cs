namespace Sharezbold.ElementsMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    public class Sharepoint2013Migrator : IElementsMigrator
    {
        private AbstractMigrator userGroupMigrator;

        public Sharepoint2013Migrator(ClientContext clientContextSource, ClientContext clientContextTarget)
        {
        }

        public void MigrateContentTypes()
        {
            throw new NotImplementedException();
        }

        public void MigrateSiteColumns()
        {
            throw new NotImplementedException();
        }

        public void MigrateUser()
        {
            throw new NotImplementedException();
        }

        public void MigrateGroup()
        {
            this.userGroupMigrator.Migrate();
        }

        public void MigratePermissionlevels()
        {
            ////TODO role-migration
            throw new NotImplementedException();
        }

        public void MigrateWorkflow()
        {
            throw new NotImplementedException();
        }
    }
}
