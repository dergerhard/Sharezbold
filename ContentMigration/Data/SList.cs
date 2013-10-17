

namespace Sharezbold.ContentMigration.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
    public class SList
    {
        /// <summary>
        /// Represents the list description in SOAP
        /// </summary>
        public XmlNode XmlList { get; set; }

        /// <summary>
        /// Represents the list data
        /// </summary>
        public XmlNode XmlListData { get; set; }

        /// <summary>
        /// List items - if a list is migrated, all items will be migrated
        /// </summary>
        //public List<XmlNode> ListItems { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SList()
        {
        }
    }
}
