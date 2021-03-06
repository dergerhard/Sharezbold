﻿//-----------------------------------------------------------------------
// <copyright file="SSite.cs" company="FH Wiener Neustadt">
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
    /// Represents a sharepoint site
    /// </summary>
    public class SSite : TreeNode, IMigratable
    {
        /// <summary>
        /// Data storage object for migrate
        /// </summary>
        private bool migrate;
 
        /// <summary>
        /// storage object for XmlData
        /// </summary>
        private XmlNode xmlData;

        /// <summary>
        /// Data storage object for Lists
        /// </summary>
        private List<SList> lists;

        /// <summary>
        /// Initializes a new instance of the <see cref="SSite" /> class.
        /// </summary>
        public SSite()
            : base(string.Empty)
        {
            this.lists = new List<SList>();
            this.AllLists = new List<XmlNode>();
            this.IsSiteCollectionSite = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the element should be migrated
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
        /// Gets or sets a value indicating whether an object is ready for migration
        /// </summary>
        public bool ReadyForMigration
        {
            get
            {
                return true;
            }

            set
            {
                // do nothing. site is always migratable
            }
        }

        /// <summary>
        /// Gets the name of the object
        /// </summary>
        public new string Name
        {
            get
            {
                return this.Text;
            }
        }

        /// <summary>
        /// Gets or sets the SOAP xml data
        /// SOAP data format example:
        /// <Web Title="Fucking site collection" 
        ///         Url="http://ss13-css-009:31920"
        ///         Description="" 
        ///         Language="1033" 
        ///         FarmId="{736cfc86-9f17-4379-a356-223f380d0816}" 
        ///         Id="{e021b041-451b-4fc8-828b-cb7f6df1ac21}" 
        ///         ExcludeFromOfflineClient="False" 
        ///         CellStorageWebServiceEnabled="True" 
        ///         AlternateUrls="http://ss13-css-009:31920/" 
        ///         xmlns="http://schemas.microsoft.com/sharepoint/soap/" />
        /// </summary>
        public XmlNode XmlData 
        {
            get
            {
                return this.xmlData;
            }

            set
            {
                this.xmlData = value;
                if (this.xmlData != null)
                {
                    this.Text = this.xmlData.Attributes["Title"].InnerText;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the site is the site collection or not
        /// </summary>
        public bool IsSiteCollectionSite { get; set; }

        /// <summary>
        /// Gets all visible lists (only visible lists will be migrated)
        /// List dependencies are:
        ///     BaseType="1"
        ///     FeatureId="00bfea71-e717-4e80-aa17-d0c71b360101"
        ///     ServerTemplate="101"
        ///     Author="1073741823"
        ///     WebId="e021b041-451b-4fc8-828b-cb7f6df1ac21"  --> web/site id
        ///     ScopeId="a8056a79-ef87-45cb-a17f-f1f0ed79fd9f"
        /// </summary>
        public List<SList> Lists
        {
            get
            {
                return this.lists;
            }
        }

        /// <summary>
        /// Gets or sets all lists
        /// </summary>
        public List<XmlNode> AllLists { get; set; }

        /// <summary>
        /// Gets or sets  the parent object
        /// </summary>
        public IMigratable ParentObject { get; set; }

        /// <summary>
        /// Adds the list to the visible Lists and creates a tree node
        /// </summary>
        /// <param name="list">the list object</param>
        /// <param name="migrate">migrate it or not</param>
        public void AddList(SList list, bool migrate)
        {
            list.Migrate = migrate;
            this.lists.Add(list);
            this.Nodes.Add(list);
        }
    }
}
