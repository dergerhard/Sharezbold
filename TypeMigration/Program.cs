using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharezbold.ElementsMigration;
using Microsoft.SharePoint.Client;
using System.Net;

namespace TypeMigration
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("======== START TEST APLLICATION FOR MIGRATION =============");
            NetworkCredential credential = new NetworkCredential("Administrator", "P@ssw0rd", "cssdev");
            ClientContext sourceClientContext = new ClientContext("http://10.10.102.48");
            ClientContext targetClientContext = new ClientContext("http://10.10.102.36");
            sourceClientContext.Credentials = credential;
            targetClientContext.Credentials = credential;

            IElementsMigrator migrator = new Sharepoint2010And2013Migrator(sourceClientContext, targetClientContext);

            // migrator.MigrateContentTypes();
            migrator.MigrateGroup();

            Console.WriteLine("======== FINISHED TEST APLLICATION FOR MIGRATION =============");
            Console.ReadKey();
        }
    }
}
