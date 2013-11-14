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
        /// data storage object for MigrateTo
        /// </summary>
        private SSite migrateTo;

        /// <summary>
        /// distination site to where we migrate
        /// </summary>
        public SSite MigrateTo
        {
            get
            {
                return migrateTo;
            }
            set
            {
                migrateTo = value;
            }
        }

        /// <summary>
        /// Defines whether an object is ready for migration
        /// </summary>
        public bool ReadyForMigration
        {
            get
            {
                // if parent site is migrated we are ready
                if (((SSite)this.ParentObject).Migrate)
                {
                    return true;
                }
                else if (this.migrateTo != null)
                {
                    return true;
                }
                return false;

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
        /// Represents the parent object
        /// </summary>
        public IMigratable ParentObject { get; set; }

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
        /// Data example:
        /// <rs:data ItemCount="2" xmlns:rs="urn:schemas-microsoft-com:rowset">
        ///     <z:row ows_CompanyName="IBM" ows_ApplicationDate="2013-11-04 00:00:00" ows_ContactPerson="John Doe" ows_Interview="Yes" ows_MetaInfo="1;#" ows__ModerationStatus="0" ows__Level="1" ows_ID="1" ows_UniqueId="1;#{C99A189F-FCC5-4FF5-8DC5-298B9DA1C579}" ows_owshiddenversion="1" ows_FSObjType="1;#0" ows_Created="2013-11-05 16:50:12" ows_PermMask="0x7fffffffffffffff" ows_Modified="2013-11-05 16:50:12" ows_FileRef="1;#Lists/MyJobApplications/1_.000" xmlns:z="#RowsetSchema" />
        ///     <z:row ows_CompanyName="Siemens" ows_ApplicationDate="2013-11-06 00:00:00" ows_ContactPerson="Brigitte Ederer" ows_Interview="Yes" ows_MetaInfo="2;#" ows__ModerationStatus="0" ows__Level="1" ows_ID="2" ows_UniqueId="2;#{7515C21E-87F8-494B-8D77-2EED77A7965F}" ows_owshiddenversion="1" ows_FSObjType="2;#0" ows_Created="2013-11-05 16:52:09" ows_PermMask="0x7fffffffffffffff" ows_Modified="2013-11-05 16:52:09" ows_FileRef="2;#Lists/MyJobApplications/2_.000" xmlns:z="#RowsetSchema" />
        /// </rs:data>
        /// </summary>
        public XmlNode XmlListData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SList()
        {
            this.migrateTo = null;
        }
    }
}
