//-----------------------------------------------------------------------
// <copyright file="ContentTypesMigrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.ElementsMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// This class migrates the ContentType from the source SharePoint to the target SharePoint.
    /// </summary>
    internal class ContentTypesMigrator : AbstractMigrator
    {
        /// <summary>
        /// Holds the contentTypes for adaption.
        /// </summary>
        private HashSet<string> contentTypesToAdapt;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTypesMigrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">clientContext of the source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of the target SharePoint</param>
        public ContentTypesMigrator(ClientContext sourceClientContext, ClientContext targetClientContext)
            : base(sourceClientContext, targetClientContext)
        {
            this.contentTypesToAdapt = new HashSet<string>();
        }

        /// <summary>
        /// Migrates the contentTypes.
        /// </summary>
        /// <exception cref="ElementsMigrationException">If migration fails</exception>
        public override void Migrate()
        {
            this.ImportNewContentTypes();
        }

        /// <summary>
        /// Imports the new ContentTypes.
        /// </summary>
        /// <exception cref="ElementsMigrationException">If migration fails</exception>
        private void ImportNewContentTypes()
        {
            Console.WriteLine("import new ContentTypes...");
            ContentTypeCollection contentTypeCollectionSourceServer = this.GetAllContentTypes(sourceClientContext);
            ContentTypeCollection contentTypeCollectionTargetServer = this.GetAllContentTypes(targetClientContext);

            HashSet<string> namesOfContentTypesOnTargetServer = this.ReadAllNames(contentTypeCollectionTargetServer);

            foreach (var contentType in contentTypeCollectionSourceServer)
            {
                if (!namesOfContentTypesOnTargetServer.Contains(contentType.Name))
                {
                    Console.WriteLine("import contentType = {0}", contentType.Name);

                    this.CreateContentType(contentTypeCollectionSourceServer, contentTypeCollectionTargetServer, contentType);
                }
                else
                {
                    Console.WriteLine("don't have to migrate '{0}'", contentType.Name);
                }
            }

            try
            {
                targetClientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during importing new ContentTypes.", e);
                throw new ElementsMigrationException("Exception during importing new ContentTypes.", e);
            }
        }

        /// <summary>
        /// Adds the parent
        /// </summary>
        /// <param name="sourceContentTypeCollection">ContentTypeCollection of the source SharePoint</param>
        /// <param name="targetContentTypeCollection">ContentTypeCollection of the target SharePoint</param>
        /// <param name="parentContentType">ContentType of parent</param>
        /// <returns>return parent ContentType or null, if there is no parent</returns>
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

            this.CreateContentType(sourceContentTypeCollection, targetContentTypeCollection, parentContentType);

            return this.GetContentTypeByName(targetContentTypeCollection, parentContentType.Name);
        }

        /// <summary>
        /// Creates a new ContentType.
        /// </summary>
        /// <param name="sourceContentTypeCollection">ContentTypeCollection of the source SharePoint</param>
        /// <param name="targetContentTypeCollection">ContentTypeCollection of the target SharePoint</param>
        /// <param name="sourceContentType">ContentType from source with information for the new one</param>
        private void CreateContentType(ContentTypeCollection sourceContentTypeCollection, ContentTypeCollection targetContentTypeCollection, Microsoft.SharePoint.Client.ContentType sourceContentType)
        {
            ContentTypeCreationInformation creationObject = new ContentTypeCreationInformation();
            creationObject.Description = sourceContentType.Description;
            creationObject.Group = sourceContentType.Group;
            creationObject.Name = sourceContentType.Name;
            creationObject.ParentContentType = this.AddParent(sourceContentTypeCollection, targetContentTypeCollection, sourceContentType.Parent);

            targetContentTypeCollection.Add(creationObject);
        }

        /// <summary>
        /// Returns all ContentTypes of given SharePoint server.
        /// </summary>
        /// <param name="clientContext">clientContext of Server</param>
        /// <returns>all ContentTypes</returns>
        /// <exception cref="ElementsMigrationException">If ContentTypes could not be fetched</exception>
        private ContentTypeCollection GetAllContentTypes(ClientContext clientContext)
        {
            Web web = clientContext.Web;
            ContentTypeCollection contentTypeCollection = web.ContentTypes;

            try
            {
                clientContext.Load(contentTypeCollection);
                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during fetching the ContentTypes.", e);
                throw new ElementsMigrationException("Exception during fetching the ContentTypes.", e);
            }

            return contentTypeCollection;
        }

        /// <summary>
        /// Read all names of ContentType.
        /// </summary>
        /// <param name="contentTypeCollection">Collection to get the ContentType</param>
        /// <returns>returns all names</returns>
        private HashSet<string> ReadAllNames(ContentTypeCollection contentTypeCollection)
        {
            HashSet<string> names = new HashSet<string>();

            foreach (var contentType in contentTypeCollection)
            {
                names.Add(contentType.Name);
            }

            return names;
        }

        /// <summary>
        /// Returns found ContentType for the given namen (linear search algorithm).
        /// </summary>
        /// <param name="contentTypeCollection">Collection of ContentType to find the necessary Content type</param>
        /// <param name="name">name to search</param>
        /// <returns>found Content type</returns>
        /// <exception cref="ElementsMigrationException">if no ContentType was found</exception>
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
