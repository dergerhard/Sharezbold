namespace Sharezbold.UserManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Client;
    using Sharezbold.Util;

    public class UserManagementMigration
    {

        public void MigrateAllUserGroups(SharepointInformation sourceSharePoint, SharepointInformation targetSharePoint)
        {
            // TODO give the logic to an own method (better class):
            ClientContext contextSource = new ClientContext(sourceSharePoint.Address);
            contextSource.Credentials = sourceSharePoint.Credentials;

            Web web = contextSource.Web;
            GroupCollection groupCollection = web.SiteGroups;
            contextSource.Load(groupCollection);

            contextSource.ExecuteQuery();

            Console.WriteLine("count the gourps on the sharepoint = {0}", groupCollection.Count);

            for (int i = 0; i < groupCollection.Count; i++)
            {
                Group group = groupCollection.ElementAt(i);

                Console.WriteLine("group number {0}? = {1}", i, group.LoginName);
            }
        }

        public void MigrateAllUsers(SharepointInformation sourceSharePoint, SharepointInformation targetSharePoint)
        {
            // TODO give the logic to an own method (better class):
            ClientContext contextSource = new ClientContext(sourceSharePoint.Address);
            contextSource.Credentials = sourceSharePoint.Credentials;

            Web web = contextSource.Web;
            UserCollection userCollection = web.SiteUsers;
            contextSource.Load(userCollection);

            contextSource.ExecuteQuery();

            Console.WriteLine("count the users on the sharepoint = {0}", userCollection.Count);
            Console.WriteLine("are items available? = {0}", userCollection.AreItemsAvailable);


            for (int i = 0; i < userCollection.Count; i++)
            {
                User user = userCollection.ElementAt(i);

                Console.WriteLine("user number {0}? = {1}", i, user.LoginName);
            }
        }
    }
}
