using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sharezbold.ContentMigration
{
    public class TEST
    {
        public TEST()
        {
            //http://ss13-css-009:23838/testtt/Lists/MyJobApplications/_vti_bin/Views.asmx

            //string url = @"http://sps2013003";
            //string url = @"http://ss13-css-009:23838";
            //string url = @"http://ss13-css-009:23838/testtt/Lists/MyJobApplications";
            //string url = @"http://10.10.7.179";

            string url = @"http://sps2013003";
            string urlViews = url + @"/_vti_bin/Views.asmx";
            string urlWebs = url + @"/_vti_bin/Webs.asmx";
            string urlLists = url + @"/_vti_bin/Lists.asmx";

            var SrcCredentials = new CredentialCache();
            SrcCredentials.Add(new Uri(url), "NTLM", new NetworkCredential("Administrator", "P@ssw0rd", "CSSSPS2013003"));
           
            var dstLists = new Sharezbold.ContentMigration.ListsWS.Lists();
            dstLists.Url = urlLists;
            dstLists.Credentials = SrcCredentials;

            var dstViews = new Sharezbold.ContentMigration.ViewsWS.Views();
            dstViews.Url = urlViews;
            dstViews.Credentials = SrcCredentials;

            Console.WriteLine(dstLists.Url);
            
            /*Get Name attribute values (GUIDs) for list and view. */
            string listName = "Announcements";
            string viewName = "";

            XmlNode abc = dstViews.GetViewCollection(listName);

            XmlNode ndListView = dstLists.GetListAndView(listName, viewName);
            string strListID = ndListView.ChildNodes[0].Attributes["Name"].Value;
            string strViewID = ndListView.ChildNodes[1].Attributes["Name"].Value;

            Console.WriteLine(listName);
            Console.WriteLine(viewName);
            Console.WriteLine(strListID);
            Console.WriteLine(strViewID);
            
            
            /*Create an XmlDocument object and construct a Batch element and its
            attributes. Note that an empty ViewName parameter causes the method to use the default view. */
            XmlDocument doc = new XmlDocument();
            XmlElement batchElement = doc.CreateElement("Batch");
            batchElement.SetAttribute("OnError", "Continue");
            batchElement.SetAttribute("ListVersion", "1");
            batchElement.SetAttribute("ViewName", strViewID);

            /*Specify methods for the batch post using CAML. To update or delete, 
            specify the ID of the item, and to update or add, specify 
            the value to place in the specified column.*/
            batchElement.InnerXml = "<Method ID='1' Cmd='New'>" +
               "<Field Name='ID'>6</Field>" +
               "<Field Name='Title'>Created</Field></Method>";

            /*Update list items. This example uses the list GUID, which is recommended, 
            but the list display name will also work.*/
            try
            {
                dstLists.UpdateListItems(strListID, batchElement);
            }
            catch (Exception ex)
            {
            }

        }
    }
}
