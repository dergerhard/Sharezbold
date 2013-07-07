namespace Sharezbold.ElementsMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;
    using Sharezbold.ElementsMigration.UserManagement;
    using Sharezbold.ElementsMigration.ContentType;

    public class Sharepoint2013Migrator : IElementsMigrator
    {
        private ContentTypesDownloader contentTypesDownloader;
        private ContentTypesUploader contentTypesUploader;
        private UserManagementDownloader userManagementDownloader;
        private UserManagementUploader userManagementUploader;

        private AbstractMigrator userGroupMigrator;

        public Sharepoint2013Migrator(ClientContext clientContextSource, ClientContext clientContextTarget)
        {
            this.contentTypesDownloader = new ContentTypesDownloader(clientContextSource);
            this.userManagementDownloader = new UserManagementDownloader(clientContextSource);

            this.contentTypesUploader = new ContentTypesUploader(clientContextTarget);
            this.userManagementUploader = new UserManagementUploader(clientContextTarget);

            this.userGroupMigrator = new UserGroupMigrator(clientContextSource, clientContextTarget);
        }

        public void MigrateContentTypes()
        {
            this.contentTypesUploader.UploadContentType (this.contentTypesDownloader.GetAllContentTypes());
        }

        public void MigrateSiteColumns()
        {
            throw new NotImplementedException();
        }

        public void MigrateUser()
        {
            this.userManagementUploader.UploadUserGroups(this.userManagementDownloader.GetAllGroups());
            this.userManagementUploader.UploadUsers(this.userManagementDownloader.GetAllUsers());
        }

        public void MigrateGroup()
        {
            this.userGroupMigrator.Migrate();
        }

        public void MigratePermissionlevels()
        {
            throw new NotImplementedException();
        }

        public void MigrateWorkflow()
        {
            throw new NotImplementedException();
        }
    }
}
