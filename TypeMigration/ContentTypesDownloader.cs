

namespace Sharezbold.ElementsMigration.ContentType
{
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    internal class ContentTypesDownloader
    {
        private ClientContext clientContext;

        internal ContentTypesDownloader(ClientContext clientContext)
        {
            this.clientContext = clientContext;
        }

        internal void UploadContentType(ContentTypeCollection contentTypeCollection)
        {
            if (contentTypeCollection == null || contentTypeCollection.Count == 0)
            {
                Console.WriteLine("No Content-Type-Collection to upload!");
                return;
            }

            ContentTypeCollection contentTypeCollectionOnServer = this.clientContext.Web.ContentTypes;

            try
            {
                this.clientContext.Load(contentTypeCollection);
                this.clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                throw new ElementsMigrationException("Could not upload ContentTypes to target-server.", e);
            }

            foreach (var contentType in contentTypeCollection)
            {
                if (!contentTypeCollectionOnServer.Contains<ContentType>(contentType))
                {
                    ContentTypeCreationInformation creationObject = new ContentTypeCreationInformation();
                    creationObject.Description = contentType.Description ;
                    creationObject.Group = contentType.Group ;
                    creationObject.Name = contentType.Name ;
                    //// TODO if content-Type parent is not on server --> add id
                    creationObject.ParentContentType = contentType.Parent;
                }
            }
        }
    }
}
