using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientContext ctx = new ClientContext("http://localhost");
            Web web = ctx.Web;
            PropertyValues propValues = web.AllProperties;
            //ListCollection allLists = web.Lists;

            ctx.Load(web);
            ctx.Load(propValues);
            //ctx.Load(allLists);
            ctx.ExecuteQuery();

            string cypher = (string)propValues["fhwn encryptionkey"];
            Console.WriteLine("cypher = {0}", cypher);
            if (cypher != null)
            {
                Console.WriteLine("Please enter your encrypted password: ");
                string password = Console.ReadLine();
                Console.WriteLine("Decrypted: {0}", password.Decrypt(cypher));

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
