using System;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Client;
    

namespace UITest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CollapseExpandTreeviewItems(TreeViewItem Item, bool expand)
            {
                Item.IsExpanded = expand;

                foreach (TreeViewItem item in Item.Items)
                {
                    item.IsExpanded = expand;

                    if (item.HasItems)
                        CollapseExpandTreeviewItems(item, expand);
                }
            
            }

        private void btDoLoad_Click(object sender, RoutedEventArgs e)
        {

            // SPFarm.Local represents the highest possible node in the hierarchy.
            SPFarm farm = SPFarm.Local;
            TreeViewItem tviRoot = new TreeViewItem();
            tviRoot.Header = farm.Name;

            // Iterate through each server in the farm.
            foreach (SPServer server in farm.Servers)
            {
                TreeViewItem tviServer = new TreeViewItem();
                tviServer.Header = server.Name;
                tviRoot.Items.Add(tviServer);

                // Iterate through each service of all services on the server.
                foreach (SPServiceInstance inst in server.ServiceInstances)
                {
                    // If the current service is a SPWebService
                    // (which host's the web applications)
                    if ((inst.Service is SPWebService))// && (! inst.TypeName.ToLower().Contains("administrat")))
                    {
                        //insert service name (consider inst.status)
                        TreeViewItem tviInst = new TreeViewItem();
                        tviInst.Header = inst.TypeName;
                        tviServer.Items.Add(tviInst);

                        // Get all web applications.
                        SPWebApplicationCollection wepApps = (inst.Service as SPWebService).WebApplications;

                        // Iterate trough each web application.
                        foreach (SPWebApplication app in wepApps)
                        {
                            TreeViewItem tviWebApp = new TreeViewItem();
                            tviWebApp.Header = "Site Collection: " + app.DisplayName + ", Sitecollection count: " + app.Sites.Count;
                            tviInst.Items.Add(tviWebApp);

                            // Print the name of the application and the number of SiteCollections.
                            //Console.WriteLine("\t{0} (Site Collections: {1})", app.DisplayName, app.Sites.Count);
                            
                            // Iterate through each site.
                            foreach (SPSite site in app.Sites)
                            {
                                // Print the url and the number of webs in this SPSite.
                                //Console.WriteLine("\t\tPrimaryUri: {0}", site.PrimaryUri);
                                //Console.WriteLine("\t\tWebCount: {0}", site.AllWebs.Count);

                                TreeViewItem tviSite = new TreeViewItem();
                                tviSite.Header = site.PrimaryUri;// +", Site count: " + site.AllWebs.Count;
                                tviWebApp.Items.Add(tviSite);

                                
                                // Iterate trough each web.
                                foreach (SPWeb web in site.AllWebs)
                                {
                                    TreeViewItem tviWeb = new TreeViewItem();
                                    tviWeb.Header = web.Title;
                                    tviSite.Items.Add(tviWeb);

                                    // Print some information about the web.
                                    //Console.WriteLine("\t\t\tIsRootWeb: {0}", web.IsRootWeb);
                                    //Console.WriteLine("\t\t\tTitle: {0}", web.Title);
                                    //Console.WriteLine("\t\t\tListCount: {0}", web.Lists.Count);

                                    // Iterate trough each list in the web.
                                    foreach (SPList list in web.Lists)
                                    {
                                        TreeViewItem tviList = new TreeViewItem();
                                        tviList.Header = list.Title;
                                        tviWeb.Items.Add(tviList);

                                        // Print the title of the list.
                                        Console.WriteLine("\t\t\t\tList Name:{0}", list.Title);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            tvSharepointSource.Items.Add(tviRoot);

            foreach (TreeViewItem item in tvSharepointSource.Items)
                CollapseExpandTreeviewItems(item, true);
        }

        private void btDoLoadWithClientObjectModel_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("i am here");
            // define variables:
            //FileStream fileStream = null;
            //BinaryWriter writer = null;
            try
            {
                string username = "liebmann";
                string password = "FHWN2013!";
                string domain = "cssdev";

                ClientContext ctx = new ClientContext("http://ss13-css-009:8080/");
                
                ctx.Credentials = new NetworkCredential(username, password, domain);
                Debug.WriteLine("1");
                Web web = ctx.Web;
                ctx.Load(web);
                //ctx.Load(listCollection);
                //ctx.Load(userCollection);
                Debug.WriteLine("2");
                ctx.ExecuteQuery();
                Debug.WriteLine("3");

                ListCollection listCollection = web.Lists;
                PropertyValues propValues = web.AllProperties;
                UserCollection userCollection = web.SiteUsers;
                
                
                System.Diagnostics.Debug.WriteLine("the title of the web = {0}", web.Title);
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
                //if (writer != null) writer.Close();
                //if (fileStream != null) fileStream.Close();
            }
        }

            
    }


}


/*
      public static void Main(string[] args)
        {
            // Set console size to maximum - 2 rows/columns.
            Console.WindowWidth = Console.LargestWindowWidth - 2;
            Console.WindowHeight = Console.LargestWindowHeight - 2;
            Console.BufferHeight = 999;

            // SPFarm.Local represents the highest possible node in the hierarchy.
            SPFarm farm = SPFarm.Local;

            // Iterate through each server in the farm.
            foreach (SPServer server in farm.Servers)
            {
                // Iterate through each service of all services on the server.
                foreach (SPServiceInstance inst in server.ServiceInstances)
                {
                    // Print the service name and the service status.
                    Console.WriteLine("{0} ({1})", inst.TypeName, inst.Status);

                    // If the current service is a SPWebService
                    // (which host's the web applications)
                    if (inst.Service is SPWebService)
                    {
                        // Get all web applications.
                        SPWebApplicationCollection wepApps = (inst.Service as SPWebService).WebApplications;

                        // Iterate trough each web application.
                        foreach (SPWebApplication app in wepApps)
                        {
                            // Print the name of the application and the number of SiteCollections.
                            Console.WriteLine("\t{0} (Site Collections: {1})", app.DisplayName, app.Sites.Count);

                            // Iterate through each site.
                            foreach (SPSite site in app.Sites)
                            {
                                // Print the url and the number of webs in this SPSite.
                                Console.WriteLine("\t\tPrimaryUri: {0}", site.PrimaryUri);
                                Console.WriteLine("\t\tWebCount: {0}", site.AllWebs.Count);

                                // Iterate trough each web.
                                foreach (SPWeb web in site.AllWebs)
                                {
                                    // Print some information about the web.
                                    Console.WriteLine("\t\t\tIsRootWeb: {0}", web.IsRootWeb);
                                    Console.WriteLine("\t\t\tTitle: {0}", web.Title);
                                    Console.WriteLine("\t\t\tListCount: {0}", web.Lists.Count);

                                    // Iterate trough each list in the web.
                                    foreach (SPList list in web.Lists)
                                    {
                                        // Print the title of the list.
                                        Console.WriteLine("\t\t\t\tList Name:{0}", list.Title);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Print "press key" info.
            Console.WriteLine("\nPress any key to exit the program");

            // Wait until someone hit's the keyboard.
            Console.ReadKey();
        }
    }*/