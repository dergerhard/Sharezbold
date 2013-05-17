using System;
using System.Collections.Generic;
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
            /*
            TreeViewItem root = new TreeViewItem();
            root.Header = "root";

            TreeViewItem child1 = new TreeViewItem();
            child1.Header = "child1";
            TreeViewItem child2 = new TreeViewItem();
            child2.Header = "child2";

            root.Items.Add(child1);
            root.Items.Add(child2);

            tvSharepointSource.Items.Add(root);
            */

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
                    //insert service name (consider inst.status)
                    TreeViewItem tviInst = new TreeViewItem();
                    tviInst.Header = inst.TypeName;
                    tviServer.Items.Add(tviInst);

                    // If the current service is a SPWebService
                    // (which host's the web applications)
                    if (inst.Service is SPWebService)
                    {
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

            
    }


}
