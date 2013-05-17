using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace SharePointTutorial.PasswordAddingEventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class PasswordAddingEventReceiver : SPItemEventReceiver
    {
        /// <summary>
        /// An item is being added.
        /// </summary>
        public override void ItemAdding(SPItemEventProperties properties)
        {
            string password = (string) properties.AfterProperties["Password"];
            string cypher = (string) properties.List.ParentWeb.Properties["FHWN EncryptionKey"];

            properties.AfterProperties["Password"] = password.Encrypt(cypher);

            base.ItemAdding(properties);
        }


    }
}