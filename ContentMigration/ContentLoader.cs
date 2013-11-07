﻿//-----------------------------------------------------------------------
// <copyright file="LoadingElementsException.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ContentMigration
{
    using System;
    using System.Collections.Generic;
    using System.Collections;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Sharezbold.ContentMigration.Data;
    using System.Xml.Linq;
    using System.Windows.Forms;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Diagnostics;
    using Sharezbold.Logging;

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
        /// the logger object
        /// </summary>
        private Logger log = null;
        
        /// <summary>
        /// Default constructor, takes the initialized web service class
        /// </summary>
        /// <param name="service">Web service access class</param>
        /// <param name="log">The logger class</param>
        public ContentLoader(WebService service, Logger log = null)
        {
            this.ws = service;
            if (log == null)
            {
                this.log = new Logger();
            }
            else
            {
                this.log = log;
            }
        }

        public SSiteCollection SourceSiteCollection { get; private set; }

        public SSiteCollection DestinationSiteCollection { get; private set; }

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
        /// a list with the destinaiton site urls - for checking if a new url is valid
        /// </summary>
        private List<string> destinationSiteUrls = new List<string>();

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
                
                // only do this, if destination site is being loaded
                if (!loadListData)
                {
                    destinationSiteUrls.Add(w.Attributes["Url"].InnerText);
                }
                 
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
        public async Task<bool> MigrateAllAsync()
        {
            /* whole migration
             * 
             * 1. Transfer users and groups
             * 2. Transfer all metadata as:
             *      2.1 site columns
             *      2.2 content types (depend on site columns)
             *      2.3 site templates
             *   
             * 
             *  3. Transfer site collection - done
             *  4. Transfer sites
             *      4.1 Transfer site itself
             *      4.2 Transfer list and library definitions
             *      4.3 Transfert list and library data
             *  5. Set permissions
             *  
             *  6. Transfer files
             */

            // specifies whether to go on with the migration
            bool keepGoing = true;

            // migrate site collection
            if (this.SourceSiteCollection.Migrate)
            {
                keepGoing = await this.MigrateSiteCollectionAsync();
            }

            // migrate sites
            foreach (SSite site in this.SourceSiteCollection.Sites)
            {
                if (site.Migrate && keepGoing)
                {
                    keepGoing = await this.MigrateSiteAsync(site, this.DestinationSiteCollection);
                }

                //migrate its lists and views
                if (keepGoing)
                {
                    foreach (SList list in site.Lists)
                    {
                        if (list.Migrate)
                        {
                            await this.MigrateListAndViewsAsync(list);
                            // lists are independent parts - no need to write its results to "keepGoing"
                        }
                    }
                }

                // TODO: migrate list data
            }

            if (!keepGoing)
            {
                this.log.AddMessage("MIGRATION ABORTED");
            }          
             
            // migrate site columns
            // TODO: implement for all sites
            /*
            Task<bool> taskSiteColumns = Task.Factory.StartNew(() =>
            {
                try
                {
                    log.Items.Add("Starting Site Columns migration");
                    this.MigrateSiteColumns();
                    log.Items.Add("Site Columns migration finished");
                    return true;
                }
                catch (Exception e)
                {
                    log.Items.Add("Error (Site Column Migration): " + e.Message);
                    return false;
                }
            });
            await taskSiteColumns;
             * */
            

            /*
             * 1. Site collection to migrate?
             *      --> migrate site collection
             * 2. Sites to migrate
             *      list of sites
             * 3. All lists without source site
             */
            return keepGoing;
        }

        /// <summary>
        /// Migrate a site
        /// </summary>
        /// <param name="src">source site to migrate</param>
        /// <param name="dstSC">destination site collection</param>
        public async Task<bool> MigrateSiteAsync(SSite src, SSiteCollection dstSC)
        {
            Task<bool> t = Task.Factory.StartNew(() =>
            {
                try
                {
                    /// mirate the site f:
                    ///   - if it is not the site collection site (its a conventional subsite)
                    ///   - if it is the site collection site and the site collection is not migrated (then the contents are migrated to a conventional site)
                    if (!src.IsSiteCollectionSite || (src.IsSiteCollectionSite && !this.SourceSiteCollection.Migrate))
                    {
                        this.log.AddMessage("Migrating site \"" + src.Name + "\" started");

                        string url = Regex.Replace(src.XmlData.Attributes["Title"].InnerText, @"[^A-Za-z0-9_\.~]+", "-");

                        //find a fitting url
                        int i = 1;
                        string newUrl = url;
                        while (destinationSiteUrls.FindAll(delegate(string s) { return s.EndsWith(newUrl); }).Count > 0)
                        {
                            newUrl = url + i.ToString();
                            i++;
                        }
                        url = newUrl;
                        
                        string title = src.XmlData.Attributes["Title"].InnerText;
                        string description = src.XmlData.Attributes["Description"].InnerText;
                        string templateName = this.GetSiteTemplate(src.XmlData.Attributes["Url"].InnerText);
                        uint language = this.GetLanguage(this.SourceSiteCollection);
                        bool languageSpecified = true;
                        uint locale = this.GetLocale();
                        bool localeSpecified = true;
                        uint collationLocale = locale;
                        bool collationLocaleSpecified = true;
                        bool uniquePermissions = true;
                        bool uniquePermissionsSpecified = true;
                        bool anonymous = true;
                        bool anonymousSpecified = true;
                        bool presence = true;
                        bool presenceSpecified = true;

                        try
                        {
                            this.ws.DstSites.CreateWeb(url, title, description, templateName, language, languageSpecified, locale, localeSpecified, collationLocale, collationLocaleSpecified, uniquePermissions, uniquePermissionsSpecified, anonymous, anonymousSpecified, presence, presenceSpecified);
                        }
                        catch (Exception e)
                        {
                            // there is always an error... no matter what you do (XML error, altough I couldn't have made one here)
                            // has to be ignored
                            //this.log.AddMessage("Site migration of site \"" + src.Name + "\" FAILED. Check if a site with the same name already exists! Error message: " + e.Message);
                            //return false;
                        }

                        try
                        {
                            //definately runtime error TODO
                            this.MigrateSiteColumnsAsync(src.XmlData.Attributes["Url"].InnerText, url);
                        }
                        catch (Exception e)
                        { }
                        //}

                        this.log.AddMessage("Migrating site \"" + src.Name + "\" finished");

                    }
                    return true;
                }
                catch (Exception e)
                {
                    // here only a http error could occurr
                    this.log.AddMessage("Migrating site \"" + src.Name + "\" error: " + e.Message);
                    return false;
                }
            });
            return await t;
        }

        /// <summary>
        /// Migrates a list and its views. Site Columns are not included
        /// </summary>
        /// <param name="list"></param>
        public async Task<bool> MigrateListAndViewsAsync(SList list)
        {
            Task<bool> t = Task.Factory.StartNew(() =>
            {
                // if list with the same name exists --> delete
                XmlElement l = (XmlElement)list.XmlList;
                string listName = l.Attributes["Title"].InnerText;

                string urlBuffer = this.ws.DstLists.Url;
                this.ws.DstLists.Url = list.MigrateTo.XmlData.Attributes["Url"].InnerText + WebService.UrlLists;
                try
                {
                    this.ws.DstLists.DeleteList(listName);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Log: " + e.Message);
                }

                //add list from source
                this.ws.DstLists.AddList(listName, l.Attributes["Description"].InnerText, Convert.ToInt32(l.Attributes["ServerTemplate"].InnerText));

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
                string viewUrlBuffer = this.ws.DstViews.Url;

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
                //Console.WriteLine(viewCollection.OuterXml);
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
                this.ws.DstViews.Url = viewUrlBuffer;
                this.ws.DstLists.Url = urlBuffer;
                return true;
            });
            return await t;
        }

        /// <summary>
        /// Migrates all new site columns from src to dst. Columns which changed are ignored, as "system columns". These unfortunately can't
        /// be  recognised yet.
        /// </summary>
        /// <param name="src">source site to migrate</param>
        /// <param name="dst">destination site to migrate</param>
        public async Task<bool> MigrateSiteColumnsAsync(SSite src, SSite dst)
        {
            return await this.MigrateSiteColumnsAsync(src.XmlData.Attributes["Url"].InnerText, dst.XmlData.Attributes["Url"].InnerText);
        }


        /// <summary>
        /// Migrates all new site columns from src to dst. Columns which changed are ignored, as "system columns". These unfortunately can't
        /// be  recognised yet.
        /// </summary>
        /// <param name="src">source site to migrate</param>
        /// <param name="dst">destination site to migrate</param>
        public async Task<bool> MigrateSiteColumnsAsync(string src, string dst)
        {
            Task<bool> t = Task.Factory.StartNew(() =>
            {
                string websSrcUrlBuffer = this.ws.SrcWebs.Url;
                string websDstUrlBuffer = this.ws.DstWebs.Url;

                try
                {
                    this.log.AddMessage("Migrating site columns started");
                    
                    this.ws.SrcWebs.Url = src;
                    this.ws.DstWebs.Url = dst;

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

                    this.log.AddMessage("Migriting site columns: preparing " + createColumns.Count + " columns for migration");

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
                }
                catch (Exception e)
                {
                    this.log.AddMessage("Migrating site columns error: " + e.Message);
                }
                finally
                {
                    this.ws.SrcWebs.Url = websSrcUrlBuffer;
                    this.ws.DstWebs.Url = websDstUrlBuffer;
                }
                return true;

            });
            return await t;
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
        /// Retreives the site template from the HTML site. Web services offer no possibility to do this.
        /// </summary>
        /// <returns>the site template name</returns>
        public string GetSiteTemplate(string url="")
        {
            // get HTML site
            if (url.Equals(""))
            {
                url = this.ws.SrcUrl;
            }
            WebRequest request = WebRequest.Create(url);
            request.Credentials = this.ws.SrcCredentials;
            request.PreAuthenticate = true;
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            dataStream.Close();

            //lets play "find the template"....
            //SP2010 & 2013: var g_wsaSiteTemplateId = 'STS#1';
            int startHere = responseFromServer.IndexOf("g_wsaSiteTemplateId");
            string hereSomewhere = responseFromServer.Substring(startHere, 100);
            string[] parts = hereSomewhere.Split('\'');

            return parts[1];
        }

        /// <summary>
        /// Extracts the language number
        /// </summary>
        /// <param name="sc">the site collection to look for</param>
        /// <returns>the language code</returns>
        public uint GetLanguage(SSiteCollection sc)
        {
            return Convert.ToUInt32(sc.XmlData.ElementAt(0).Attributes["Language"].InnerText);
        }

        /// <summary>
        /// Migrates a site collection
        /// </summary>
        public async Task<bool> MigrateSiteCollectionAsync()
        {
            Task<bool> t = Task.Factory.StartNew(() =>
            {
                XmlNode scXml = this.SourceSiteCollection.XmlData.ElementAt(0);
                string url = scXml.Attributes["Url"].InnerText;

                this.log.AddMessage("Migrating site collection \"" + scXml.Attributes["Title"] + "\"");
                
                try
                {
                    this.ws.DstAdmin.CreateSite(ws.DstUrl,
                        scXml.Attributes["Title"].InnerText,
                        scXml.Attributes["Description"].InnerText,
                        (int)this.GetLanguage(this.SourceSiteCollection),
                        this.GetSiteTemplate(),
                        ws.DstDomain + "\\" + ws.DstUser,
                        ws.DstUser,
                        "",
                        "",
                        "");
                }
                catch (Exception e)
                {
                    this.log.AddMessage("Error migrating site collection \"" + scXml.Attributes["Title"] + "\". Message: " + e.Message);
                }
                finally
                {
                    this.log.AddMessage("Migrating site collection \"" + scXml.Attributes["Title"] + "\" finished");
                }
                return true;
            });

            return await t;
        }

        /// <summary>
        /// Retreives the locale of a random list. There is no "regular" way to get the locale with a web service
        /// </summary>
        /// <returns>Sharepoint LCID locale code</returns>
        private uint GetLocale()
        {
            XmlNode lc = this.ws.SrcLists.GetListCollection();
            Console.WriteLine(lc.ChildNodes[0].Attributes["Name"].InnerText);
            XmlNode list = this.ws.SrcLists.GetList(lc.ChildNodes[0].Attributes["Name"].InnerText);
            return Convert.ToUInt32(list["RegionalSettings"]["Locale"].InnerText);
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
