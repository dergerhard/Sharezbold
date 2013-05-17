using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace SharePointTutorial.ItemUpdatedEventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class ItemUpdatedEventReceiver : SPItemEventReceiver
    {
        /// <summary>
        /// An item is being updated.
        /// </summary>
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            string passwordBeforeChanging = (string)properties.ListItem["Password"];
            string passwordAfterChanging = (string)properties.AfterProperties["Password"];
            string cypher = (string)properties.List.ParentWeb.Properties["FHWN EncryptionKey"];

            if (!passwordBeforeChanging.Equals(passwordAfterChanging) && !passwordBeforeChanging.Equals(passwordAfterChanging.Encrypt(cypher)))
            {

                properties.AfterProperties["Password"] = passwordAfterChanging.Encrypt(cypher);
            }
            else
            {
                properties.AfterProperties["Password"] = passwordBeforeChanging;
            }

            base.ItemUpdating(properties);
        }


    }
}