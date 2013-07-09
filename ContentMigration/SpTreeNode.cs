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
        /// Initializes a new instance of the <see cref="SpTreeNode"/> class.
        /// </summary>
        /// <param name="identifier">text that will be displayed</param>
        public SpTreeNode(string identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpTreeNode"/> class. Offers a way to store treenode and corresponding object together.
        /// </summary>
        /// <param name="identifier">text to be displayed in the tree view</param>
        /// <param name="dataObject">data object of the node</param>
        public SpTreeNode(string identifier, object dataObject) : base(identifier)
        {
            this.DataObject = dataObject;
            this.Skip = true;
            this.DestinationObject = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpTreeNode"/> class.
        /// </summary>
        /// <param name="identifier">text to be displayed in the tree view</param>
        /// <param name="dataObject">data object of the node</param>
        /// <param name="type">Type of the migration</param>
        /// <param name="destinationObject"></param>
        public SpTreeNode(string identifier, object dataObject, bool skip, object destinationObject)
            : base(identifier)
        {
            this.DataObject = dataObject;
            this.Skip = skip;
            this.DestinationObject = destinationObject;
        }

        /// <summary>
        /// Gets or sets the data object that represents the object that correspondes to the visual entry of the tree view
        /// </summary>
        public object DataObject
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the skip value (omitted if true)
        /// </summary>
        public bool Skip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Destination Object
        /// </summary>
        public object DestinationObject
        {
            get;
            set;
        }
    }
}
