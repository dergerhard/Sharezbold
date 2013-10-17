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
            ClientContext target = new ClientContext("http://10.10.102.38");
            NetworkCredential credentials = new NetworkCredential("Administrator", "P@ssw0rd", "CSSDEV");

            source.Credentials = credentials;
            target.Credentials = credentials;

            SharePoint2010And2013FileMigrator migrator = new SharePoint2010And2013FileMigrator(source, target);

            string documentListName = "Documents";
            string documentName = "sharepoint2010.png";
            string documentType = "Image";
            Stream inputStream = migrator.DownloadDocument(documentListName, documentName, documentType);

            string outputFile = @"C:\temp\sharepoint2010.png";
            using (Stream outputStream = System.IO.File.OpenWrite(outputFile))
            {
                byte[] buffer = new byte[8 * 1024];
                int len;
                while ((len = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputStream.Write(buffer, 0, len);
                    outputStream.Flush();
                }
                outputStream.Close();
            }

            inputStream.Close();
            /*
            string destinationFile = "http://10.10.102.36/images/sharepoint_new_2010.png";
            string sourceFile = "http://10.10.102.36/images/sharepoint2010.png";

            SharePointFileMigrator migrator = new SharePointFileMigrator(source, target);
            migrator.CopyDocuments(sourceFile, destinationFile);
            //migrator.Migrate(sourceFile, destinationFile);
            */
            //http://blogs.msdn.com/b/sridhara/archive/2010/03/12/uploading-files-using-client-object-model-in-sharepoint-2010.aspx
            // http://www.codeproject.com/Articles/103503/How-to-upload-download-a-document-in-SharePoint-20
            /*
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
            */
            Console.WriteLine("Finished File-Migartion");
            Console.ReadKey();
        }
    }
}
