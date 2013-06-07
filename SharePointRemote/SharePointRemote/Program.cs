using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientContext ctx = new ClientContext("http://10.10.102.36");
            
            //ListCollection allLists = web.Lists;
            string username = "holzgethan";
            string password = "FHWN2013!";
            string domain = "cssdev";
            ctx.Credentials = new NetworkCredential(username, password, domain);
            Web web = ctx.Web;
            PropertyValues propValues = web.AllProperties;
            ctx.Load(web, website => website.Title);
            ctx.Load(web.Webs);
            ctx.Load(propValues);

            //ctx.Load(allLists);
            try
            {
               ctx.ExecuteQuery();

                Console.WriteLine(web.Title);
                /*
                string cypher = (string)propValues["fhwn encryptionkey"];
                Console.WriteLine("cypher = {0}", cypher);
                if (cypher != null)
                {
                    Console.WriteLine("Please enter your encrypted password: ");
                    string pwd = Console.ReadLine();
                    // Console.WriteLine("Decrypted: {0}", pwd.Decrypt(cypher));

                }
                 */
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            

            //foreach (List list in allLists)
            //{
            //    Console.WriteLine("Found list with name '{0}'", list.Title);

            //    if (list.Title == "FHWN UserList")
            //    {
            //        string cypher = (string) propValues["fhwn encryptionkey"];
            //    }
            //}

            Console.ReadLine();
        }
    }
}
