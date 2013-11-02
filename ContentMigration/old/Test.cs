﻿// <auto-generated/>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using Sharezbold.ContentMigration.ListsWS;
using Sharezbold.ContentMigration.SitesWS;
using System.Xml.Linq;
using System.IO;

namespace Sharezbold.ContentMigration
{
    public class Test
    {
        private const string urlLists = @"/_vti_bin/lists.asmx";
        private const string urlWebs = @"/_vti_bin/webs.asmx";
        private const string urlSites = @"/_vti_bin/sites.asmx";
        private const string urlAdmin = @"/_vti_adm/admin.asmx";

        private string srcUrl;
        private string dstUrl;

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
        private AdminWS.Admin dstAdmin;
        private ListsWS.Lists srcLists;
        private ListsWS.Lists dstLists;

        /// <summary>
        /// sets up the web services
        /// </summary>
        public Test()
        {
            ///////////////////////////////////////////////////////////////////////////
            //set up url
            srcUrl = "http://ss13-css-009:31920/"; //team site
            //srcUrl = "http://ss13-css-009:31920/testsubsite/"; //blog
            string srcUrlLists = srcUrl + urlLists;
            string srcUrlWebs = srcUrl + urlWebs;
            string srcUrlSites = srcUrl + urlSites;
            
            dstUrl = "http://ss13-css-007:5485/";
            string dstUrlLists = dstUrl + urlLists;
            string dstUrlWebs = dstUrl + urlWebs;
            string dstUrlSites = dstUrl + urlSites;

            //central admin is needed for admin.asmx
            string dstUrlCA = "http://ss13-css-007:8080/";
            string dstUrlCaAdmin = dstUrlCA + urlAdmin;

            //set up credentials
            srcUser = "Administrator";
            srcPassword = "P@ssw0rd";
            srcDomain = "cssdev";

            dstUser = "Administrator";
            dstPassword = "P@ssw0rd";
            dstDomain = "cssdev";

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
        }

        /// <summary>
        /// for migration of site collections the central admin-url is needed!
        /// </summary>
        public void migrateSiteCollection()
        {
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


            // 3. Transfer site collections
            //admService.CreateSite("http://Server_Name/sites/SiteCollection_Name",
            //  "Title", "Description", 1033, "STS#0", 
            //  "Domain_Name\\User_Alias","User_Display_Name",
            //  "User_E-mail","","");
            
            /*
             * sites    --> templates for site collection
             */


            //get all webs names
            XmlNode allSrcWebs = srcWebs.GetAllSubWebCollection();
            // result<List>: <Web Title="Fucking site collection" Url="http://ss13-css-009:31920" xmlns="http://schemas.microsoft.com/sharepoint/soap/" />
            Dictionary<string, string> webs = new Dictionary<string,string>();
            foreach (XmlNode web in allSrcWebs)
                webs.Add(web.Attributes["Url"].InnerText, web.Attributes["Title"].InnerText);

            //get webs metadata
            var allSrcWebsXml = new List<XmlNode>();
            foreach (KeyValuePair<string, string> web in webs)
            {
                XmlNode w = srcWebs.GetWeb(web.Key);
                allSrcWebsXml.Add(w);
                // result:
                /*<Web Title="Fucking site collection" 
                 *      Url="http://ss13-css-009:31920"
                 *      Description="" 
                 *      Language="1033" 
                 *      FarmId="{736cfc86-9f17-4379-a356-223f380d0816}" 
                 *      Id="{e021b041-451b-4fc8-828b-cb7f6df1ac21}" 
                 *      ExcludeFromOfflineClient="False" 
                 *      CellStorageWebServiceEnabled="True" 
                 *      AlternateUrls="http://ss13-css-009:31920/" 
                 *      xmlns="http://schemas.microsoft.com/sharepoint/soap/" />
                 *
                 * <Web Title="TestSubSite" 
                 *      Url="http://ss13-css-009:31920/testsubsite" 
                 *      Description="just a blog" 
                 *      Language="1033" 
                 *      FarmId="{736cfc86-9f17-4379-a356-223f380d0816}" 
                 *      Id="{72bc0e61-0852-4a20-9363-1253dea245ba}" 
                 *      ExcludeFromOfflineClient="False" 
                 *      CellStorageWebServiceEnabled="True" 
                 *      AlternateUrls="http://ss13-css-009:31920/" 
                 *      xmlns="http://schemas.microsoft.com/sharepoint/soap/" />
                 */

                //Console.WriteLine(w.OuterXml + "\r\n####################");
            }
            
            //XmlNode siteCollection = allSrcWebs.ChildNodes[0];

            try
            {
                string url = allSrcWebsXml.ElementAt(0).Attributes["Url"].InnerText;

                /*dstAdmin.CreateSite(dstUrl ,
                    "Fucking SC",
                    "description...",
                    1033,
                    "STS#1",
                    dstDomain + "\\" + dstUser,  //vllt: "SS13-CSS-007\\administrator",
                    dstUser,
                    "",
                    "", //@"http://ss13-css-007:5485/fsc",
                    ""); //"SP2013Gerhard");*/

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
            }
            
            //get site templates
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
            }
            

        }

        /// <summary>
        /// Retreives the site template from the HTML site. Web services offer no possibility to do this.
        /// </summary>
        /// <returns>the site template name</returns>
        public string getSiteTemplate()
        {
            // get HTML site
            WebRequest request = WebRequest.Create(srcUrl);
            request.Credentials = srcCredentials;
            request.PreAuthenticate = true;
            WebResponse response =request.GetResponse();
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
        /// Retreives the locale of a random list. There is no "regular" way to get the locale with a web service
        /// </summary>
        /// <returns>Sharepoint LCID locale code</returns>
        private int getLocale()
        {
            XmlNode lc = srcLists.GetListCollection();
            Console.WriteLine(lc.ChildNodes[0].Attributes["Name"].InnerText);
            XmlNode list = srcLists.GetList(lc.ChildNodes[0].Attributes["Name"].InnerText);
            return Convert.ToInt32(list["RegionalSettings"]["Locale"].InnerText);
        }
        

        /// <summary>
        /// Migrates all new site columns from src to dst. Columns which changed are ignored, as "system columns" can't
        /// be  recognised yet.
        /// </summary>
        public void MigrateSiteColumns()
        {
            ///////////////////////////////////////////////////////////////////////////
            //set up url
            
            string srcUrl = @"http://ss13-css-009:31920/";
            string srcUrlLists = srcUrl + urlLists;
            string srcUrlWebs = srcUrl + urlWebs;
           
            string dstUrl = @"http://ss13-css-007:5485/";
            string dstUrlLists = dstUrl + urlLists;
            string dstUrlWebs = dstUrl + urlWebs;

            //set up credentials
            CredentialCache srcCredentials = new CredentialCache();
            srcCredentials.Add(new Uri(srcUrl), "NTLM", new NetworkCredential("Administrator", "P@ssw0rd", "cssdev"));
            
            CredentialCache dstCredentials = new CredentialCache();
            dstCredentials.Add(new Uri(dstUrl), "NTLM", new NetworkCredential("Administrator", "P@ssw0rd", "cssdev"));
            
            
            // WORKS #################################################################
            /*
            //create web service object
            ListsWS.Lists ls = new ListsWS.Lists();

            //apply url
            ls.Url = srcUrlLists;

            //apply credentials
            ls.Credentials = srcCredentials;
            
            XmlNode node = ls.GetList("My Applications");
            Console.WriteLine(this.XmlToString(node, 4));
            */
            ///////////////////////////////////////////////////////////////////////////

            // src ---------------------------------------------
            WebsWS.Webs srcWebs = new WebsWS.Webs();
            srcWebs.Url = srcUrlWebs;
            srcWebs.Credentials = srcCredentials;
            XmlNode srcColumsXml = srcWebs.GetColumns();
            
            // dst ---------------------------------------------
            WebsWS.Webs dstWebs = new WebsWS.Webs();
            dstWebs.Url = dstUrlWebs;
            dstWebs.Credentials = dstCredentials;
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
            
            Console.WriteLine(this.XmlToString(returnValue, 4));

            //Console.WriteLine("{0} elements to update\r\n{1} elements to create", updateColumns.Count, createColumns.Count);
        }

        public string XmlToString(System.Xml.XmlNode node, int indentation)
        {
            using (var sw = new System.IO.StringWriter())
            {
                using (var xw = new System.Xml.XmlTextWriter(sw))
                {
                    xw.Formatting = System.Xml.Formatting.Indented;
                    xw.Indentation = indentation;
                    node.WriteContentTo(xw);
                }
                return sw.ToString();
            }
        }
    }
}