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

        internal ContentTypeCollection GetAllContentTypes()
        {
            Console.WriteLine("Get all content types");
            Web web = this.clientContext.Web;
            ContentTypeCollection contentTypeCollection = web.ContentTypes;

            try
            {
                this.clientContext.Load(contentTypeCollection);
                this.clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                throw new ElementsMigrationException("Could not load the groups from the source SharePoint.", e);
            }

            return contentTypeCollection;
        }

    }
}
