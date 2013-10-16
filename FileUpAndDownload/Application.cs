using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Microsoft.SharePoint.Client;

namespace Sharezbold.FileMigration
{
    public class Application
    {
        public static void Main(string[] args)
        {
        //http://blogs.msdn.com/b/sridhara/archive/2010/03/12/uploading-files-using-client-object-model-in-sharepoint-2010.aspx
            // http://www.codeproject.com/Articles/103503/How-to-upload-download-a-document-in-SharePoint-20
            
            Console.WriteLine("Start File-Migration");

            ClientContext source = new ClientContext("http://10.10.102.36");
            ClientContext target = new ClientContext("http://10.10.102.38");
            NetworkCredential credentials = new NetworkCredential("Administrator", "P@ssw0rd", "cssdev");

            source.Credentials = credentials;

            SharePoint2010And2013FileMigrator migrator = new SharePoint2010And2013FileMigrator(source, target);

            try
            {
                // source.Web.Lists;
                //string fileurl = (string)liitem["FileRef"];
                migrator.DownloadDocument("./testBLOGsite/Lists/Photos/sharepoint2010.png");// , "sharepoint2010.png");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            Console.WriteLine("Finished File-Migartion");
            Console.ReadKey();
        }
    }
}
