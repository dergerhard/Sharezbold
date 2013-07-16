﻿//-----------------------------------------------------------------------
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
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Workflow;
    using Extension;

    /// <summary>
    /// Migrates the workflows from SharePoint 2010 and 2013 to SharePoint 2013.
    /// </summary>
    internal class WorkflowMigrator : AbstractMigrator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowMigrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of target SharePoint</param>
        internal WorkflowMigrator(ClientContext sourceClientContext, ClientContext targetClientContext)
            : base(sourceClientContext, targetClientContext)
        {
        }

        /// <summary>
        /// Migrates the datas.
        /// </summary>
        /// <exception cref="ElementsMigrationException">if migration fails</exception>
        public override void Migrate()
        {
            this.ImportNewWorkflow();
        }

        /// <summary>
        /// Imports new workflows to the target SharePoint.
        /// </summary>
        /// <exception cref="ElementsMigrationException">if migration fails</exception>
        private void ImportNewWorkflow()
        {
            Console.WriteLine("Import new WorkflowAssociations");
            WorkflowAssociationCollection sourceWorkflowAssociations = this.GetAllWorkflowAssociationCollection(this.sourceClientContext);
            WorkflowAssociationCollection targetWorkflowAssociations = this.GetAllWorkflowAssociationCollection(this.targetClientContext);

            if (sourceWorkflowAssociations.Count == 0)
            {
                Log.AddLast("no workflows to migrate...");
                return;
            }

            HashSet<string> targetWorkflowNames = targetWorkflowAssociations.GetAllNames();

            foreach (var sourceWorkflowAssociation in sourceWorkflowAssociations)
            {
                if (!targetWorkflowNames.Contains(sourceWorkflowAssociation.Name))
                {
                    Console.WriteLine("import new workflow '{0}'", sourceWorkflowAssociation.Name);
                    Log.AddLast("import new workflow '" + sourceWorkflowAssociation.Name + "'");

                    WorkflowAssociationCreationInformation creationObject = new WorkflowAssociationCreationInformation();
                    creationObject.Name = sourceWorkflowAssociation.Name;
                    creationObject.HistoryList = this.GetHistoryList(this.sourceClientContext);
                    creationObject.TaskList = this.GetTaskList(this.sourceClientContext);
                    creationObject.Template = this.GetTemplate();

                    WorkflowAssociation targetWorkflowAssociation = targetWorkflowAssociations.Add(creationObject);

                    targetWorkflowAssociation.AllowManual = sourceWorkflowAssociation.AllowManual;
                    targetWorkflowAssociation.AutoStartChange = sourceWorkflowAssociation.AutoStartChange;
                    targetWorkflowAssociation.AutoStartCreate = sourceWorkflowAssociation.AutoStartCreate;
                    targetWorkflowAssociation.Enabled = sourceWorkflowAssociation.Enabled;
                    targetWorkflowAssociation.AssociationData = sourceWorkflowAssociation.AssociationData;
                }
                else
                {
                    Console.WriteLine("don't have to migrate workflow '{0}'", sourceWorkflowAssociation.Name);
                    Log.AddLast("don't have to migrate workflow '" + sourceWorkflowAssociation.Name + "'");
                }
            }

            try
            {
                this.targetClientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during importing new Workflows.", e);
                Log.AddLast("Exception during importing new Workflows. Error = " + e.Message);
                throw new ElementsMigrationException("Exception during importing new Workflows.", e);
            }
        }

        /// <summary>
        /// Returns all WorkflowAssociationCollection of the given SharePoint.
        /// </summary>
        /// <param name="clientContext">ClientContext of SharePoint</param>
        /// <returns>WorkflowAssociationCollection of given SharePoint</returns>
        /// <exception cref="ElementsMigrationException">if fetching WorkflowAssociationCollection fails</exception>
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

        /// <summary>
        /// Returns the Historylist of the given SharePoint.
        /// </summary>
        /// <param name="clientContext">ClientContext of the SharePoint</param>
        /// <returns>historylist of clientContext or null if there is none</returns>
        private List GetHistoryList(ClientContext clientContext)
        {
            Web web = clientContext.Web;
            List historyList = web.Lists.GetByTitle("Workflow History");

            try
            {
                clientContext.Load(historyList);
                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during fetching the history of the Workflow.", e);
                Log.AddLast("Exception during fetching the history of the Workflow. Error = " + e.Message);
                Log.AddLast("using null history-list now...");

                return null;
            }

            return historyList;
        }

        /// <summary>
        /// Gets the tasklist of the workflows.
        /// </summary>
        /// <param name="clientContext">ClientContext of SharePoint</param>
        /// <returns>tasklist or null if there is none.</returns>
        private List GetTaskList(ClientContext clientContext)
        {
            Web web = clientContext.Web;
            List taskList = web.Lists.GetByTitle("Tasks");

            try
            {
                clientContext.Load(taskList);
                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during fetching the tasks of the Workflow.", e);
                Log.AddLast("Exception during fetching the tasks of the Workflow. Error = " + e.Message);
                Log.AddLast("using null task-list now...");

                return null;
            }

            return taskList;
        }

        /// <summary>
        /// Gets the WorkflowTemplate
        /// </summary>
        /// <returns>the workflowTemplate</returns>
        /// <exception cref="ElementsMigrationException">if there is no workflowTemplate</exception>
        private WorkflowTemplate GetTemplate()
        {
            WorkflowTemplateCollection sourceWorkflowTemplateCollection = sourceClientContext.Web.WorkflowTemplates;
            WorkflowTemplateCollection targetWorkflowTemplateCollection = targetClientContext.Web.WorkflowTemplates;

            try
            {
                sourceClientContext.ExecuteQuery();
                targetClientContext.ExecuteQuery();

                if (targetWorkflowTemplateCollection == null || targetWorkflowTemplateCollection.Count == 0)
                {
                    Log.AddLast("No templates for Workflow found!");
                    throw new ElementsMigrationException("No templates for Workflow found!");
                }

                if (sourceWorkflowTemplateCollection == null || sourceWorkflowTemplateCollection.Count == 0)
                {
                    //// nothing to search return target->first
                    return targetWorkflowTemplateCollection.First();
                }

                foreach (var targetWorkflowTemplate in targetWorkflowTemplateCollection)
                {
                    foreach (var sourceWorkflowTemplate in sourceWorkflowTemplateCollection)
                    {
                        if (targetWorkflowTemplate.Name == sourceWorkflowTemplate.Name)
                        {
                            return targetWorkflowTemplate;
                        }
                    }
                }

                Log.AddLast("No templates for Workflow found!");
                throw new ElementsMigrationException("No templates for Workflow found!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during fetching the template of the Workflow.", e);
                Log.AddLast("Exception during fetching the template of the Workflow. Error = " + e.Message);

                throw new ElementsMigrationException("Exception during fetching the WorkflowTemplates.", e);
            }
        }
    }
}
