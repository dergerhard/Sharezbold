//-----------------------------------------------------------------------
// <copyright file="SpTreeNode.cs" company="FH Wiener Neustadt">
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

    /// <summary>
    /// helper class for coupling the view with the corresponding data
    /// </summary>
    public class SpTreeNode : TreeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpTreeNode"/> class. Offers a way to store tree node and corresponding object together.
        /// </summary>
        /// <param name="migrationObject">the migration object </param>
        public SpTreeNode(MigrationObject migrationObject) : base(migrationObject.ToString())
        {
            this.MigrationObject = migrationObject;
        }

        /// <summary>
        /// Gets or sets the migration object
        /// </summary>
        public MigrationObject MigrationObject
        {
            get;
            set;
        }
    }
}
