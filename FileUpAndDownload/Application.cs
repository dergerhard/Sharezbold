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

            // PROXY:
            /*
             * 
             * ClientContext context = new ClientContext("<a valid url>");
context.ExecutingWebRequest += (sen, ags) =>
{
  WebProxy myProxy = new WebProxy();
  myProxy.Address = new Uri("http://<proxy_server_address>");

  myProxy.Credentials = new System.Net.NetworkCredential("jack_reacher","<password>", "<domain>");
  args.WebRequestExecutor.WebRequest.Proxy = myProxy;
};
context.ExecuteQuery();
             */

            Console.WriteLine("Start File-Migration");

            ClientContext source = new ClientContext("http://10.10.102.36");
            ClientContext target = new ClientContext("http://10.10.102.38");
            NetworkCredential credentials = new NetworkCredential("Administrator", "P@ssw0rd", "cssdev");

            source.Credentials = credentials;

            SharePoint2010And2013FileMigrator migrator = new SharePoint2010And2013FileMigrator(source, target);

            try
            {
                migrator.DownloadDocument("images", "sharepoint2010.png");
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
