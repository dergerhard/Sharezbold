﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Sharezbold.Settings
{
    using System.Xml.Serialization;

    // 
    // This source code was auto-generated by xsd, Version=4.0.30319.17929.
    // 


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:xmlns:Sharezbold:SharepointMigration")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:xmlns:Sharezbold:SharepointMigration", IsNullable = false)]
    public partial class MigrationSettings
    {

        private string fromHostField;

        private string fromDomainField;

        private string fromUserNameField;

        private string fromPasswordField;

        private string toHostField;

        private string toHostCAField;

        private string toDomainField;

        private string toUserNameField;

        private string toPasswordField;

        private bool proxyActiveField;

        private string proxyUrlField;

        private string proxyUsernameField;

        private string proxyPasswordField;

        private bool siteCollectionMigrationField;

        /// <remarks/>
        public string FromHost
        {
            get
            {
                return this.fromHostField;
            }
            set
            {
                this.fromHostField = value;
            }
        }

        /// <remarks/>
        public string FromDomain
        {
            get
            {
                return this.fromDomainField;
            }
            set
            {
                this.fromDomainField = value;
            }
        }

        /// <remarks/>
        public string FromUserName
        {
            get
            {
                return this.fromUserNameField;
            }
            set
            {
                this.fromUserNameField = value;
            }
        }

        /// <remarks/>
        public string FromPassword
        {
            get
            {
                return this.fromPasswordField;
            }
            set
            {
                this.fromPasswordField = value;
            }
        }

        /// <remarks/>
        public string ToHost
        {
            get
            {
                return this.toHostField;
            }
            set
            {
                this.toHostField = value;
            }
        }

        /// <remarks/>
        public string ToHostCA
        {
            get
            {
                return this.toHostCAField;
            }
            set
            {
                this.toHostCAField = value;
            }
        }

        /// <remarks/>
        public string ToDomain
        {
            get
            {
                return this.toDomainField;
            }
            set
            {
                this.toDomainField = value;
            }
        }

        /// <remarks/>
        public string ToUserName
        {
            get
            {
                return this.toUserNameField;
            }
            set
            {
                this.toUserNameField = value;
            }
        }

        /// <remarks/>
        public string ToPassword
        {
            get
            {
                return this.toPasswordField;
            }
            set
            {
                this.toPasswordField = value;
            }
        }

        /// <remarks/>
        public bool ProxyActive
        {
            get
            {
                return this.proxyActiveField;
            }
            set
            {
                this.proxyActiveField = value;
            }
        }

        /// <remarks/>
        public string ProxyUrl
        {
            get
            {
                return this.proxyUrlField;
            }
            set
            {
                this.proxyUrlField = value;
            }
        }

        /// <remarks/>
        public string ProxyUsername
        {
            get
            {
                return this.proxyUsernameField;
            }
            set
            {
                this.proxyUsernameField = value;
            }
        }

        /// <remarks/>
        public string ProxyPassword
        {
            get
            {
                return this.proxyPasswordField;
            }
            set
            {
                this.proxyPasswordField = value;
            }
        }

        /// <remarks/>
        public bool SiteCollectionMigration
        {
            get
            {
                return this.siteCollectionMigrationField;
            }
            set
            {
                this.siteCollectionMigrationField = value;
            }
        }
    }
}