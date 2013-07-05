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
        private object dataObject;

        /// <summary>
        /// the data object represents the object that correspondes to the visual entry of the tree view
        /// </summary>
        public object DataObject
        {
            get;
            set;
        }

        
        /// <summary>
        /// just a copy of the original one from tree node
        /// </summary>
        /// <param name="identifier">text that will be displayed</param>
        public SpTreeNode(string identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// offers a way to put name and data object to a tree node
        /// </summary>
        /// <param name="identifier">text to be displayed in the tree view</param>
        /// <param name="dataObject">data object of the node</param>
        public SpTreeNode(string identifier, object dataObject) : base (identifier)
        {
            this.DataObject = dataObject;
        }
    }
}
