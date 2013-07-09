//-----------------------------------------------------------------------
// <copyright file="MigrationObject.cs" company="FH Wiener Neustadt">
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
    public class MigrationObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpTreeNode"/> class.
        /// </summary>
        /// <param name="identifier">text to be displayed in the tree view</param>
        /// <param name="dataObject">data object of the node</param>
        /// <param name="type">Type of the migration</param>
        /// <param name="destinationObject"></param>
        public MigrationObject(string identifier, object dataObject, bool skip=true, object destinationObject=null)
        {
            this.Identifier = identifier;
            this.DataObject = dataObject;
            this.Skip = skip;
            this.DestinationObject = destinationObject;
        }

        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        public string Identifier
        {
            get;
            set;
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

        /// <summary>
        /// Gets the information if the element is ready for migration
        /// </summary>
        public bool ReadyForMigration
        {
            get
            {
                if (this.DataObject != null && DestinationObject != null || Skip == true)
                    return true;
                else return false;
            }
        }

        /// <summary>
        /// Overrides the toString method
        /// </summary>
        /// <returns>the identifier of the migration object</returns>
        public override string ToString()
        {
            return this.Identifier;
        }
    }
}
