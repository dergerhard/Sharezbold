

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
    using System.Windows.Forms;
    using System.IO;

    /// <summary>
    /// Is responsible for downloading/uploading data from the source to the destinaiton server
    /// </summary>
    public class ContentLoader
    {
        /// <summary>
        /// provides access to the web services
        /// </summary>
        private WebService ws;

        /// <summary>
        /// Default constructor, takes the initialized web service class
        /// </summary>
        /// <param name="service">Web service access class</param>
        public ContentLoader(WebService service)
        {
            this.ws = service;
        }

        public SSiteCollection SourceSiteCollection { get; private set; }

        public SSiteCollection DestinationSiteCollection { get; private set; }

        /*
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
        }*/

         

        /// <summary>
        /// Loads the source site collectionand stores it
        /// </summary>
        /// <returns>the source site collection</returns>
        public SSiteCollection LoadSourceSiteCollection()
        {
            this.SourceSiteCollection = this.LoadSharepointTree(this.ws.SrcWebs, this.ws.SrcLists, true);
            return this.SourceSiteCollection;
        }

        /// <summary>
        /// Loads the destination site collection and stores it
        /// </summary>
        /// <returns>the source site collection</returns>
        public SSiteCollection LoadDestinationSiteCollection()
        {
            this.DestinationSiteCollection = this.LoadSharepointTree(this.ws.DstWebs, this.ws.DstLists, false);
            return this.DestinationSiteCollection;
        }

        /// <summary>
        /// Loads the site collection of a Sharepoint server
        /// </summary>
        /// <param name="srcWebs">webs web service</param>
        /// <param name="srcLists">lists web service</param>
        /// <param name="loadListData">load data items or not</param>
        /// <returns></returns>
        private SSiteCollection LoadSharepointTree(WebsWS.Webs srcWebs, ListsWS.Lists srcLists, bool loadListData)
        {
            SSiteCollection siteCollection = new SSiteCollection();

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
            var allSrcWebsXml = new List<XmlNode>();
            
            foreach (KeyValuePair<string, string> web in webs)
            {
                // load details on each web
                XmlNode w = srcWebs.GetWeb(web.Key);
                allSrcWebsXml.Add(w);
                SSite site = new SSite();
                site.ParentObject = siteCollection;
                site.XmlData = w;

                string url = w.Attributes["Url"].InnerText + WebService.UrlLists;
                Console.WriteLine(url);

                // get all lists
                srcLists.Url = url;
                XmlNode lc = srcLists.GetListCollection();

                // lists to migrate: Hidden="False"
                foreach (XmlNode list in lc.ChildNodes)
                {
                    //site.AllLists.Add(list);

                    if (list.Attributes["Hidden"].InnerText.ToUpper().Equals("FALSE"))
                    {
                        // load list details with all fields
                        XmlNode listDetails = srcLists.GetList(list.Attributes["Title"].InnerText);
                        SList sList = new SList();
                        sList.ParentObject = site;
                        sList.XmlList = listDetails;

                        // load list data
                        if (loadListData)
                        {
                            XmlNode ndListItems = srcLists.GetListItems(list.Attributes["Title"].InnerText, null, null, null, null, null, null);
                            sList.XmlListData = ndListItems;
                        }
                        site.AddList(sList, false);
                        Console.WriteLine("\t\t" + list.Attributes["Title"].InnerText);
                    }
                }

                if (firstRun)
                {
                    site.IsSiteCollectionSite = true;
                    firstRun = false;
                }
                else
                {
                    site.IsSiteCollectionSite = false;
                }

                siteCollection.AddSite(site, false);
            }
            srcLists.Url = srcListsUrlBuffer;
            siteCollection.XmlData = allSrcWebsXml;

            return siteCollection;
        }

        /// <summary>
        /// Migrates all selected items from source to destination. Items must be set up by the user!
        /// </summary>
        public void MigrateAll(ListBox log)
        {
            try
            {
                log.Items.Add("Starting Site Columns migration");
                this.MigrateSiteColumns();
                log.Items.Add("Site Columns migration finished");
            }
            catch (Exception e)
            {
                log.Items.Add("Error (Site Column Migration): " + e.Message);
            }

            try
            {
                log.Items.Add("Starting Site Collection migration");
                this.MigrateSiteCollection();
                log.Items.Add("Site Collection migration finished");
            }
            catch (Exception e)
            {
                log.Items.Add("Error (Site Collection Migration): " + e.Message);
            }

            /*
             * 1. Site collection to migrate?
             *      --> migrate site collection
             * 2. Sites to migrate
             *      list of sites
             * 3. All lists without source site
             */
        }


        /// <summary>
        /// Migrates a list and its views. Site Columns are not included
        /// </summary>
        /// <param name="list"></param>
        public void MigrateList(SList list)
        {

            // if list with the same name exists --> delete
            XmlElement l = (XmlElement)list.XmlList;
            string listName = l.Attributes["Title"].InnerText;

            try
            {
                this.ws.DstLists.DeleteList(listName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Log: " + e.Message);
            }

            //add list from source
            this.ws.DstLists.AddList(listName,  l.Attributes["Description"].InnerText, Convert.ToInt32(l.Attributes["ServerTemplate"].InnerText));

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
            this.ws.DstLists.UpdateList(listName, listProperties, fields, fields, null, "");

            // migrate the views
            // first delete the dst views
            try
            {
                XmlNode viewsToDelete = this.ws.DstViews.GetViewCollection(listName);
                foreach (XmlNode view in viewsToDelete)
                {
                    this.ws.DstViews.DeleteView(listName, view.Attributes["Name"].InnerText);
                }
            }
            catch (Exception e)
            {
            }

            XmlNode viewCollection = this.ws.SrcViews.GetViewCollection(listName);
            Console.WriteLine(viewCollection.OuterXml);
            foreach (XmlNode view in viewCollection)
            {
                string viewName = view.Attributes["Name"].InnerText;
                XmlNode viewDetail = this.ws.SrcViews.GetView(listName, viewName);

                XmlDocument doc2 = new XmlDocument();
                XmlElement viewFields = (XmlElement)doc2.ImportNode(viewDetail["ViewFields"], true);

                XmlDocument doc3 = new XmlDocument();
                XmlElement query = (XmlElement)doc3.ImportNode(viewDetail["Query"], true);

                XmlDocument doc4 = new XmlDocument();
                XmlElement rowLimit = (XmlElement)doc4.ImportNode(viewDetail["RowLimit"], true);

                bool makeViewDefault = viewDetail.Attributes["DefaultView"].InnerText.ToUpper().Equals("TRUE") ? true : false;

                // add the view
                this.ws.DstViews.AddView(listName, view.Attributes["DisplayName"].InnerText, viewFields, query, rowLimit, viewDetail.Attributes["Type"].InnerText, makeViewDefault);
            }

        }


        /// <summary>
        /// Migrates all new site columns from src to dst. Columns which changed are ignored, as "system columns" can't
        /// be  recognised yet.
        /// </summary>
        public void MigrateSiteColumns()
        {
            XmlNode srcColumsXml = this.ws.SrcWebs.GetColumns();
            XmlNode dstColumnsXml = this.ws.DstWebs.GetColumns();

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
            XmlNode returnValue = this.ws.DstWebs.UpdateColumns(newFields, null, null);

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

        /// <summary>
        /// Checks if migration of site collection is possible (only if destination web application is empty)
        /// </summary>
        public bool IsSiteCollectionMigrationPossible
        {
            get
            {
                XmlNode dstWebs = this.ws.DstWebs.GetAllSubWebCollection();
                return dstWebs.ChildNodes.Count == 0 ? true : false;
            }
        }

        /// <summary>
        /// Retreives the site template from the HTML site. Web services offer no possibility to do this.
        /// </summary>
        /// <returns>the site template name</returns>
        public string GetSiteTemplate()
        {
            // get HTML site
            WebRequest request = WebRequest.Create(this.ws.SrcUrl);
            request.Credentials = this.ws.SrcCredentials;
            request.PreAuthenticate = true;
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            dataStream.Close();

            //letz play "find the template"....
            //SP2010 & 2013: var g_wsaSiteTemplateId = 'STS#1';
            int startHere = responseFromServer.IndexOf("g_wsaSiteTemplateId");
            string hereSomewhere = responseFromServer.Substring(startHere, 100);
            string[] parts = hereSomewhere.Split('\'');

            return parts[1];
        }

        /// <summary>
        /// Migrates a site collection
        /// </summary>
        public void MigrateSiteCollection()
        {
            List<XmlNode> scXml = this.SourceSiteCollection.XmlData;
            string url = scXml.ElementAt(0).Attributes["Url"].InnerText;

            this.ws.DstAdmin.CreateSite(ws.DstUrl,
                scXml.ElementAt(0).Attributes["Title"].InnerText,
                scXml.ElementAt(0).Attributes["Description"].InnerText,
                Convert.ToInt32(scXml.ElementAt(0).Attributes["Language"].InnerText),
                this.GetSiteTemplate(),
                ws.DstDomain + "\\" + ws.DstUser,
                ws.DstUser,
                "",
                "",
                "");
        }

        /// <summary>
        /// Retreives the locale of a random list. There is no "regular" way to get the locale with a web service
        /// </summary>
        /// <returns>Sharepoint LCID locale code</returns>
        private int GetLocale()
        {
            XmlNode lc = this.ws.SrcLists.GetListCollection();
            Console.WriteLine(lc.ChildNodes[0].Attributes["Name"].InnerText);
            XmlNode list = this.ws.SrcLists.GetList(lc.ChildNodes[0].Attributes["Name"].InnerText);
            return Convert.ToInt32(list["RegionalSettings"]["Locale"].InnerText);
        }
        
        /// <summary>
        /// retreives the site template
        /// </summary>
        public void GetSiteTemplates()
        {
             //get site templates
            string strDisplay = "";
            SitesWS.Template[] templates;
            ws.SrcSites.GetSiteTemplates((uint)(this.GetLocale()), out templates);

            foreach (SitesWS.Template template in templates)
            {
                strDisplay = "Title: " + template.Title + "  Name: " + template.Name +
                    "  Description: " + template.Description + "  IsCustom: " +
                    template.IsCustom + "  ID: " + template.ID + "  ImageUrl: " + template.ImageUrl +
                    "  IsHidden: " + template.IsHidden + "  IsUnique: " + template.IsUnique + "\n\n";
                //Console.WriteLine(strDisplay + "\r\n################");
            }
        }


        
  

    }
}
