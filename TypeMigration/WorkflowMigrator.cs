//-----------------------------------------------------------------------
// <copyright file="WorkflowMigrator.cs" company="FH Wiener Neustadt">
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
    using Microsoft.SharePoint.Client.Workflow;

    internal class WorkflowMigrator : AbstractMigrator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowMigrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of target SharePoint</param>
        internal WorkflowMigrator(ClientContext sourceClientContext, ClientContext targetClientContext) : base(sourceClientContext, targetClientContext)
        {
        }

        public override void Migrate()
        {
            throw new NotImplementedException();
        }

        private void ImkportNewWorkflowTemplate()
        {
            Console.WriteLine("Import new WorkflowAssociations");
            WorkflowTemplateCollection sourceWorkflowTemplates = this.GetAllWorkflowTemplateCollection(sourceClientContext);
            WorkflowTemplateCollection targetWorkflowTemplates = this.GetAllWorkflowTemplateCollection(targetClientContext);

            foreach (var sourceWorkflowTemplate in sourceWorkflowTemplates)
            {
                
            }
        }

        private void ImportNewWorkflow()
        {
            Console.WriteLine("Import new WorkflowAssociations");
            WorkflowAssociationCollection sourceWorkflowAssociations = this.GetAllWorkflowAssociationCollection(this.sourceClientContext);
            WorkflowAssociationCollection targetWorkflowAssociations = this.GetAllWorkflowAssociationCollection(this.targetClientContext);

            foreach (var sourceWorkflowAssociation in sourceWorkflowAssociations)
            {
                WorkflowAssociationCreationInformation creationObject = new WorkflowAssociationCreationInformation();
                // creationObject.ContentTypeAssociationHistoryListName;
                // creationObject.HistoryList = sourceWorkflowAssociation.HistoryListTitle;
                creationObject.Name = sourceWorkflowAssociation.Name;
                
                // creationObject.TaskList = sourceWorkflowAssociation.Ta
                   // c
            }
        }

        private WorkflowAssociationCollection GetAllWorkflowAssociationCollection(ClientContext clientContext)
        {
            Web web = clientContext.Web;
            WorkflowAssociationCollection collection = web.WorkflowAssociations;

            try
            {
                clientContext.Load(collection);
                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during fetching the WorkflowAssociations.", e);
                throw new ElementsMigrationException("Exception during fetching the WorkflowAssociations.", e);
            }

            return collection;
        }

        private WorkflowTemplateCollection GetAllWorkflowTemplateCollection(ClientContext clientContext)
        {
            Web web = clientContext.Web;
            WorkflowTemplateCollection collection = web.WorkflowTemplates;

            try
            {
                clientContext.Load(collection);
                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during fetching the WorkflowTemplates.", e);
                throw new ElementsMigrationException("Exception during fetching the WorkflowTemplates.", e);
            }

            return collection;
        }
    }
}
