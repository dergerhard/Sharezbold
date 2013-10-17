//-----------------------------------------------------------------------
// <copyright file="ContentDownloader.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ContentMigration.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Xml;
    
    /// <summary>
    /// Represents a Sharepoint list
    /// Example data:
    /// <List DocTemplateUrl="/Shared Documents/Forms/template.dotx"
    ///         DefaultViewUrl="/Shared Documents/Forms/AllItems.aspx"
    ///         ID="{74270611-00CF-40AD-94C8-DC23B4856963}"
    ///         Title="Documents"
    ///         Description=""
    ///         ImageUrl="/_layouts/15/images/itdl.png?rev=23"
    ///         Name="{74270611-00CF-40AD-94C8-DC23B4856963}"
    ///         BaseType="1"
    ///         FeatureId="00bfea71-e717-4e80-aa17-d0c71b360101"
    ///         ServerTemplate="101"
    ///         Created="20131012 22:53:34"
    ///         Modified="20131012 23:40:27"
    ///         LastDeleted="20131012 22:53:34"
    ///         Version="0"
    ///         Direction="none"
    ///         Flags="4104"
    ///         ItemCount="1"
    ///         ReadSecurity="1"
    ///         WriteSecurity="1"
    ///         Author="1073741823"
    ///         WebId="e021b041-451b-4fc8-828b-cb7f6df1ac21"
    ///         ScopeId="a8056a79-ef87-45cb-a17f-f1f0ed79fd9f"
    ///         Hidden="False"
    ///         MaxItemsPerThrottledOperation="5000" />    
    /// </summary>
    public class SList : TreeNode, IMigratable
    {
        /// <summary>
        /// Data storage object for migrate
        /// </summary>
        private bool migrate;

        /// <summary>
        /// Defines wheter to migrate the site collection or not
        /// </summary>
        public bool Migrate
        {
            get
            {
                return this.migrate;
            }

            set
            {
                this.migrate = value;
            }
        }

        /// <summary>
        /// Defines whether an object is ready for migration
        /// </summary>
        public bool ReadyForMigration
        {
            get
            {
                return true;
            }

            set
            {
                
            }
        }

        /// <summary>
        /// Gaters the name of the object
        /// </summary>
        public string Name
        {
            get
            {
                return this.Text;
            }
        }
        
        /// <summary>
        /// Storage object for XmlList
        /// </summary>
        private XmlNode xmlList;

        /// <summary>
        /// Represents the list description in SOAP
        /// </summary>
        public XmlNode XmlList 
        {
            get
            {
                return this.xmlList;
            }

            set
            {
                this.xmlList = value;
                if (this.xmlList != null)
                {
                    this.Text = xmlList.Attributes["Title"].InnerText;
                }
            }
        }

        /// <summary>
        /// Represents the list data
        /// </summary>
        public XmlNode XmlListData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SList()
        {
        }
    }
}
