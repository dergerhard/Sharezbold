using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            SPSite site = new SPSite("http://localhost");
            SPWeb web = site.RootWeb;
            string cypher = (string) web.Properties["FHWN EncryptionKey"];

            Console.WriteLine("cypher = {0}", cypher);
            if (cypher != null)
            {
                Console.WriteLine("Please enter your encrypted password: ");
                string password = Console.ReadLine();
                Console.WriteLine("Decrypted: {0}", password.Decrypt(cypher));
            }

            Console.ReadKey();
        }
    }
}
