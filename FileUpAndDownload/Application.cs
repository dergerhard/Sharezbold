using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

using Microsoft.SharePoint.Client;

namespace Sharezbold.FileMigration
{
    public class Application
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Start File-Migration");
            ClientContext source = new ClientContext("http://10.10.102.36");
            ClientContext target = new ClientContext("http://10.10.102.48");
            NetworkCredential credentials = new NetworkCredential("Administrator", "P@ssw0rd", "CSSDEV");

            source.Credentials = credentials;
            target.Credentials = credentials;

            Web web = source.Web;
            ListCollection lists = web.Lists;

            source.Load(web);
            source.Load(lists);
            source.ExecuteQuery();
            target.Load(target.Web);
            target.ExecuteQuery();

            Console.WriteLine("Sharepoint 2010 version = {0}", target.ServerVersion.Major);
            Console.WriteLine("Sharepoint 2013 version = {0}", source.ServerVersion.Major);

            /*
            SharePoint2010And2013Migrator migrator = new SharePoint2010And2013Migrator(source, target);

            List<string> guids = new List<string>();
            
            foreach (var list in lists)
            {
                Console.WriteLine("GUID = '{0}'; name of list = '{1}'", list.Id.ToString(), list.Title);
                guids.Add(list.Id.ToString());
            }
            
            migrator.MigrateFiles(guids);
            */
            /* bd293c00-bfa9-4282-b824-f109900ced64
            Console.WriteLine("Start File-Migration");
            ClientContext source = new ClientContext("http://10.10.102.36");
            ClientContext target = new ClientContext("http://10.10.102.36");
            NetworkCredential credentials = new NetworkCredential("Administrator", "P@ssw0rd", "CSSDEV");

            source.Credentials = credentials;
            target.Credentials = credentials;

            SharePoint2010And2013FileMigrator migrator = new SharePoint2010And2013FileMigrator(source, target);

            string documentListName = "Documents";
            string documentName = "sharepoint2010.png";
            string documentType = "Image";
            string documentNewName = "sharepoint2010_new.png";
            Stream inputStream = migrator.DownloadDocument(documentListName, documentName);
            migrator.UploadDocument(documentListName, migrator.RelativeUrl + "new.png", StreamToByte(inputStream));

            inputStream.Close();
             * */
            Console.WriteLine("Finished File-Migartion");
            Console.ReadKey();
        }
    }
}
