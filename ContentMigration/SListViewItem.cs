//-----------------------------------------------------------------------
// <copyright file="SListViewItem.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ContentMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Microsoft.SharePoint.Client;
    using Sharezbold.ContentMigration;
    using Sharezbold.ContentMigration.Data;

    /// <summary>
    /// Represents a ListViewItem with a migration object
    /// </summary>
    public class SListViewItem : ListViewItem
    {
        /// <summary>
        /// Represents a list view item with a migration object
        /// </summary>
        /// <param name="migrationObject">Represents the migration object</param>
        public SListViewItem(IMigratable  migrationObject)
            : base(migrationObject.Name, migrationObject.ReadyForMigration ? 1 : 0)
        {
            this.MigrationObject = migrationObject;

            if (this.MigrationObject is SSite)
            {
                this.Text = "          " + this.Text;
            }
            else if (this.MigrationObject is SList)
            {
                this.Text = "                    " + this.Text;
            }
        }

        /// <summary>
        /// Gets or sets the migration object
        /// </summary>
        public IMigratable MigrationObject
        {
            get;
            set;
        }

        /// <summary>
        /// Updates the image of the list item. Red if not ready, green if ready
        /// </summary>
        public void UpdateReadyForMigration()
        {
            this.ImageIndex = this.MigrationObject.ReadyForMigration ? 1 : 0;
        }
    }
}
