

namespace Sharezbold.ContentMigration
{
    using System;
    using System.Collections.Generic;
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

        private string srcUrl;
        private string dstUrl;
        private string dstUrlCA;

        private CredentialCache srcCredentials;
        private CredentialCache dstCredentials;
        private CredentialCache dstCredentialsCA;

        private string srcUser;
        private string srcDomain;
        private string srcPassword;

        private string dstUser;
        private string dstDomain;
        private string dstPassword;

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
        public WebService(string srcUrl, string srcUser, string srcDomain, string srcPassword, string dstUrl, string dstUrlCA, string dstUser, string dstDomain, string dstPassword)
        {
            this.srcUrl = srcUrl;
            this.srcUser = srcUser;
            this.srcDomain = srcDomain;
            this.srcPassword = srcPassword;
            this.dstUrl = dstUrl;
            this.dstUrlCA = dstUrlCA;
            this.dstUser = dstUser;
            this.dstDomain = dstDomain;
            this.dstPassword = dstPassword;

            // set up urls
            string srcUrlLists = this.srcUrl + UrlLists;
            string srcUrlWebs = this.srcUrl + UrlWebs;
            string srcUrlSites = this.srcUrl + UrlSites;
            string srcUrlSiteData = this.srcUrl + UrlSiteData;
            string srcUrlViews = this.srcUrl + UrlViews;

            string dstUrlLists = this.dstUrl + UrlLists;
            string dstUrlWebs = this.dstUrl + UrlWebs;
            string dstUrlSites = this.dstUrl + UrlSites;
            string dstUrlSiteData = this.dstUrl + UrlSiteData;
            string dstUrlViews = this.dstUrl + UrlViews;

            //central admin is needed for admin.asmx
            string dstUrlCaAdmin = dstUrlCA + UrlAdmin;

            this.srcCredentials = new CredentialCache();
            this.srcCredentials.Add(new Uri(srcUrl), "NTLM", new NetworkCredential(srcUser, srcPassword, srcDomain));

            this.dstCredentials = new CredentialCache();
            this.dstCredentials.Add(new Uri(dstUrl), "NTLM", new NetworkCredential(dstUser, dstPassword, dstDomain));

            this.dstCredentialsCA = new CredentialCache();
            this.dstCredentialsCA.Add(new Uri(dstUrlCA), "NTLM", new NetworkCredential(dstUser, dstPassword, dstDomain));

            this.SrcSites = new SitesWS.Sites();
            this.SrcSites.Url = srcUrlSites;
            this.SrcSites.Credentials = this.srcCredentials;

            this.SrcWebs = new WebsWS.Webs();
            this.SrcWebs.Url = srcUrlWebs;
            this.SrcWebs.Credentials = this.srcCredentials;

            this.DstWebs = new WebsWS.Webs();
            this.DstWebs.Url = dstUrlWebs;
            this.DstWebs.Credentials = this.dstCredentials;

            this.DstSites = new SitesWS.Sites();
            this.DstSites.Url = dstUrlSites;

            this.DstSites.Credentials = this.dstCredentials;

            this.DstAdmin = new AdminWS.Admin();
            this.DstAdmin.Url = dstUrlCaAdmin;
            this.DstAdmin.Credentials = this.dstCredentialsCA;

            this.SrcLists = new ListsWS.Lists();
            this.SrcLists.Url = srcUrlLists;
            this.SrcLists.Credentials = this.srcCredentials;

            this.DstLists = new ListsWS.Lists();
            this.DstLists.Url = dstUrlLists;
            this.DstLists.Credentials = this.dstCredentials;

            this.SrcSiteData = new SiteDataWS.SiteData();
            this.SrcSiteData.Url = srcUrlSiteData;
            this.SrcSiteData.Credentials = this.srcCredentials;

            this.DstSiteData = new SiteDataWS.SiteData();
            this.DstSiteData.Url = dstUrlSiteData;
            this.DstSiteData.Credentials = this.dstCredentials;

            this.SrcViews = new ViewsWS.Views();
            this.SrcViews.Url = srcUrlViews;
            this.SrcViews.Credentials = this.srcCredentials;

            this.DstViews = new ViewsWS.Views();
            this.DstViews.Url = dstUrlViews;
            this.DstViews.Credentials = this.dstCredentials;
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
                }
                catch (Exception ex)
                {
                    return false;
                }
                return true;
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
                }
                catch (Exception ex)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
