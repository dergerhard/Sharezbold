﻿//-----------------------------------------------------------------------
// <copyright file="SSiteCollection.cs" company="FH Wiener Neustadt">
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
    /// Information for a Sharepoint site collection
    /// </summary>
    public class SSiteCollection : TreeNode, IMigratable
    {
        /// <summary>
        /// Data storage object for Sites
        /// </summary>
        private List<SSite> sites;

        /// <summary>
        /// Data storage object for migrate
        /// </summary>
        private bool migrate;

        /// <summary>
        /// Initializes a new instance of the <see cref="SSiteCollection"/> class.
        /// </summary>
        public SSiteCollection()
            : base(string.Empty)
        {
            this.sites = new List<SSite>();
            this.migrate = false;
            this.ParentObject = null;
        }

        /// <summary>
        /// Gets or sets the Xml data of the site collection
        /// </summary>
        public List<XmlNode> XmlData { get; set; }

        /// <summary>
        /// Gets all sub sites
        /// </summary>
        public List<SSite> Sites
        {
            get
            {
                return this.sites;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to migrate the site collection or not. 
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
                foreach (SSite s in this.sites)
                {
                    if (s.IsSiteCollectionSite)
                    {
                        s.Migrate = value;
                    }
                }
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
        /// Gets or sets  the parent object
        /// </summary>
        public IMigratable ParentObject { get; set; }

        /// <summary>
        /// Adds a site to the data base and the child tree nodes
        /// </summary>
        /// <param name="s">The site to add</param>
        /// <param name="migrate">Specifies whether to migrate it or not</param>
        public void AddSite(SSite s, bool migrate)
        {
            s.Migrate = migrate;
            this.sites.Add(s);
            this.Nodes.Add(s);
            if (s.IsSiteCollectionSite)
            {
                this.Text = s.XmlData != null ? s.XmlData.Attributes["Title"].InnerText : string.Empty;
            }
        }
    }
}
