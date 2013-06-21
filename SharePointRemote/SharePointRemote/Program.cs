using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using System.IO;
// using Microsoft.SharePoint.Workflow;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // define variables:
            FileStream fileStream = null;
            BinaryWriter writer = null;
            try
            {
                string username = "holzgethan";
                string password = "FHWN2013!";
                string domain = "cssdev";

                ClientContext ctx = new ClientContext("http://10.10.102.36");

                ctx.Credentials = new NetworkCredential(username, password, domain);
                Web web = ctx.Web;
                ListCollection listCollection = web.Lists;
                PropertyValues propValues = web.AllProperties;
                UserCollection userCollection = web.SiteUsers;
                ctx.Load(web);
                ctx.Load(listCollection);
                ctx.Load(userCollection);

                ctx.ExecuteQuery();

                Console.WriteLine("the title of the web = {0}", web.Title);
                Console.WriteLine("count of the list-collection = {0}", listCollection.Count);
                Console.WriteLine("count the users on the sharepoint = {0}", userCollection.Count);
                Console.WriteLine("are items available? = {0}", userCollection.AreItemsAvailable);

                
                for (int i = 0; i < userCollection.Count; i++)
                {
                    User user = userCollection.ElementAt(i);

                    Console.WriteLine("user number {0}? = {1}", i, user.LoginName);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                if (writer != null) writer.Close();
                if (fileStream != null) fileStream.Close();
            }

           // ClientContext ctx = new ClientContext("http://10.10.102.36");
            // ClientContext ctx = new ClientContext("http://localhost");
            
            // ListCollection allLists = web.Lists;
            
            // ctx.Load(web, website => website.Title);
            // ctx.Load(web.Webs);
            // ctx.Load(web.Lists);
            // ctx.Load(propValues);

            //ctx.Load(allLists);
            try
            {
             //  ctx.ExecuteQuery();
                /*
               foreach (var list in web.Lists)
               {
                   ctx.Load(list);
                   // ctx.ExecuteQuery();
                   Console.WriteLine("List Name: {0}", list.Title);
                   for (int i = 0; i < list.ItemCount; i++)
                   {
                       Console.WriteLine("item by id: {0}", list.GetItemById(i).ToString());
                   }

                   /*
                   ctx.Load(list.WorkflowAssociations);
                   
                   foreach (var item in list.WorkflowAssociations)
                   {
                       Console.WriteLine("item = " + item.ToString());                       
                   }
                    * *
               }*/
            
            //    Console.WriteLine(web.Title);
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
            Console.WriteLine("END OF TEST");
            Console.ReadLine();
        }
    }
}
