

namespace Sharezbold.ContentMigration.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    public class SSite
    {
        /// <summary>
        /// Represents the SOAP xml data
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
        public XmlNode XmlData { get; set; }
        
        /// <summary>
        /// Represents all visible lists (only visible lists will be migrated)
        /// List dependencies are:
        ///     BaseType="1"
        ///     FeatureId="00bfea71-e717-4e80-aa17-d0c71b360101"
        ///     ServerTemplate="101"
        ///     Author="1073741823"
        ///     WebId="e021b041-451b-4fc8-828b-cb7f6df1ac21"  --> web/site id
        ///     ScopeId="a8056a79-ef87-45cb-a17f-f1f0ed79fd9f"
        /// </summary>
        public List<KeyValuePair<SList, bool>> Lists { get; set; }

        /// <summary>
        /// Represents all lists
        /// </summary>
        public List<XmlNode> AllLists { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SSite()
        {
            this.Lists = new List<KeyValuePair<SList, bool>>();
            this.AllLists = new List<XmlNode>();
        }
    }
}
