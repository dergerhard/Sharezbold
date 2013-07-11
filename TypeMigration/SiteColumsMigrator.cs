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
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

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
        internal SiteColumsMigrator(ClientContext sourceClientContext, ClientContext targetClientContext) : base(sourceClientContext, targetClientContext)
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
            Log.AddLast("import new SiteColumns");
            FieldCollection sourceFieldCollection = this.GetAllFields(sourceClientContext);
            FieldCollection targetFieldCollection = this.GetAllFields(targetClientContext);

            HashSet<string> targetFieldTitles = this.GetTitlesOfFields(targetFieldCollection);

            foreach (var sourceField in sourceFieldCollection)
            {
                if (!targetFieldTitles.Contains(sourceField.Title))
                {
                    Log.AddLast("import new field = '" + sourceField.Title + "'");
                    string newField = "<Field DisplayName='" + sourceField.Title + "' Type='" + sourceField.TypeAsString + "' />";
                    targetFieldCollection.AddFieldAsXml(newField, true, AddFieldOptions.DefaultValue);
                }
                else
                {
                    Console.WriteLine("don't have to import '{0}'", sourceField.Title);
                    Log.AddLast("don't have to import '" + sourceField.Title + "'");
                }
            }

            try
            {
                targetClientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during importing new Fields.", e);
                Log.AddLast("Exception during importing new Fields. Error = " + e.Message);
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
                Log.AddLast("Exception during fetching new Field. Error = " + e.Message);
                throw new ElementsMigrationException("Exception during fetching new Field.", e);
            }

            return fieldCollection;
        }

        /// <summary>
        /// Gets the titles of the fields.
        /// </summary>
        /// <param name="fields">fields to read the titles</param>
        /// <returns>titles of the field</returns>
        private HashSet<string> GetTitlesOfFields(FieldCollection fields)
        {
            HashSet<string> titles = new HashSet<string>();

            foreach (var field in fields)
            {
                titles.Add(field.Title);
            }

            return titles;
        }
    }
}
