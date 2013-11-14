

namespace Sharezbold.ContentMigration
{
    using Sharezbold.ContentMigration.Data;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Sets up web services and provides access to them
    /// </summary>
    public sealed class WebService
    {
        public const string UrlLists = @"/_vti_bin/lists.asmx";
        public const string UrlWebs = @"/_vti_bin/webs.asmx";
        public const string UrlSites = @"/_vti_bin/sites.asmx";
        public const string UrlAdmin = @"/_vti_adm/admin.asmx";
        public const string UrlSiteData = @"/_vti_bin/SiteData.asmx";
        public const string UrlViews = @"/_vti_bin/Views.asmx";

        public string SrcUrl { get; private set; }
        public string DstUrl { get; private set; }
        public string DstUrlCA { get; private set; }

        public CredentialCache SrcCredentials { get; private set; }
        public CredentialCache DstCredentials { get; private set; }
        public CredentialCache DstCredentialsCA { get; private set; }

        public string SrcUser { get; private set; }
        public string SrcDomain { get; private set; }
        public string SrcPassword { get; private set; }

        public string DstUser { get; private set; }
        public string DstDomain { get; private set; }
        public string DstPassword { get; private set; }

        public SitesWS.Sites SrcSites { get; private set; }
        public SitesWS.Sites DstSites { get; private set; }
        public WebsWS.Webs SrcWebs { get; private set; }
        public WebsWS.Webs DstWebs { get; private set; }
        public AdminWS.Admin DstAdmin { get; private set; }
        public ListsWS.Lists SrcLists { get; private set; }
        public ListsWS.Lists DstLists { get; private set; }
        public SiteDataWS.SiteData SrcSiteData { get; private set; }
        public SiteDataWS.SiteData DstSiteData { get; private set; }
        public ViewsWS.Views SrcViews { get; private set; }
        public ViewsWS.Views DstViews { get; private set; }

        /// <summary>
        /// Sets up the web services
        /// </summary>
        public WebService(string srcUrl, string srcUser, string srcDomain, string srcPassword, string dstUrl, string dstUrlCA, string DstUser, string DstDomain, string DstPassword)
        {
            this.SrcUrl = srcUrl;
            this.SrcUser = srcUser;
            this.SrcDomain = srcDomain;
            this.SrcPassword = srcPassword;
            this.DstUrl = dstUrl;
            this.DstUrlCA = dstUrlCA;
            this.DstUser = DstUser;
            this.DstDomain = DstDomain;
            this.DstPassword = DstPassword;

            // set up urls
            string srcUrlLists = this.SrcUrl + UrlLists;
            string srcUrlWebs = this.SrcUrl + UrlWebs;
            string srcUrlSites = this.SrcUrl + UrlSites;
            string srcUrlSiteData = this.SrcUrl + UrlSiteData;
            string srcUrlViews = this.SrcUrl + UrlViews;

            string dstUrlLists = this.DstUrl + UrlLists;
            string dstUrlWebs = this.DstUrl + UrlWebs;
            string dstUrlSites = this.DstUrl + UrlSites;
            string dstUrlSiteData = this.DstUrl + UrlSiteData;
            string dstUrlViews = this.DstUrl + UrlViews;

            //central admin is needed for admin.asmx
            string dstUrlCaAdmin = DstUrlCA + UrlAdmin;

            this.SrcCredentials = new CredentialCache();
            this.SrcCredentials.Add(new Uri(SrcUrl), "NTLM", new NetworkCredential(SrcUser, SrcPassword, SrcDomain));

            this.DstCredentials = new CredentialCache();
            this.DstCredentials.Add(new Uri(DstUrl), "NTLM", new NetworkCredential(DstUser, DstPassword, DstDomain));

            this.DstCredentialsCA = new CredentialCache();
            this.DstCredentialsCA.Add(new Uri(DstUrlCA), "NTLM", new NetworkCredential(DstUser, DstPassword, DstDomain));

            this.SrcSites = new SitesWS.Sites();
            this.SrcSites.Url = srcUrlSites;
            this.SrcSites.Credentials = this.SrcCredentials;

            this.SrcWebs = new WebsWS.Webs();
            this.SrcWebs.Url = srcUrlWebs;
            this.SrcWebs.Credentials = this.SrcCredentials;

            this.DstWebs = new WebsWS.Webs();
            this.DstWebs.Url = dstUrlWebs;
            this.DstWebs.Credentials = this.DstCredentials;

            this.DstSites = new SitesWS.Sites();
            this.DstSites.Url = dstUrlSites;

            this.DstSites.Credentials = this.DstCredentials;

            this.DstAdmin = new AdminWS.Admin();
            this.DstAdmin.Url = dstUrlCaAdmin;
            this.DstAdmin.Credentials = this.DstCredentialsCA;

            this.SrcLists = new ListsWS.Lists();
            this.SrcLists.Url = srcUrlLists;
            this.SrcLists.Credentials = this.SrcCredentials;

            this.DstLists = new ListsWS.Lists();
            this.DstLists.Url = dstUrlLists;
            this.DstLists.Credentials = this.DstCredentials;

            this.SrcSiteData = new SiteDataWS.SiteData();
            this.SrcSiteData.Url = srcUrlSiteData;
            this.SrcSiteData.Credentials = this.SrcCredentials;

            this.DstSiteData = new SiteDataWS.SiteData();
            this.DstSiteData.Url = dstUrlSiteData;
            this.DstSiteData.Credentials = this.DstCredentials;

            this.SrcViews = new ViewsWS.Views();
            this.SrcViews.Url = srcUrlViews;
            this.SrcViews.Credentials = this.SrcCredentials;

            this.DstViews = new ViewsWS.Views();
            this.DstViews.Url = dstUrlViews;
            this.DstViews.Credentials = this.DstCredentials;
        }

        /// <summary>
        /// Set the correct url to views web service
        /// </summary>
        /// <param name="list">list to migrate</param>
        public void SetViewsMigrateTo(SList list)
        {
            this.DstViews.Url = list.MigrateTo.XmlData.Attributes["Url"].InnerText + UrlViews;
        }

        /// <summary>
        /// Tries to log in to source server, is true if server is accessible
        /// </summary>
        public bool IsSourceLoginPossible
        {
            get
            {
                try
                {
                    this.SrcWebs.GetWebCollection();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Source login failed: " + ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Tries to log in to destination server, is true if server is accessible
        /// </summary>
        public bool IsDestinationLoginPossible
        {
            get
            {
                try
                {
                    this.DstWebs.GetWebCollection();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Destination login failed: " + ex.Message);
                    return false;
                }
            }
        }
    }
}
