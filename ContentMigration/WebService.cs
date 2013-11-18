

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
        /// <summary>
        /// The lists url
        /// </summary>
        public const string UrlLists = @"/_vti_bin/lists.asmx";

        /// <summary>
        /// The webs url
        /// </summary>
        public const string UrlWebs = @"/_vti_bin/webs.asmx";

        /// <summary>
        /// The sites url
        /// </summary>
        public const string UrlSites = @"/_vti_bin/sites.asmx";

        /// <summary>
        /// The admin url
        /// </summary>
        public const string UrlAdmin = @"/_vti_adm/admin.asmx";

        /// <summary>
        /// The site data url
        /// </summary>
        public const string UrlSiteData = @"/_vti_bin/SiteData.asmx";

        /// <summary>
        /// The views url
        /// </summary>
        public const string UrlViews = @"/_vti_bin/Views.asmx";

        /// <summary>
        /// Gets the source url
        /// </summary>
        public string SrcUrl { get; private set; }
        
        /// <summary>
        /// Gets the destination url
        /// </summary>
        public string DstUrl { get; private set; }
        
        /// <summary>
        /// Gets the destination url of its central administration
        /// </summary>
        public string DstUrlCA { get; private set; }

        /// <summary>
        /// Gets the source credential cache
        /// </summary>
        public CredentialCache SrcCredentials { get; private set; }
        
        /// <summary>
        /// Gets the destination credential cache
        /// </summary>
        public CredentialCache DstCredentials { get; private set; }
        
        /// <summary>
        /// Gets the destination credential cache of its central administration
        /// </summary>
        public CredentialCache DstCredentialsCA { get; private set; }

        /// <summary>
        /// Gets the source username
        /// </summary>
        public string SrcUser { get; private set; }
        
        /// <summary>
        /// Gets the source domain
        /// </summary>
        public string SrcDomain { get; private set; }
        
        /// <summary>
        /// Gets the source password
        /// </summary>
        public string SrcPassword { get; private set; }

        /// <summary>
        /// Gets the destination username
        /// </summary>
        public string DstUser { get; private set; }
        
        /// <summary>
        /// Gets the destination domain
        /// </summary>
        public string DstDomain { get; private set; }
        
        /// <summary>
        /// Gets the destination password
        /// </summary>
        public string DstPassword { get; private set; }

        /// <summary>
        /// Gets the source sites web service
        /// </summary>
        public SitesWS.Sites SrcSites { get; private set; }

        /// <summary>
        /// Gets the destination sites web service
        /// </summary>
        public SitesWS.Sites DstSites { get; private set; }

        /// <summary>
        /// Gets the source webs web service
        /// </summary>
        public WebsWS.Webs SrcWebs { get; private set; }

        /// <summary>
        /// Gets the destination webs web service
        /// </summary>
        public WebsWS.Webs DstWebs { get; private set; }

        /// <summary>
        /// Gets the destination admin web service
        /// </summary>
        public AdminWS.Admin DstAdmin { get; private set; }

        /// <summary>
        /// Gets the source lists web service
        /// </summary>
        public ListsWS.Lists SrcLists { get; private set; }

        /// <summary>
        /// Gets the destination lists web service
        /// </summary>
        public ListsWS.Lists DstLists { get; private set; }

        /// <summary>
        /// Gets the source sites web service
        /// </summary>
        public SiteDataWS.SiteData SrcSiteData { get; private set; }

        /// <summary>
        /// Gets the destination sites web service
        /// </summary>
        public SiteDataWS.SiteData DstSiteData { get; private set; }

        /// <summary>
        /// Gets the source views web service
        /// </summary>
        public ViewsWS.Views SrcViews { get; private set; }

        /// <summary>
        /// Gets the destination views web service
        /// </summary>
        public ViewsWS.Views DstViews { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the server is accessible or not
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
        /// Gets a value indicating whether login is possible on destination server
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

        /// <summary>
        /// Sets up the web services
        /// </summary>
        /// <param name="srcUrl">The source url</param>
        /// <param name="srcUser">The source user</param>
        /// <param name="srcDomain">The source domain</param>
        /// <param name="srcPassword">The source password</param>
        /// <param name="dstUrl">The destination url</param>
        /// <param name="dstUrlCA">The destination central administration url</param>
        /// <param name="DstUser">The destination user</param>
        /// <param name="DstDomain">The destination domain</param>
        /// <param name="DstPassword">The destination password</param>
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

            // central admin is needed for admin.asmx
            string dstUrlCaAdmin = this.DstUrlCA + UrlAdmin;

            this.SrcCredentials = new CredentialCache();
            this.SrcCredentials.Add(new Uri(this.SrcUrl), "NTLM", new NetworkCredential(this.SrcUser, this.SrcPassword, this.SrcDomain));

            this.DstCredentials = new CredentialCache();
            this.DstCredentials.Add(new Uri(this.DstUrl), "NTLM", new NetworkCredential(this.DstUser, this.DstPassword, this.DstDomain));

            this.DstCredentialsCA = new CredentialCache();
            this.DstCredentialsCA.Add(new Uri(this.DstUrlCA), "NTLM", new NetworkCredential(this.DstUser, this.DstPassword, this.DstDomain));

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
            this.DstViews.Url = list.MigrateToUrl + UrlViews;
        }

        /// <summary>
        /// Set the correct url to lists web service
        /// </summary>
        /// <param name="list">the source list</param>
        public void SetListsMigrateTo(SList list)
        {
            this.DstLists.Url = list.MigrateToUrl + UrlLists;
        }

        /// <summary>
        /// Sets the correct url to destination webs web service
        /// </summary>
        /// <param name="url">the new url</param>
        public void SetWebsMigrateTo(string url)
        {
            this.DstWebs.Url = url + UrlWebs;
        }

        /// <summary>
        /// Sets the correct url to source webs web service
        /// </summary>
        /// <param name="url"></param>
        public void SetWebsMigrateFrom(string url)
        {
            this.SrcWebs.Url = url + UrlWebs;
        }
    }
}
