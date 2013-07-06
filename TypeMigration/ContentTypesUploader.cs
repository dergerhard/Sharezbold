

namespace Sharezbold.ElementsMigration.ContentType
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    internal class ContentTypesUploader
    {
        private ClientContext clientContext;

        public ContentTypesUploader(ClientContext clientContext)
        {
            this.clientContext = clientContext;
        }

        internal void UploadContentType(ContentTypeCollection contentTypeCollection)
        {
            Console.WriteLine("upload contenttypes to the target-sharepoint");

            if (contentTypeCollection == null || contentTypeCollection.Count == 0)
            {
                Console.WriteLine("No Content-Type-Collection to upload!");
                return;
            }

            ContentTypeCollection contentTypeCollectionOnServer = this.clientContext.Web.ContentTypes;

            try
            {
                Console.WriteLine("Get ContentTypes from targetserver");
                this.clientContext.Load(contentTypeCollectionOnServer);
                this.clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                throw new ElementsMigrationException("Could not upload ContentTypes to target-server.", e);
            }


            foreach (var contentType in contentTypeCollection)
            {
                Console.WriteLine("next contentType = {0}", contentType.Name); 
                //// TODO compare names
                if (!contentTypeCollectionOnServer.Contains<ContentType>(contentType))
                {
                    Console.WriteLine("upload contentType = {0}", contentType.Name);
                    
                    ContentTypeCreationInformation creationObject = new ContentTypeCreationInformation();
                    creationObject.Description = contentType.Description;
                    creationObject.Group = contentType.Group;
                    creationObject.Name = contentType.Name;
                    //// TODO if content-Type parent is not on server --> add id
                    creationObject.ParentContentType = null;
                    // creationObject.ParentContentType = contentType.Parent;
                    

                    //contentTypeCollectionOnServer.AddExistingContentType(contentType);
                    contentTypeCollectionOnServer.Add(creationObject);  
                }
            }

            this.clientContext.ExecuteQuery();
        }
    }
}
