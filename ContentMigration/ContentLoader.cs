//-----------------------------------------------------------------------
// <copyright file="ContentLoader.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.ContentMigration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Linq;
    using ElementsMigration;
    using Extensions;
    using Microsoft.SharePoint.Client;
    using Sharezbold.ContentMigration.Data;
    using Sharezbold.Logging;
    
    /// <summary>
    /// Is responsible for downloading/uploading data from the source to the destination server
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
        /// Data for migration.
        /// </summary>
        private MigrationData migrationData;

        /// <summary>
        /// A list with the destination site URLs - for checking if a new url is valid
        /// </summary>
        private List<string> destinationSiteUrls = new List<string>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLoader" /> class.  Default constructor, takes the initialized web service class
        /// </summary>
        /// <param name="service">Web service access class</param>
        /// <param name="migrationData">data for migration</param>
        /// <param name="log">The logger class</param>
        public ContentLoader(WebService service, MigrationData migrationData, Logger log = null)
        {
            this.ws = service;
            this.migrationData = migrationData;

            if (log == null)
            {
                this.log = new Logger();
            }
            else
            {
                this.log = log;
            }
        }

        /// <summary>
        /// Gets the source site collection
        /// </summary>
        public SSiteCollection SourceSiteCollection { get; private set; }

        /// <summary>
        /// Gets the destination site collection
        /// </summary>
        public SSiteCollection DestinationSiteCollection { get; private set; }

        /// <summary>
        /// Loads the source site collection and stores it
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
        /// Migrates all selected items from source to destination. Items must be set up by the user!
        /// </summary>
        /// <returns>If successful or not</returns>
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
            
            this.log.AddMessage("Migration started");

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
                    // keepGoing = await this.MigrateSiteAsync(site, this.DestinationSiteCollection);
                    keepGoing = await this.MigrateSiteAsync(site, this.DestinationSiteCollection);
                }

                // migrate its lists and views
                if (keepGoing)
                {
                    foreach (SList list in site.Lists)
                    {
                        if (list.Migrate)
                        {
                            // lists are independent parts - no need to write its results to "keepGoing"
                            // list data is migrated withing MigrateListAndViews
                            await this.MigrateListAndViewsAsync(list);
                        }
                    }
                }
            }

            if (!keepGoing)
            {
                this.log.AddMessage("Migration ABORTED");
            }
            else
            {
                this.log.AddMessage("Migration finished");
            }
 
            // file migration:
            // remove false.... ;) only to block it for testing
            if (keepGoing && false)  
            {
                foreach (string item in this.migrationData.WebUrlsToMigrate)
                {
                    Dictionary<Web, Web> sourceTargetWebs = item.GetRelativeUrlOfWeb(this.migrationData.SourceClientContext, this.migrationData.TargetClientContext);

                    foreach (var entry in sourceTargetWebs)
                    {
                        this.migrationData.FileMigrator.MigrateFilesOfWeb(entry.Key, entry.Value);
                    }
                }
            }

            return keepGoing;
        }

        /// <summary>
        /// Migrate a site
        /// </summary>
        /// <param name="src">source site to migrate</param>
        /// <param name="dstSC">destination site collection</param>
        /// <returns>If successful or not</returns>
        public async Task<bool> MigrateSiteAsync(SSite src, SSiteCollection dstSC)
        {
            // mirate the site f:
            //   - if it is not the site collection site (its a conventional subsite)
            //   - if it is the site collection site and the site collection is not migrated (then the contents are migrated to a conventional site)
            if (!src.IsSiteCollectionSite || (src.IsSiteCollectionSite && !this.SourceSiteCollection.Migrate))
            {
                this.log.AddMessage("Migrating site \"" + src.Name + "\" started");

                string url = Regex.Replace(src.XmlData.Attributes["Title"].InnerText, @"[^A-Za-z0-9_\.~]+", "-");

                // find a fitting url
                int i = 1;
                string newUrl = url;
                while (this.destinationSiteUrls.FindAll(delegate(string s) { return s.EndsWith(newUrl); }).Count > 0)
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
                uint locale = await this.GetLocale();
                bool localeSpecified = true;
                uint collationLocale = locale;
                bool collationLocaleSpecified = true;
                bool uniquePermissions = true;
                bool uniquePermissionsSpecified = true;
                bool anonymous = true;
                bool anonymousSpecified = true;
                bool presence = true;
                bool presenceSpecified = true;

                Task<bool> createWeb = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        this.ws.DstSites.CreateWeb(url, title, description, templateName, language, languageSpecified, locale, localeSpecified, collationLocale, collationLocaleSpecified, uniquePermissions, uniquePermissionsSpecified, anonymous, anonymousSpecified, presence, presenceSpecified);
                        return true;
                    }
                    catch (Exception e)
                    {
                        // there is always an error... no matter what you do (XML error, altough I couldn't have made one here)
                        // has to be ignored
                        Debug.WriteLine(e.Message);
                        return false;
                    }
                });

                if (await createWeb)
                {
                    Task<XmlNode> getWebs = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            return this.ws.DstWebs.GetWebCollection();
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                            return (new XmlDocument()).CreateElement("null");
                        }
                    });

                    XmlNode webs = await getWebs;
                    foreach (XmlNode web in webs)
                    {
                        if (web.Attributes["Url"].InnerText.EndsWith(url))
                        {
                            url = web.Attributes["Url"].InnerText;
                        }
                    }

                    // set the new migrate to 
                    foreach (var l in src.Lists)
                    {
                        l.MigrateToUrl = url;
                    }

                    await this.MigrateSiteColumnsAsync(src.XmlData.Attributes["Url"].InnerText, url); 
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Migrates a list and its views. Site Columns are not included
        /// </summary>
        /// <param name="list">The list to migrate</param>
        /// <returns>If successful or not</returns>
        private async Task<bool> MigrateListAndViewsAsync(SList list)
        {
            this.log.AddMessage("Migrate lists starting migration of list \"" + list.Name + "\"");
            this.log.Indent = this.log.Indent + 1;

            // if list with the same name exists --> delete
            XmlElement l = (XmlElement)list.XmlList;
            string listName = l.Attributes["Title"].InnerText;

            this.ws.SetListsMigrateTo(list);

            Task<string> deleteList = Task.Factory.StartNew(() =>
            {
                try
                {
                    this.ws.DstLists.DeleteListAsync(listName);
                    return "Migrate lists found list with same title at destinaiton and deleted it";
                }
                catch (Exception e)
                {
                    return "Exception: " + e.Message;
                }
            });
            string res = await deleteList;
            if (res.StartsWith("Exception"))
            {
                this.log.AddMessage(res, true);
            }
            else
            {
                this.log.AddMessage(res);
            }
            
            // add list from source
            Task<KeyValuePair<string, bool>> addList = Task.Factory.StartNew(() =>
            {
                try
                {
                    Thread.Sleep(1000);
                    this.ws.DstLists.AddList(listName, l.Attributes["Description"].InnerText, Convert.ToInt32(l.Attributes["ServerTemplate"].InnerText));
                    return new KeyValuePair<string, bool>("Migrate lists added the new list", false);
                }
                catch (Exception e)
                {
                    // if this occurrs, the list already exists - altought this should not be possible as it is deleted before, it still happens
                    return new KeyValuePair<string, bool>("Adding the list \"" + listName + "\" failed. Error: " + e.Message, true);
                }
            });

            var response = await addList;
            this.log.AddMessage(response.Key, response.Value);
                
            // copy list properties
            XmlDocument doc = new XmlDocument();
            foreach (XmlAttribute attr in l.Attributes)
            {
                XmlElement listProperties = doc.CreateElement("List");
                listProperties.SetAttribute(attr.Name, attr.Value);
                Task<string> updateList = Task.Factory.StartNew(() => 
                {
                    try
                    {
                        this.ws.DstLists.UpdateList(listName, listProperties, null, null, null, string.Empty);
                        return string.Empty;
                    }
                    catch (Exception e)
                    {
                        return "added attribute FAILED: " + attr.Name + ". Error: " + e.Message;
                    }
                });

                res = await updateList;
                if (res.Equals(string.Empty))
                {
                    this.log.AddMessage("Added list property \"" + attr.Name + "\"", true);
                }
                else
                {
                    this.log.AddMessage(res, true);
                }
            }

            this.log.AddMessage("List properties added");
            int i = 1;
            
            foreach (XmlNode field in l["Fields"])
            {
                if (field.Name.Equals("Field"))
                {
                    XmlElement fields = doc.CreateElement("Fields");

                    XmlElement method = doc.CreateElement("Method");
                    method.SetAttribute("ID", i.ToString());
                    XmlNode newField = doc.ImportNode(field, true);
                    newField.Attributes.RemoveNamedItem("ID");
                    method.AppendChild(newField);
                    fields.AppendChild(method);

                    Task<string> updateList2 = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            this.ws.DstLists.UpdateList(listName, null, fields, fields, null, string.Empty);
                            return string.Empty;
                        }
                        catch (Exception e)
                        {
                            return "Migrate lists migration of fields failed. XML: " + fields.OuterXml + " Error: " + e.Message;
                        }
                    });

                    this.log.AddMessage(await updateList2, true);
                    
                    i++;
                }
            }

            this.log.AddMessage("Fields added");

            // migrate the views
            // first delete the dst views
            this.ws.SetViewsMigrateTo(list);

            XmlNode viewsToDelete = null;

            Task<string> vc = Task.Factory.StartNew(() =>
            {
                try
                {
                    viewsToDelete = this.ws.DstViews.GetViewCollection(listName);
                    return string.Empty;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return string.Empty;
                }
            });

            this.log.AddMessage(await vc);

            if (viewsToDelete != null)
            {
                foreach (XmlNode view in viewsToDelete)
                {
                    Task<string> deleteView = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            this.ws.DstViews.DeleteView(listName, view.Attributes["Name"].InnerText);
                            return string.Empty;
                        }
                        catch (Exception e)
                        {
                            return "Migrate lists, view deletion: " + e.Message;
                        }
                    });

                    this.log.AddMessage(await deleteView, true);
                }
            }

            Task<XmlNode> viewColl = Task.Factory.StartNew(() =>
            {
                try
                {
                    return this.ws.SrcViews.GetViewCollection(listName);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return (new XmlDocument()).CreateElement("null");
                }
            });

            XmlNode viewCollection = await viewColl;
            string viewID = string.Empty;

            foreach (XmlNode view in viewCollection)
            {
                if (!view.Attributes["DisplayName"].InnerText.Equals(string.Empty))
                {
                    string viewName = view.Attributes["Name"].InnerText;

                    Task<XmlNode> viewDet = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            return this.ws.SrcViews.GetView(listName, viewName);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                            return (new XmlDocument()).CreateElement("null");
                        }
                    });

                    XmlNode viewDetail = await viewDet;

                    // prepare elements
                    XmlDocument doc2 = new XmlDocument();
                    XmlElement viewFields;
                    XmlDocument doc3 = new XmlDocument();
                    XmlElement query;
                    XmlDocument doc4 = new XmlDocument();
                    XmlElement rowLimit;

                    try
                    {
                        viewFields = (XmlElement)doc2.ImportNode(viewDetail["ViewFields"], true);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        viewFields = doc2.CreateElement("ViewFields");
                    }

                    try
                    {
                        query = (XmlElement)doc3.ImportNode(viewDetail["Query"], true);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        query = doc.CreateElement("Query");
                    }

                    try
                    {
                        rowLimit = (XmlElement)doc4.ImportNode(viewDetail["RowLimit"], true);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        rowLimit = doc.CreateElement("RowLimit");
                    }

                    bool makeViewDefault = false;
                    try
                    {
                        makeViewDefault = viewDetail.Attributes["DefaultView"].InnerText.ToUpper().Equals("TRUE") ? true : false;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }

                    // add the view
                    Task<string> addView2 = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            string viewname = view.Attributes["DisplayName"].InnerText;
                            XmlNode resp = this.ws.DstViews.AddView(listName, viewname, viewFields, query, rowLimit, viewDetail.Attributes["Type"].InnerText, makeViewDefault);

                            if (makeViewDefault)
                            {
                                viewID = resp.Attributes["Name"].InnerText; // viewname;
                            }

                            return "Migrate lists added view \"" + viewname + "\"";
                        }
                        catch (Exception e)
                        {
                            return "Migrate lists add view error at \"" + view.Attributes["DisplayName"].InnerText + "\": " + e.Message;
                        }
                    });

                    this.log.AddMessage(await addView2);
                }
            }
            
            // now migrate all items
            await this.MigrateListData(list, viewID);

            this.log.Indent = this.log.Indent - 1;
            this.log.AddMessage("Migrate lists finished");
            
            return true;
        }

        /// <summary>
        /// Migrates a list data.
        /// </summary>
        /// <param name="list">The list whose data will be migrated</param>
        /// <param name="viewID">The id of the view</param>
        /// <returns>If successful or not</returns>
        private async Task<bool> MigrateListData(SList list, string viewID)
        {
            /*
             * 
             * <rs:data ItemCount="2" xmlns:rs="urn:schemas-microsoft-com:rowset">
             * <z:row 
             *          ows_CompanyName="IBM" 
             *          ows_ApplicationDate="2013-11-04 00:00:00" 
             *          ows_ContactPerson="John Doe" 
             *          ows_Interview="Yes" 
             *          ows_MetaInfo="1;#" 
             *          ows__ModerationStatus="0" 
             *          ows__Level="1" 
             *          ows_ID="1" 
             *          ows_UniqueId="1;#{C99A189F-FCC5-4FF5-8DC5-298B9DA1C579}" 
             *          ows_owshiddenversion="1" 
             *          ows_FSObjType="1;#0" 
             *          ows_Created="2013-11-05 16:50:12" 
             *          ows_PermMask="0x7fffffffffffffff" 
             *          ows_Modified="2013-11-05 16:50:12" 
             *          ows_FileRef="1;#Lists/MyJobApplications/1_.000" 
             *          xmlns:z="#RowsetSchema" />
             *          
             *          how to do it:
             *          1. get the ids and all information
             *          2. create the elements only with its IDs
             *          3. Update every attribute in own Method for failsafe
             *          
             *          create ONE element with name and title
             * 
             */
            this.ws.SetListsMigrateTo(list);

            Task<string> listID = Task.Factory.StartNew(() =>
            {
                try
                {
                    XmlNode ndListView = this.ws.DstLists.GetListAndView(list.Name, string.Empty);
                    return ndListView.ChildNodes[0].Attributes["Name"].InnerText;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return string.Empty;
                }
            });

            string strListID = await listID;
            string strViewID = viewID;

            var migrationElements = new Dictionary<int, Dictionary<string, string>>();

            // get the items from the list
            XmlElement listdata = (XmlElement)list.XmlListData;
            
            // iterate through z:row-nodes
            foreach (XmlLinkedNode row in listdata["rs:data"])
            {
                if (row.GetType() == typeof(XmlElement))
                {
                    int id = int.Parse(row.Attributes["ows_ID"].InnerText);
                    var attrs = new Dictionary<string, string>();

                    XmlElement el = (XmlElement)row;
                    
                    // iterate through attributes
                    foreach (XmlAttribute attr in el.Attributes)
                    {
                        if (attr.Name.StartsWith("ows_") && !attr.Name.Equals("ows_ID"))
                        {
                            attrs[attr.Name.Substring(4)] = attr.Value;
                        }
                    }

                    migrationElements[id] = attrs;
                }
            }

            // 1. create the element without any information
            // 2. create all the element information in isolated "methods" in one batch to prevent one error from blocking the transfer
            int elementCount = 0;
            foreach (var el in migrationElements)
            {
                XmlDocument doc = new XmlDocument();
                XmlElement batchElement = doc.CreateElement("Batch");
                batchElement.SetAttribute("OnError", "Continue");
                batchElement.SetAttribute("ListVersion", "1");
                batchElement.SetAttribute("ViewName", strViewID);
        
                // create new element with id and title
                var method = doc.CreateElement("Method");
                method.SetAttribute("ID", "1");
                method.SetAttribute("Cmd", "New");

                var field = doc.CreateElement("Field");
                field.SetAttribute("Name", "ID");
                field.InnerText = el.Key.ToString();
                method.AppendChild(field);
                
                batchElement.AppendChild(method);

                Task<KeyValuePair<bool, XmlNode>> createNewListItem = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        // create the element only wit its ID
                        return new KeyValuePair<bool, XmlNode>(true, this.ws.DstLists.UpdateListItems(strListID, batchElement));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        return new KeyValuePair<bool, XmlNode>(false, null);
                    }
                });

                KeyValuePair<bool, XmlNode> newListItem = await createNewListItem;
                
                if (newListItem.Key == true)
                {
                    XmlNode res = newListItem.Value;
                    string newID = res["Result"]["z:row"].Attributes["ows_ID"].InnerText;

                    doc = new XmlDocument();
                    batchElement = doc.CreateElement("Batch");
                    batchElement.SetAttribute("OnError", "Continue");
                    batchElement.SetAttribute("ListVersion", "1");
                    batchElement.SetAttribute("ViewName", strViewID);

                    int i = 1;
                    
                    // create all following elements
                    foreach (var attribute in el.Value)
                    {
                        if (!attribute.Key.Equals("ID"))
                        {
                            var method2 = doc.CreateElement("Method");
                            method2.SetAttribute("ID", i.ToString());
                            method2.SetAttribute("Cmd", "Update");

                            var field2 = doc.CreateElement("Field");
                            field2.SetAttribute("Name", "ID");
                            field2.InnerText = newID;
                            method2.AppendChild(field2);

                            var field3 = doc.CreateElement("Field");
                            field3.SetAttribute("Name", attribute.Key);
                            field3.InnerText = attribute.Value;
                            method2.AppendChild(field3);

                            batchElement.AppendChild(method2);
                            i++;
                        }
                    }

                    Task<string> updateNewItem = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            // updating the new item with all other fields
                            // it is done that way to isolate each field update
                            this.ws.DstLists.UpdateListItems(strListID, batchElement);
                            return string.Empty;
                        }
                        catch (Exception e)
                        {
                            return "Error while transfering list data. Error message: " + e.Message;
                        }
                    });

                    this.log.AddMessage(await updateNewItem, true);
                }

                elementCount++;
            }

            this.log.AddMessage("Migrated " + elementCount.ToString() + " items to destination");
            return true;
        }

        /// <summary>
        /// Loads the site collection of a Sharepoint server
        /// </summary>
        /// <param name="srcWebs">webs web service</param>
        /// <param name="srcLists">lists web service</param>
        /// <param name="loadListData">load data items or not</param>
        /// <returns>A Sharepoint site collection tree</returns>
        private SSiteCollection LoadSharepointTree(WebsWS.Webs srcWebs, ListsWS.Lists srcLists, bool loadListData)
        {
            SSiteCollection siteCollection = new SSiteCollection();

            // get all webs names (first is the site collection)
            XmlNode allSrcWebs = srcWebs.GetAllSubWebCollection();

            // result<List>: <Web Title="Fucking site collection" Url="http://ss13-css-009:31920" xmlns="http://schemas.microsoft.com/sharepoint/soap/" />
            Dictionary<string, string> webs = new Dictionary<string, string>();
            foreach (XmlNode web in allSrcWebs)
            {
                webs.Add(web.Attributes["Url"].InnerText, web.Attributes["Title"].InnerText);
            }

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
                    this.destinationSiteUrls.Add(w.Attributes["Url"].InnerText);
                }

                // get all lists
                srcLists.Url = url;
                XmlNode lc = srcLists.GetListCollection();

                // lists to migrate: Hidden="False"
                foreach (XmlNode list in lc.ChildNodes)
                {
                    // if BaseType==1 --> its a document library
                    if (list.Attributes["Hidden"].InnerText.ToUpper().Equals("FALSE") && !list.Attributes["BaseType"].InnerText.Equals("1"))
                    {
                        // load list details with all fields
                        XmlNode listDetails = srcLists.GetList(list.Attributes["Title"].InnerText);
                        Console.WriteLine(list.Attributes["Title"].InnerText + ", BaseType=" + listDetails.Attributes["BaseType"].InnerText);
                        SList shareList = new SList();
                        shareList.ParentObject = site;
                        shareList.XmlList = listDetails;

                        // load list data
                        if (loadListData)
                        {
                            // attention: GetListItems only returns the elements of the default view, if you do not specify the viewfields you want
                            XmlDocument xmlDoc = new System.Xml.XmlDocument();
                            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");

                            XmlElement field;
                            foreach (XmlElement f in shareList.XmlList["Fields"])
                            {
                                field = xmlDoc.CreateElement("FieldRef");
                                field.SetAttribute("Name", f.Attributes["Name"].InnerText);
                                viewFields.AppendChild(field);
                            }

                            XmlNode listItems = srcLists.GetListItems(list.Attributes["Title"].InnerText, null, null, viewFields, null, null, null);
                            shareList.XmlListData = listItems;
                        }

                        site.AddList(shareList, false);
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
        /// Migrates all new site columns from src to dst. Columns which changed are ignored, as "system columns". These unfortunately can't
        /// be  recognized yet.
        /// </summary>
        /// <param name="src">source site to migrate</param>
        /// <param name="dst">destination site to migrate</param>
        /// <returns>If successful or not</returns>
        private async Task<bool> MigrateSiteColumnsAsync(string src, string dst)
        {
            this.ws.SetWebsMigrateFrom(src);
            this.ws.SetWebsMigrateTo(dst);

            this.log.AddMessage("Migrating site columns started");

            Task<XmlNode> getSrcCols = Task.Factory.StartNew(() => { return this.ws.SrcWebs.GetColumns(); });
            XmlNode srcColumsXml = await getSrcCols;

            Task<XmlNode> getDstCols = Task.Factory.StartNew(() => { return this.ws.DstWebs.GetColumns(); });
            XmlNode dstColumnsXml = await getDstCols;

            // convert to xdoc
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
                if (!dstColumns.ContainsKey(el.Attribute("ID").Value))
                {
                    createColumns.Add(el);
                }
            }

            // now columns that have to be updated or created are identified
            // time to create or update them
            int id = 1;

            this.log.AddMessage("Migriting site columns: preparing " + createColumns.Count + " columns for migration");

            string createStr = string.Empty;
            foreach (XElement el in createColumns)
            {
                createStr += "<Method ID=\"" + id++ + "\" Cmd=\"New\">" + el.ToString() + "</Method>";
            }

            XmlDocument createDoc = new XmlDocument();
            createDoc.LoadXml("<Fields>" + createStr + "</Fields>");
            XmlNode createNode = createDoc.DocumentElement;

            XmlDocument xmlDoc = new XmlDocument();

            // Fields to be added
            XmlElement newFields = xmlDoc.CreateElement("Fields");
            
            newFields.InnerXml = createStr; 

            Task<string> updateColumns = Task.Factory.StartNew(() =>
            {
                try
                {
                    this.ws.DstWebs.UpdateColumns(newFields, null, null);
                    return string.Empty;
                }
                catch (Exception e)
                {
                    return "Migrating site columns error: " + e.Message;
                }
            });

            this.log.AddMessage(await updateColumns);
            return true;
        }

        /// <summary>
        /// Retrieves the site template from the HTML site. Web services offer no possibility to do this.
        /// </summary>
        /// <param name="url">The URL to the site</param>
        /// <returns>The site template name</returns>
        private string GetSiteTemplate(string url = "")
        {
            // get HTML site
            if (url.Equals(string.Empty))
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

            // lets play "find the template"....
            // SP2010 & 2013: var g_wsaSiteTemplateId = 'STS#1';
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
        private uint GetLanguage(SSiteCollection sc)
        {
            return Convert.ToUInt32(sc.XmlData.ElementAt(0).Attributes["Language"].InnerText);
        }

        /// <summary>
        /// Migrates a site collection
        /// </summary>
        /// <returns>If successful or not</returns>
        private async Task<bool> MigrateSiteCollectionAsync()
        {
            XmlNode sourceXml = this.SourceSiteCollection.XmlData.ElementAt(0);
            this.log.AddMessage("Migrating site collection \"" + sourceXml.Attributes["Title"] + "\"");
            string url = sourceXml.Attributes["Url"].InnerText;

            Task<string> t = Task.Factory.StartNew(() =>
            {
                try
                {
                    this.ws.DstAdmin.CreateSite(
                        ws.DstUrl,
                        sourceXml.Attributes["Title"].InnerText,
                        sourceXml.Attributes["Description"].InnerText,
                        (int)this.GetLanguage(this.SourceSiteCollection),
                        this.GetSiteTemplate(),
                        ws.DstDomain + "\\" + ws.DstUser,
                        ws.DstUser,
                        string.Empty,
                        string.Empty,
                        string.Empty);
                    return string.Empty;
                }
                catch (Exception e)
                {
                    return "Error migrating site collection \"" + sourceXml.Attributes["Title"] + "\". Message: " + e.Message;
                }
            });

            string response = await t;
            if (response.Equals(string.Empty))
            {
                this.log.AddMessage("Migrating site collection \"" + sourceXml.Attributes["Title"] + "\" finished");
            }
            else
            {
                this.log.AddMessage(response);
            }

            return true;
        }

        /// <summary>
        /// Retrieves the locale of a random list. There is no "regular" way to get the locale with a web service
        /// </summary>
        /// <returns>Sharepoint LCID locale code</returns>
        private async Task<uint> GetLocale()
        {
            Task<uint> locale = Task.Factory.StartNew(() =>
            {
                try
                {
                    XmlNode lc = this.ws.SrcLists.GetListCollection();
                    XmlNode list = this.ws.SrcLists.GetList(lc.ChildNodes[0].Attributes["Name"].InnerText);
                    return Convert.ToUInt32(list["RegionalSettings"]["Locale"].InnerText);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return (uint)1033;
                }
            });

            return await locale;
        }
        
        /// <summary>
        /// Retrieves the site template
        /// </summary>
        private async void GetSiteTemplates()
        {
            // get site templates
            string strDisplay = string.Empty;
            SitesWS.Template[] templates;
            this.ws.SrcSites.GetSiteTemplates((uint)(await this.GetLocale()), out templates);

            foreach (SitesWS.Template template in templates)
            {
                strDisplay = "Title: " + template.Title + "  Name: " + template.Name +
                    "  Description: " + template.Description + "  IsCustom: " +
                    template.IsCustom + "  ID: " + template.ID + "  ImageUrl: " + template.ImageUrl +
                    "  IsHidden: " + template.IsHidden + "  IsUnique: " + template.IsUnique + "\n\n";
            }
        }
    }
}