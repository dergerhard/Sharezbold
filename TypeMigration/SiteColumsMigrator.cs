//-----------------------------------------------------------------------
// <copyright file="SiteColumsMigrator.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.ElementsMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extension;
    using Microsoft.SharePoint.Client;
    using Logging;

    /// <summary>
    /// This class migrates the fields (SiteColumn).
    /// </summary>
    internal class SiteColumsMigrator : AbstractMigrator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SiteColumsMigrator"/> class.
        /// </summary>
        /// <param name="sourceClientContext">ClientContext of source SharePoint</param>
        /// <param name="targetClientContext">ClientContext of target SharePoint</param>
        internal SiteColumsMigrator(ClientContext sourceClientContext, ClientContext targetClientContext, Logger logger) : base(sourceClientContext, targetClientContext, logger)
        {
        }

        /// <summary>
        /// Migrate fields from source Sharepoint to target Sharepoint.
        /// </summary>
        /// <exception cref="ElementsMigrationException">if importing or fetcihng of fields fails</exception>
        public override void Migrate()
        {
            this.ImportNewField();
        }

        /// <summary>
        /// Import new fields to the target SharePoint.
        /// </summary>
        /// <exception cref="ElementsMigrationException">if importing of fields fails</exception>
        private void ImportNewField()
        {
            Console.WriteLine("import new fields");
            Logger.AddMessage("import new SiteColumns");
            FieldCollection sourceFieldCollection = this.GetAllFields(SourceClientContext);
            FieldCollection targetFieldCollection = this.GetAllFields(TargetClientContext);

            HashSet<string> targetFieldTitles = targetFieldCollection.GetAllTitles();

            foreach (var sourceField in sourceFieldCollection)
            {
                if (!targetFieldTitles.Contains(sourceField.Title))
                {
                    Logger.AddMessage("import new field = '" + sourceField.Title + "'");
                    string newField = "<Field DisplayName='" + sourceField.Title + "' Type='" + sourceField.TypeAsString + "' />";
                    Field targetField = targetFieldCollection.AddFieldAsXml(newField, true, AddFieldOptions.DefaultValue);
                    targetField.Description = sourceField.Description;
                    targetField.Direction = sourceField.Direction;
                    targetField.EnforceUniqueValues = sourceField.EnforceUniqueValues;
                    targetField.FieldTypeKind = sourceField.FieldTypeKind;
                    //// TODO getGroup: targetField.Group = sourceField.Group;
                    targetField.Hidden = sourceField.Hidden;
                    try
                    {
                        targetField.Indexed = sourceField.Indexed;
                    }
                    catch (PropertyOrFieldNotInitializedException)
                    { }
                    try
                    {
                        targetField.ReadOnlyField = sourceField.ReadOnlyField;
                    }
                    catch (PropertyOrFieldNotInitializedException)
                    { }
                    try
                    {
                        targetField.Required = sourceField.Required;
                    }
                    catch (PropertyOrFieldNotInitializedException)
                    { }
                    try
                    {
                        targetField.StaticName = sourceField.StaticName;
                    }
                    catch (PropertyOrFieldNotInitializedException)
                    { }

                    try
                    {
                        targetField.Tag = sourceField.Tag;
                    }
                    catch (PropertyOrFieldNotInitializedException)
                    { }

                    try
                    {
                        targetField.TypeAsString = sourceField.TypeAsString;
                    }
                    catch (PropertyOrFieldNotInitializedException)
                    { }

                    try
                    {
                        targetField.ValidationFormula = sourceField.ValidationFormula;
                    }
                    catch (PropertyOrFieldNotInitializedException)
                    { }

                    try
                    {
                        targetField.ValidationMessage = sourceField.ValidationMessage;
                    }
                    catch (PropertyOrFieldNotInitializedException)
                    { }
                }
                else
                {
                    Console.WriteLine("don't have to import '{0}'", sourceField.Title);
                    Logger.AddMessage("don't have to import '" + sourceField.Title + "'");
                }
            }

            try
            {
                TargetClientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during importing new Fields.", e);
                Logger.AddMessage("Exception during importing new Fields. Error = " + e.Message);
                throw new ElementsMigrationException("Exception during importing new Fields.", e);
            }
        }

        /// <summary>
        /// Get all fields of given SharePoint.
        /// </summary>
        /// <param name="clientContext">ClientContext of SharePoint</param>
        /// <returns>all fields</returns>
        /// <exception cref="ElementsMigrationException">if fetching of fields fails</exception>
        private FieldCollection GetAllFields(ClientContext clientContext)
        {
            Web web = clientContext.Web;
            FieldCollection fieldCollection = web.Fields;

            try
            {
                clientContext.Load(fieldCollection);
                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during fetching new Field.", e);
                Logger.AddMessage("Exception during fetching new Field. Error = " + e.Message);
                throw new ElementsMigrationException("Exception during fetching new Field.", e);
            }

            return fieldCollection;
        }
    }
}
