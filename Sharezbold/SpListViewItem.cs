//-----------------------------------------------------------------------
// <copyright file="SpListViewItem.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold
{
    using Microsoft.SharePoint.Client;
    using Sharezbold.ContentMigration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// Represents a ListViewItem with a migration object
    /// </summary>
    public class SpListViewItem : ListViewItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">Represents the display text</param>
        /// <param name="imageIndex">Represents the image index</param>
        /// <param name="migrationObject">Represents the migration object</param>
        public SpListViewItem(MigrationObject migrationObject) : base(migrationObject.Identifier, migrationObject.ReadyForMigration ? 1 : 0)
        {
            this.MigrationObject = migrationObject;

            /*if (this.MigrationObject.DataObject.GetType()==typeof(Web))
            {
                this.Text 
            }
            else */
            if (this.MigrationObject.DataObject.GetType()==typeof(List))
            {
                this.Text = "          " + this.Text;
            }
            else if (this.MigrationObject.DataObject.GetType()==typeof(ListItem))
            {
                this.Text = "                    " + this.Text;
            }
        }

        /// <summary>
        /// Gets or sets the migration object
        /// </summary>
        public MigrationObject MigrationObject
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
