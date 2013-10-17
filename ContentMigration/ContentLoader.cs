

namespace Sharezbold.ContentMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Sharezbold.ContentMigration.Data;
    using System.Xml.Linq;

    /// <summary>
    /// Is responsible for downloading/uploading data from the source to the destinaiton server
    /// </summary>
    public class ContentLoader
    {
        private const string urlLists = @"/_vti_bin/lists.asmx";
        private const string urlWebs = @"/_vti_bin/webs.asmx";
        private const string urlSites = @"/_vti_bin/sites.asmx";
        private const string urlAdmin = @"/_vti_adm/admin.asmx";
        private const string urlSiteData = @"/_vti_bin/SiteData.asmx";
        private const string urlViews = @"/_vti_bin/Views.asmx";

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

        private SitesWS.Sites srcSites;
        private SitesWS.Sites dstSites;
        private WebsWS.Webs srcWebs;
        private WebsWS.Webs dstWebs;
        private AdminWS.Admin dstAdmin;
        private ListsWS.Lists srcLists;
        private ListsWS.Lists dstLists;
        private SiteDataWS.SiteData srcSiteData;
        private SiteDataWS.SiteData dstSiteData;
        private ViewsWS.Views srcViews;
        private ViewsWS.Views dstViews;

        /// <summary>
        /// sets up the web services
        /// </summary>
        public ContentLoader(string srcUrl, string srcUser, string srcDomain, string srcPassword, string dstUrl, string dstUrlCA, string dstUser, string dstDomain, string dstPassword)
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
            string srcUrlLists = srcUrl + urlLists;
            string srcUrlWebs = srcUrl + urlWebs;
            string srcUrlSites = srcUrl + urlSites;
            string srcUrlSiteData = srcUrl + urlSiteData;
            string srcUrlViews = srcUrl + urlViews;
            
            string dstUrlLists = dstUrl + urlLists;
            string dstUrlWebs = dstUrl + urlWebs;
            string dstUrlSites = dstUrl + urlSites;
            string dstUrlSiteData = dstUrl + urlSiteData;
            string dstUrlViews = dstUrl + urlViews;

            //central admin is needed for admin.asmx
            string dstUrlCaAdmin = dstUrlCA + urlAdmin;

            srcCredentials = new CredentialCache();
            srcCredentials.Add(new Uri(srcUrl), "NTLM", new NetworkCredential(srcUser, srcPassword, srcDomain));

            dstCredentials = new CredentialCache();
            dstCredentials.Add(new Uri(dstUrl), "NTLM", new NetworkCredential(dstUser, dstPassword, dstDomain));

            dstCredentialsCA = new CredentialCache();
            dstCredentialsCA.Add(new Uri(dstUrlCA), "NTLM", new NetworkCredential(dstUser, dstPassword, dstDomain));

            srcSites = new SitesWS.Sites();
            srcSites.Url = srcUrlSites;
            srcSites.Credentials = srcCredentials;

            srcWebs = new WebsWS.Webs();
            srcWebs.Url = srcUrlWebs;
            srcWebs.Credentials = srcCredentials;

            dstWebs = new WebsWS.Webs();
            dstWebs.Url = dstUrlWebs;
            dstWebs.Credentials = dstCredentials;

            dstSites = new SitesWS.Sites();
            dstSites.Url = dstUrlSites;
            dstSites.Credentials = dstCredentials;

            dstAdmin = new AdminWS.Admin();
            dstAdmin.Url = dstUrlCaAdmin;
            dstAdmin.Credentials = dstCredentialsCA;

            srcLists = new ListsWS.Lists();
            srcLists.Url = srcUrlLists;
            srcLists.Credentials = srcCredentials;

            dstLists = new ListsWS.Lists();
            dstLists.Url = dstUrlLists;
            dstLists.Credentials = dstCredentials;

            srcSiteData = new SiteDataWS.SiteData();
            srcSiteData.Url = srcUrlSiteData;
            srcSiteData.Credentials = srcCredentials;

            dstSiteData = new SiteDataWS.SiteData();
            dstSiteData.Url = dstUrlSiteData;
            dstSiteData.Credentials = dstCredentials;

            srcViews = new ViewsWS.Views();
            srcViews.Url = srcUrlViews;
            srcViews.Credentials = srcCredentials;

            dstViews = new ViewsWS.Views();
            dstViews.Url = dstUrlViews;
            dstViews.Credentials = dstCredentials;
        }

        /// <summary>
        /// Tries to log in to source server, is true if server is accessible
        /// </summary>
        public bool IsSourceLoginPossible
        {
            get
            {
                // todo
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
                // todo
                return true;
            }
        }


        public SSiteCollection SourceSiteCollection { get; private set; }

        public SSiteCollection DestinationSiteCollection { get; private set; }

        public void test2()
        {

            //XmlNode listDetails = srcLists.GetList("My Applications");
            /*
             * AddList (
	            string listName,    --> Title
	            string description, --> Description
	            int templateID      --> ServerTemplate )

             * foreach field
             *      
             */
        }


        public void test()
        {

            SiteDataWS._sWebMetadata siteMData = null;
            SiteDataWS._sWebWithTime[] siteTime = null;
            SiteDataWS._sListWithTime[] lstMData = null;
            SiteDataWS._sFPUrl[] urls = null;
            string strSGroups;
            string[] strSGrpUsrs;
            string[] strSGrpGrps;

            srcSiteData.GetWeb(out siteMData, out siteTime, out lstMData, out urls,
                out strSGroups, out strSGrpUsrs, out strSGrpGrps);

            string info = "";
            info = strSGroups + "\n";

            foreach (string usr in strSGrpUsrs)
            {
                info += usr + "\n";
            }

            foreach (string grp in strSGrpGrps)
            {
                info += grp + "\n";
            }

            //Console.WriteLine(info);



            SiteDataWS._sSiteMetadata scSiteMData;
            SiteDataWS._sWebWithTime[] scSiteTime;

            string users;
            string groups;
            string[] arrGroups;

            srcSiteData.GetSite(out scSiteMData, out scSiteTime, out users, out groups, out arrGroups);

            Console.WriteLine(scSiteMData.ToString() + "\r\n####");
            Console.WriteLine(scSiteTime.ToString() + "\r\n####");
            Console.WriteLine(users+"\r\n####");
            Console.WriteLine(groups + "\r\n#####");
            foreach (string g in arrGroups)
                Console.WriteLine(g);

            Console.WriteLine("...");
        }


        public SSiteCollection LoadSourceData()
        {
            this.SourceSiteCollection = new SSiteCollection();

            /* whole migration
             * 1. Transfer users and groups
             * 2. Transfer all metadata as:
             *      2.1 site columns
             *      2.2 content types (depend on site columns)
             *      2.3 site templates
             *      
             *  3. Transfer site collection
             *  4. Transfer sites
             *  5. Transfer lists and libraries
             *  6. Transfer list data
             *  7. Transfer files
             */


            // get all webs names (first is the site collection)
            XmlNode allSrcWebs = srcWebs.GetAllSubWebCollection();
            // result<List>: <Web Title="Fucking site collection" Url="http://ss13-css-009:31920" xmlns="http://schemas.microsoft.com/sharepoint/soap/" />
            Dictionary<string, string> webs = new Dictionary<string, string>();
            foreach (XmlNode web in allSrcWebs)
                webs.Add(web.Attributes["Url"].InnerText, web.Attributes["Title"].InnerText);

            bool firstRun = true;
            string srcListsUrlBuffer = srcLists.Url;
            foreach (KeyValuePair<string, string> web in webs)
            {
                // load details on each web
                XmlNode w = srcWebs.GetWeb(web.Key);
                SSite site = new SSite();
                site.XmlData = w;

                string url = w.Attributes["Url"].InnerText + urlLists;
                Console.WriteLine(url);

                // get all lists
                srcLists.Url = url;
                XmlNode lc = srcLists.GetListCollection(); 
                
                // lists to migrate: Hidden="False"
                foreach (XmlNode list in lc.ChildNodes)
                {
                    site.AllLists.Add(list);

                    if (list.Attributes["Hidden"].InnerText.ToUpper().Equals("FALSE"))
                    {
                        // load list details with all fields
                        XmlNode listDetails = srcLists.GetList(list.Attributes["Title"].InnerText);
                        SList sList = new SList();
                        sList.XmlList = listDetails;
                        
                        if (listDetails.Attributes["Title"].InnerText.Equals("My Applications"))
                        {
                            Console.WriteLine("abc");
                            this.MigrateList(sList);
                        }


                        // load list data
                        XmlNode ndListItems = srcLists.GetListItems(list.Attributes["Title"].InnerText, null, null, null, null, null, null);
                        sList.XmlListData = ndListItems;

                        site.Lists.Add(new KeyValuePair<SList,bool>(sList, false));
                        Console.WriteLine("\t\t" + list.Attributes["Title"].InnerText);
                    }
                }

                if (firstRun)
                {
                    this.SourceSiteCollection.SiteCollectionSite = site;
                    firstRun = false;
                }
                else
                {
                    this.SourceSiteCollection.Sites.Add(new KeyValuePair<SSite, bool>(site, false));
                }
            }
            srcLists.Url = srcListsUrlBuffer;

            
            return this.SourceSiteCollection;
            
        }

        /// <summary>
        /// Migrates a list and its views. Site Columns are not included
        /// </summary>
        /// <param name="list"></param>
        public void MigrateList(SList list)
        {
            //this.MigrateSiteColumns();

            // if list with the same name exists --> delete
            XmlElement l = (XmlElement)list.XmlList;
            string listName = l.Attributes["Title"].InnerText;

            try
            {
                dstLists.DeleteList(listName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Log: " + e.Message);
            }

            //add list from source
            dstLists.AddList(listName,  l.Attributes["Description"].InnerText, Convert.ToInt32(l.Attributes["ServerTemplate"].InnerText));

            // copy list properties
            XmlDocument doc = new XmlDocument();
            XmlElement listProperties = doc.CreateElement("List");
            foreach (XmlAttribute attr in l.Attributes)
            {
                listProperties.SetAttribute(attr.Name, attr.Value);
            }

            XmlElement fields = doc.CreateElement("Fields");
            int i = 1;
            foreach (XmlNode field in l["Fields"])
            {
                if (field.Name.Equals("Field"))
                {
                    XmlElement method = doc.CreateElement("Method");
                    method.SetAttribute("ID", i.ToString());
                    XmlNode newField = doc.ImportNode(field, true);
                    newField.Attributes.RemoveNamedItem("ID");
                    method.AppendChild(newField);
                    fields.AppendChild(method);
                    i++;
                }
            }

            // update the list
            dstLists.UpdateList(listName, listProperties, fields, fields, null, "");

            // migrate the views
            // first delete the dst views
            try
            {
                XmlNode viewsToDelete = dstViews.GetViewCollection(listName);
                foreach (XmlNode view in viewsToDelete)
                {
                    dstViews.DeleteView(listName, view.Attributes["Name"].InnerText);
                }
            }
            catch (Exception e)
            {
            }

            XmlNode viewCollection = srcViews.GetViewCollection(listName);
            Console.WriteLine(viewCollection.OuterXml);
            foreach (XmlNode view in viewCollection)
            {
                string viewName = view.Attributes["Name"].InnerText;
                XmlNode viewDetail = srcViews.GetView(listName, viewName);

                XmlDocument doc2 = new XmlDocument();
                XmlElement viewFields = (XmlElement)doc2.ImportNode(viewDetail["ViewFields"], true);

                XmlDocument doc3 = new XmlDocument();
                XmlElement query = (XmlElement)doc3.ImportNode(viewDetail["Query"], true);

                XmlDocument doc4 = new XmlDocument();
                XmlElement rowLimit = (XmlElement)doc4.ImportNode(viewDetail["RowLimit"], true);

                bool makeViewDefault = viewDetail.Attributes["DefaultView"].InnerText.ToUpper().Equals("TRUE") ? true : false;

                // add the view
                dstViews.AddView(listName, view.Attributes["DisplayName"].InnerText, viewFields, query, rowLimit, viewDetail.Attributes["Type"].InnerText, makeViewDefault);
            }

        }


        /// <summary>
        /// Migrates all new site columns from src to dst. Columns which changed are ignored, as "system columns" can't
        /// be  recognised yet.
        /// </summary>
        public void MigrateSiteColumns()
        {
            XmlNode srcColumsXml = srcWebs.GetColumns();
            XmlNode dstColumnsXml = dstWebs.GetColumns();

            //convert to xdoc
            XDocument srcDoc = XDocument.Parse(srcColumsXml.OuterXml);
            XDocument dstDoc = XDocument.Parse(dstColumnsXml.OuterXml);

            /* COLUMNS
             * create dest dictionary
             * for all src elements
             *      if dest contains src
             *          compare:
             *            == --> do nothing (same content type)
             *            != --> add to update list (something changed, so migrate)
             *      else
             *          --> add to create list (because its new)
             */

            // create dst dictionary
            Dictionary<string, XElement> dstColumns = new Dictionary<string, XElement>();
            foreach (XElement el in dstDoc.Root.Elements())
            {
                dstColumns.Add(el.Attribute("ID").Value, el);
            }

            //List<XElement> updateColumns = new List<XElement>();
            List<XElement> createColumns = new List<XElement>();

            XNodeEqualityComparer comparer = new XNodeEqualityComparer();

            foreach (XElement el in srcDoc.Root.Elements())
            {
                if (dstColumns.ContainsKey(el.Attribute("ID").Value))
                {
                    /*if (comparer.GetHashCode(el) != comparer.GetHashCode(dstColumns[el.Attribute("ID").Value]))
                    {
                        updateColumns.Add(el);
                    }*/
                }
                else
                {
                    createColumns.Add(el);
                }
            }

            // now columns that have to be updated or created are identified
            // time to create or update them
            int id = 1;
            /*string updateStr = "";
            foreach (XElement el in updateColumns)
            {
                //updateStr += el.ToString();
                updateStr += "<Method ID=\"" + id++ + "\" Cmd=\"Update\">" + el.ToString() + "</Method>";
            }
            XmlDocument updateDoc = new XmlDocument();
            updateDoc.LoadXml("<Fields>" + updateStr + "</Fields>");
            XmlNode updateNode = updateDoc.DocumentElement;
            */

            string createStr = "";
            foreach (XElement el in createColumns)
            {
                createStr += "<Method ID=\"" + id++ + "\" Cmd=\"New\">" + el.ToString() + "</Method>";
            }
            XmlDocument createDoc = new XmlDocument();
            createDoc.LoadXml("<Fields>" + createStr + "</Fields>");
            XmlNode createNode = createDoc.DocumentElement;

            //xmldocument object
            XmlDocument xDoc = new XmlDocument();
            //Fields to be added
            XmlElement newFields = xDoc.CreateElement("Fields");
            //Fields to be edited
            //XmlElement updateFields = xDoc.CreateElement("Fields");

            newFields.InnerXml = createStr; //"<Method ID=\"1\">"+createStr+"</Method>";
            //updateFields.InnerXml = updateStr; //"<Method ID=\"2\">"+updateStr+"</Method>";

            //XmlNode returnValue = dstWebs.UpdateColumns(newFields, updateFields, null);
            XmlNode returnValue = dstWebs.UpdateColumns(newFields, null, null);

            //Console.WriteLine(this.XmlToString(returnValue, 4));

            //Console.WriteLine("{0} elements to update\r\n{1} elements to create", updateColumns.Count, createColumns.Count);
        }


        public int GetSharePointVersion()
        {
            /*
             * HttpWebRequest webReq = HttpWebRequest.Create
	("http:/<site>/_vti_bin/shtml.dll/_vti_rpc") as HttpWebRequest;
webReq.Method = WebRequestMethods.Http.Post;
webReq.Credentials = new NetWorkCredential(user, passwd, domain);

StreamWriter strWriter = new StreamWriter(webReq.GetRequestStream());
strWriter.Write 
	("method=server version:server_extension_version&service_name=site_url=/");

HttpWebResponse WebResponse = webRequest.GetResponse() as HttpWebResponse;
String response = new StreamReader(WebResponse.GetResponseStream()).ReadToEnd();*/
            return 2013;
        }

        // todo
        public void CreateSiteCollection()
        {
            /*
             *  try
            {
                string url = allSrcWebsXml.ElementAt(0).Attributes["Url"].InnerText;

                dstAdmin.CreateSite(dstUrl,
                    allSrcWebsXml.ElementAt(0).Attributes["Title"].InnerText,
                    allSrcWebsXml.ElementAt(0).Attributes["Description"].InnerText,
                    Convert.ToInt32(allSrcWebsXml.ElementAt(0).Attributes["Language"].InnerText),
                    this.getSiteTemplate(),
                    dstDomain + "\\" + dstUser,
                    dstUser,
                    "",
                    "",
                    "");
            }
            catch (Exception sse)
            {
                throw sse;
                //throw new NotSupportedException("Your destination server does not support creating site collections with a web service.", sse);
            }*/
        }

        public void GetSiteTemplates()
        {
            /*
             * //get site templates
            string strDisplay = "";
            SitesWS.Template[] templates;
            srcSites.GetSiteTemplates((uint)(this.getLocale()), out templates);

            foreach (SitesWS.Template template in templates)
            {
                strDisplay = "Title: " + template.Title + "  Name: " + template.Name +
                    "  Description: " + template.Description + "  IsCustom: " +
                    template.IsCustom + "  ID: " + template.ID + "  ImageUrl: " + template.ImageUrl +
                    "  IsHidden: " + template.IsHidden + "  IsUnique: " + template.IsUnique + "\n\n";
                //Console.WriteLine(strDisplay + "\r\n################");
            }*/
        }


        
  

    }
}
