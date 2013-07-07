

namespace Sharezbold.ElementsMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    internal class ContentTypesMigrator : AbstractMigrator
    {

        private HashSet<string> contentTypesToAdapt;

        public ContentTypesMigrator(ClientContext sourceClientContext, ClientContext targetClientContext)
            : base(sourceClientContext, targetClientContext)
        {
            this.contentTypesToAdapt = new HashSet<string>();
        }

        public override void Migrate()
        {
            ImportNewContentTypes();
        }

        private void ImportNewContentTypes()
        {
            Console.WriteLine("import new ContentTypes...");
            ContentTypeCollection contentTypeCollectionSourceServer = this.GetAllContentTypes(base.sourceClientContext);
            ContentTypeCollection contentTypeCollectionTargetServer = this.GetAllContentTypes(base.targetClientContext);

            HashSet<string> namesOfContentTypesOnTargetServer = this.ReadAllNames(contentTypeCollectionTargetServer);

            foreach (var contentType in contentTypeCollectionSourceServer)
            {
                if (!namesOfContentTypesOnTargetServer.Contains(contentType.Name))
                {
                    Console.WriteLine("import contentType = {0}", contentType.Name);

                    CreateContentType(contentTypeCollectionSourceServer, contentTypeCollectionTargetServer, contentType);
                }
            }

            try
            {
                base.targetClientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during importing new ContentTypes.", e);
                throw new ElementsMigrationException("Exception during importing new ContentTypes.", e);
            }
        }

        private Microsoft.SharePoint.Client.ContentType AddParent(ContentTypeCollection sourceContentTypeCollection, ContentTypeCollection targetContentTypeCollection, Microsoft.SharePoint.Client.ContentType parentContentType)
        {
            Console.WriteLine("add parent ContentType...");

            if (parentContentType == null)
            {
                return null;
            }

            try
            {
                return this.GetContentTypeByName(targetContentTypeCollection, parentContentType.Name);
            }
            catch (ElementsMigrationException e)
            {
                Console.WriteLine("have to create parent content type");
            }


            CreateContentType(sourceContentTypeCollection, targetContentTypeCollection, parentContentType);

            return this.GetContentTypeByName(targetContentTypeCollection, parentContentType.Name);

        }

        private void CreateContentType(ContentTypeCollection sourceContentTypeCollection, ContentTypeCollection targetContentTypeCollection, Microsoft.SharePoint.Client.ContentType sourceContentType)
        {
            ContentTypeCreationInformation creationObject = new ContentTypeCreationInformation();
            creationObject.Description = sourceContentType.Description;
            creationObject.Group = sourceContentType.Group;
            creationObject.Name = sourceContentType.Name;
            creationObject.ParentContentType = AddParent(sourceContentTypeCollection, targetContentTypeCollection, sourceContentType.Parent);

            targetContentTypeCollection.Add(creationObject);

        }

        private ContentTypeCollection GetAllContentTypes(ClientContext clientContex)
        {
            Web web = clientContex.Web;
            ContentTypeCollection contentTypeCollection = web.ContentTypes;

            try
            {
                clientContex.Load(contentTypeCollection);
                clientContex.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during fetching the ContentTypes.", e);
                throw new ElementsMigrationException("Exception during fetching the ContentTypes.", e);
            }

            return contentTypeCollection;
        }

        private HashSet<string> ReadAllNames(ContentTypeCollection contentTypeCollection)
        {
            HashSet<string> names = new HashSet<string>();

            foreach (var contentType in contentTypeCollection)
            {
                names.Add(contentType.Name);
            }

            return names;
        }

        private Microsoft.SharePoint.Client.ContentType GetContentTypeByName(ContentTypeCollection contentTypeCollection, string name)
        {
            foreach (var contentType in contentTypeCollection)
            {
                if (contentType.Name == name)
                {
                    return contentType;
                }
            }

            throw new ElementsMigrationException("Could not find ContentType for adaption! Name = " + name);
        }
    }
}
