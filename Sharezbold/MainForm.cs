//-----------------------------------------------------------------------
// <copyright file="MainForm.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Serialization;
    using ContentMigration;
    using Microsoft.SharePoint.Client;
    using Sharezbold.Settings;
    using Sharezbold.ContentMigration.Data;
    using Sharezbold.Logging;
    using FileMigration;
    using Extensions;

    /// <summary>
    /// The main form of the program
    /// </summary>
    public partial class MainForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// represents the migration settings/profile
        /// </summary>
        private MigrationSettings settings;

        /// <summary>
        /// Source context
        /// </summary>
        private ClientContext source;

        /// <summary>
        /// Destination context
        /// </summary>
        private ClientContext destination;

        /// <summary>
        /// used to block the ui, while thread is loading
        /// </summary>
        private PleaseWaitForm waitForm = new PleaseWaitForm();

        /// <summary>
        /// If true, checking, collapsing and expanding tree nodes is not possible
        /// </summary>
        private bool treeViewContentSelectionDisabled = false;

        /// <summary>
        /// Root site collection to transfer
        /// </summary>
        private SSiteCollection sourceSiteCollection;

        /// <summary>
        /// Destination site collection
        /// </summary>
        private SSiteCollection destinationSiteCollection;

        /// <summary>
        /// As the name says.. current (selected) configuration element
        /// </summary>
        private SListViewItem currentConfigurationElement = null;

        /// <summary>
        /// Holds all web services
        /// </summary>
        private WebService webServices;

        /// <summary>
        /// Responsible for loading data from and to the servers with SOAP
        /// </summary>
        private ContentLoader contentLoader;

        /// <summary>
        /// Log data storage
        /// </summary>
        private List<string> logList;

        /// <summary>
        /// Data binding object for logList
        /// </summary>
        //private BindingSource log;

        /// <summary>
        /// the message logger
        /// </summary>
        private Logger log;


        private SharePoint2010And2013Migrator fileMigrator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();

            // this.Size = new Size(this.Size.Width, this.Size.Height - 25); //Todo: use tablessControl
            this.treeViewContentSelection.CheckBoxes = true;
            this.listViewMigrationContent.Scrollable = true;

            ColumnHeader header = new ColumnHeader();
            header.Text = "Element";
            header.Name = "col1";
            header.Width = 400;
            this.listViewMigrationContent.Columns.Add(header);

            // disable all tabs other than the first one
            this.EnableTab(this.tabPageContentSelection, false);
            this.EnableTab(this.tabPageMigrationElements, false);
            this.EnableTab(this.tabPageMigrationPreparation, false);
            this.EnableTab(this.tabPageMigrationProgress, false);

            this.log = new Logger(this.listBoxMigrationLog, @"C:\temp\log.txt");
            this.log.AddMessage("Program started");

        }
                
        /// <summary>
        /// Updates the log
        /// </summary>
        /// <param name="logItem">the message</param>
        /// <param name="onlyLogFile">if true, message is only written to the file</param>
        internal void UpdateProgressLog(string logItem, bool onlyLogFile=false)
        {
            this.log.AddMessage(logItem, onlyLogFile);
        }

        /// <summary>
        /// Exit the application
        /// </summary>
        /// <param name="sender">sender of the action</param>
        /// <param name="e">the event which was executed</param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Start with the migration
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event of the sender</param>
        private async void ButtonStartMigration_Click(object sender, EventArgs e)
        {
            bool readyForMigration = true;
            foreach (ListViewItem lvi in listViewMigrationContent.Items)
            {
                
                if ((lvi is SListViewItem) && (!((SListViewItem)lvi).MigrationObject.ReadyForMigration))
                {
                    readyForMigration = false;
                    break;
                }
            }

            if (readyForMigration)
            {
                this.tabControMain.SelectedTab = this.tabPageMigrationProgress;
                this.EnableTab(this.tabPageMigrationPreparation, false);
                this.EnableTab(this.tabPageMigrationProgress, true);
                this.buttonFinish.Enabled = false;
                bool result = await this.contentLoader.MigrateAllAsync();
                MessageBox.Show("Migration process finished " + (result?"successfully":"with Errors. Please read the log!"), "Info");
                this.buttonFinish.Enabled = true;
            }
            else
            {
                MessageBox.Show("You have to configure all elements that should be migrated before you can start!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// copies settings to the user interface
        /// </summary>
        /// <param name="settings">settings to apply to UI</param>
        private void SettingsToUI(MigrationSettings settings)
        {
            this.settings = settings;
            this.textBoxFromDomain.Text = settings.FromDomain;
            this.textBoxFromHost.Text = settings.FromHost;
            this.textBoxFromUserName.Text = settings.FromUserName;
            this.textBoxFromPassword.Text = settings.FromPassword;

            this.textBoxToDomain.Text = settings.ToDomain;
            this.textBoxToHost.Text = settings.ToHost;
            this.textBoxToHostCA.Text = settings.ToHostCA;
            this.textBoxToUserName.Text = settings.ToUserName;
            this.textBoxToPassword.Text = settings.ToPassword;

            this.checkBoxProxyActivate.Checked = settings.ProxyActive;
            this.textBoxProxyUrl.Text = settings.ProxyUrl;
            this.textBoxProxyUsername.Text = settings.ProxyUsername;
            this.textBoxProxyPassword.Text = settings.ProxyPassword;

            this.checkBoxSiteCollectionMigration.Checked = settings.SiteCollectionMigration;
        }

        /// <summary>
        /// copies settings from the user interface to the MigrationSettings class
        /// </summary>
        /// <returns>settings from UI</returns>
        private MigrationSettings UIToSettings()
        {
            this.settings = new MigrationSettings();
            this.settings.FromDomain = this.textBoxFromDomain.Text;
            this.settings.FromHost = this.textBoxFromHost.Text;
            this.settings.FromUserName = this.textBoxFromUserName.Text;
            this.settings.FromPassword = this.textBoxFromPassword.Text;

            this.settings.ToDomain = this.textBoxToDomain.Text;
            this.settings.ToHost = this.textBoxToHost.Text;
            this.settings.ToHostCA = this.textBoxToHostCA.Text;
            this.settings.ToUserName = this.textBoxToUserName.Text;
            this.settings.ToPassword = this.textBoxToPassword.Text;

            this.settings.ProxyActive = this.checkBoxProxyActivate.Checked;
            this.settings.ProxyUrl = this.textBoxProxyUrl.Text;
            this.settings.ProxyUsername = this.textBoxProxyUsername.Text;
            this.settings.ProxyPassword = this.textBoxProxyPassword.Text;

            this.settings.SiteCollectionMigration = this.checkBoxSiteCollectionMigration.Checked;

            return this.settings;
        }

        /// <summary>
        /// Method for loading migration profiles
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event of the sender</param>
        private void LoadMigrationProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "XML files (*.xml)|*.xml";
            openFileDialog1.Title = "Open migration profile";
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    TextReader reader = new StreamReader(openFileDialog1.FileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(MigrationSettings));
                    MigrationSettings settings = (MigrationSettings)serializer.Deserialize(reader);
                    reader.Close();
                    this.SettingsToUI(settings);
                    this.log.AddMessage("Settings file \"" + openFileDialog1.FileName + "\" loaded");
                }
                catch (XmlException xe)
                {
                    Debug.WriteLine(xe.ToString());
                    this.log.AddMessage("Settings file \"" + openFileDialog1.FileName + "\" could not be loaded due to XML error. Original Message: " + xe.Message);
                    MessageBox.Show("XML reading error. The settings file is corrupted!");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    this.log.AddMessage("Settings file \"" + openFileDialog1.FileName + "\" could not be loaded. Error: " + ex.Message);
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Method for saving migration profiles
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">event of sender</param>
        private void SaveMigrationProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MigrationSettings settings = this.UIToSettings();

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML files (*.xml)|*.xml";
            saveFileDialog1.Title = "Save the current profile";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != string.Empty)
            {
                try
                {
                    TextWriter writer = new StreamWriter(saveFileDialog1.FileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(MigrationSettings));
                    serializer.Serialize(writer, settings);
                    writer.Close();
                    this.log.AddMessage("Settings saved to \"" + saveFileDialog1.FileName + "\"");
                }
                catch (XmlException xe)
                {
                    Debug.WriteLine(xe.ToString());
                    MessageBox.Show("XML writing error. The settings file could not be written correctly!");
                    this.log.AddMessage("Settings could not be saved to \"" + saveFileDialog1.FileName + "\". Error: " + xe.Message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    MessageBox.Show("Error: Could not write file to disk. Original error: " + ex.Message);
                    this.log.AddMessage("Settings could not be saved to \"" + saveFileDialog1.FileName + "\". Error: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Next Button of configuration page clicked
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">event of sender</param>
        private async void ButtonConfigurationNext_Click(object sender, EventArgs e)
        {
            try
            {
                // check if all values are set:
                this.ValidateInputFields();

                this.treeViewContentSelection.Nodes.Clear();
                this.UIToSettings();
                this.waitForm.Show();
                this.EnableTab(this.tabPageConfiguration, false);
                this.EnableTab(this.tabPageContentSelection, true);
                
                // load trees and move on to the next form
                await this.ApplyConfigurationAndLoadSourceTreeAsync();
                this.treeViewContentSelection.Nodes.Add(this.sourceSiteCollection);
                this.waitForm.Hide();
                this.tabControMain.SelectedTab = this.tabPageContentSelection;
            }
            catch (Exception ex)
            {
                // on exception - hide wait form, go back to configuration
                this.waitForm.Hide();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.log.AddMessage(ex.Message);
                this.EnableTab(this.tabPageConfiguration, true);
                this.tabControMain.SelectedTab = this.tabPageConfiguration;
            }
        }

        /// <summary>
        /// Validates input fields of target and source-server-configs.
        /// </summary>
        /// <exception cref="ArgumentNullException">if any input-field is null or empty</exception>
        private void ValidateInputFields()
        {
            if (string.IsNullOrEmpty(this.textBoxFromDomain.Text) || string.IsNullOrEmpty(this.textBoxFromHost.Text) || string.IsNullOrEmpty(this.textBoxFromUserName.Text) || string.IsNullOrEmpty(this.textBoxFromPassword.Text))
            {
                throw new ArgumentNullException("Values for source-server must not be empty!");
            }

            if (string.IsNullOrEmpty(this.textBoxToDomain.Text) || string.IsNullOrEmpty(this.textBoxToHost.Text) || string.IsNullOrEmpty(this.textBoxToUserName.Text) || string.IsNullOrEmpty(this.textBoxToPassword.Text))
            {
                throw new ArgumentNullException("Values for target-server must not be empty!");
            }
        }

        /// <summary>
        /// Tries to connect to the server and loads the migration tree.
        /// </summary>
        /// <returns>Task with bool as return value.</returns>
        private async Task<bool> ApplyConfigurationAndLoadSourceTreeAsync()
        {
            try
            {
                string txt = "Trying to connect to source";
                this.waitForm.SpecialText = txt;
                this.log.AddMessage(txt);
                await this.ConnectToSource();
            }
            catch (Exception ex)
            {
                this.waitForm.SpecialText = string.Empty;
                Debug.WriteLine(ex.ToString());
                throw new LoginFailedException("Could not connect to source SharePoint. Please check your login Data");
            }
            finally
            {
                this.waitForm.SpecialText = string.Empty;
            }

            try
            {
                // as there is no site collection at the destination it makes no sense to connect to it
                if (!this.settings.SiteCollectionMigration)
                {
                    string txt2 = "Trying to connect to destination";
                    this.waitForm.SpecialText = txt2;
                    this.log.AddMessage(txt2);
                    await this.ConnectToDestination();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new LoginFailedException("Could not connect to destination SharePoint. Please check your login Data");
            }
            finally
            {
                this.waitForm.SpecialText = string.Empty;
            }

            string txt3 = "Generating migration tree";
            this.waitForm.SpecialText = txt3;
            this.log.AddMessage(txt3);
            Task<SSiteCollection> t = Task.Factory.StartNew(() =>
            {
                return this.contentLoader.LoadSourceSiteCollection();
            });

            this.sourceSiteCollection = await t;
            
            // make sure, the site collection is checked, if it will be migrated
            if (this.settings.SiteCollectionMigration)
            {
                this.sourceSiteCollection.Checked = true;
                this.sourceSiteCollection.Migrate = true;
                foreach (SSite s in this.sourceSiteCollection.Sites)
                {
                    if (s.IsSiteCollectionSite)
                    {
                        s.Migrate = true;
                        s.Checked = true;
                    }
                }
            }

            return true;
        }
  
        /// <summary>
        /// Loads the tree where you can migrate to.
        /// </summary>
        /// <returns>Task of SpTreeNode as return value.</returns>
        private async Task<SpTreeNode> LoadDestinationTree()
        {
            Task<SpTreeNode> t = Task.Factory.StartNew(() =>
                {
                    ContentDownloader downloader = new ContentDownloader(this.destination);
                    return downloader.GenerateMigrationTree(false);
                });

            return await t;
        }
        
        /// <summary>
        /// Connects to the source, provides context.
        /// </summary>
        /// <returns>Task with flag as return value.</returns>
        private async Task<bool> ConnectToSource()
        {
            this.UIToSettings();

            Task<bool> t = Task.Factory.StartNew(() =>
            {
                Connector connector = new Connector();
                ProxySettings proxySettings = null;
                if (this.checkBoxProxyActivate.Checked)
                {
                    proxySettings = new ProxySettings(this.textBoxProxyUrl.Text.Trim(), this.textBoxProxyUsername.Text.Trim(), this.textBoxProxyPassword.Text.Trim());
                }
                this.source = connector.ConnectToClientContext(this.settings.FromHost, this.settings.FromUserName, this.settings.FromPassword, this.settings.FromDomain, proxySettings);
               
                // TODO: Central Administration HOST
                // set up web services and loader
                this.webServices = new WebService(this.settings.FromHost, this.settings.FromUserName, this.settings.FromDomain, this.settings.FromPassword, this.settings.ToHost, this.settings.ToHostCA, this.settings.ToUserName, this.settings.ToDomain, this.settings.ToPassword);
                this.contentLoader = new ContentLoader(this.webServices, this.log);
                
                return this.webServices.IsSourceLoginPossible && this.source != null;
            });

            return await t;

        }

        /// <summary>
        /// Connects to the destination, provides context.
        /// </summary>
        /// <returns>Task with bool as return value.</returns>
        private async Task<bool> ConnectToDestination()
        {
            this.UIToSettings();

            Task<bool> t = Task.Factory.StartNew(() =>
            {
                Connector connector = new Connector();
                ProxySettings proxySettings = null;
                if (this.checkBoxProxyActivate.Checked)
                {
                    proxySettings = new ProxySettings(this.textBoxProxyUrl.Text.Trim(), this.textBoxProxyUsername.Text.Trim(), this.textBoxProxyPassword.Text.Trim());
                }
                this.destination = connector.ConnectToClientContext(this.settings.ToHost, this.settings.ToUserName, this.settings.ToPassword, this.settings.ToDomain, proxySettings);

                return this.webServices.IsDestinationLoginPossible && this.destination != null;
            });

            return await t;
        }
      
        /// <summary>
        /// Checks all child nodes recursively
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
        private void TreeViewContentSelection_AfterCheck(object sender, TreeViewEventArgs e)
        {
            //// The code only executes if the user caused the checked state to change.
            if (e.Action != TreeViewAction.Unknown)
            {
                if ((this.settings.SiteCollectionMigration && e.Node is SSiteCollection) || ((e.Node is SSite) && ((SSite)e.Node).IsSiteCollectionSite))
                {
                    ((IMigratable)e.Node).Migrate = true;
                    e.Node.Checked = true;
                    MessageBox.Show("You checked \"A Site Collection will be migrated\". As the program is limited to migrate site collections to empty Web Applications, the Site Collection must be migrated!", "Info");
                }
                else if (!(e.Node is SSiteCollection))
                {
                    ((IMigratable)e.Node).Migrate = e.Node.Checked;
                }
                else
                {
                    e.Node.Checked = false;
                    MessageBox.Show("You can't migrate the site collection because there is already a site collection on the destination server present! Migration of site collections is only possible with empty destination web application!", "Error");
                }

                if (e.Node is SSiteCollection && e.Node.Checked && this.settings.SiteCollectionMigration)
                {
                    foreach (TreeNode t in e.Node.Nodes)
                    {
                        if (t is SSite && ((SSite)t).IsSiteCollectionSite)
                        {
                            ((SSite)t).Migrate = e.Node.Checked;
                            t.Checked = e.Node.Checked;
                        }
                    }
                    treeViewContentSelection.Update();
                }
            }
        }

        /// <summary>
        /// This method avoids the events BeforeExpand, BeforeCollapse, BeforeSelect and BeforeCheck to work during migration
        /// </summary>
        /// <param name="sender">the sender of the event</param>
        /// <param name="e">the event arguments</param>
        private void TreeViewContentSelection_DisableOrEnableEvent(object sender, TreeViewCancelEventArgs e)
        {
            //// disable for migration configuration
            if (this.treeViewContentSelectionDisabled)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Loads the items to configure to the listViewMigrationContent
        /// </summary>
        /// <param name="sender">the sender of the event</param>
        /// <param name="e">the EventArgs itself</param>
        private async void ButtonConfigureMigration_Click(object sender, EventArgs e)
        {
            if (this.settings.SiteCollectionMigration)
            {
                MessageBox.Show("As the destination web application is empty, migration will be started now.", "Info");
                this.tabControMain.SelectedTab = this.tabPageMigrationProgress;
                await this.contentLoader.MigrateAllAsync();

            }
            this.EnableTab(this.tabPageContentSelection, false);
            this.EnableTab(this.tabPageMigrationPreparation, true);

            /*
             * 1. Site Collection will be migrated
             *      a. --> nothing to choose, skip to migration
             * 2. Sites and lists will be migrated
             *      a. --> all lists beneath a site will stay there
             *      b. --> all lists where the site is not migrated must have a combobox to choose the destination site (or the newly created ones)
             * 3. Only lists are migrated
             *      a. same as 2.b
             */
            this.waitForm.Show();
            this.waitForm.SpecialText = "loading migration elements";
            this.log.AddMessage("Loading migration elements");

            Task<bool> t = Task<bool>.Factory.StartNew(() =>
            {
                // Generate the ListView with the source elements to configure
                // 1. site collection
                if (this.sourceSiteCollection.Migrate)
                {
                    listViewMigrationContent.Items.Add(new SListViewItem(this.sourceSiteCollection));
                }

                List<SList> listsWithoutSite = new List<SList>();

                // 2. sites
                foreach (SSite site in this.sourceSiteCollection.Sites)
                {
                    if (site.Migrate)
                    {
                        listViewMigrationContent.Items.Add(new SListViewItem(site));
                    }

                    foreach (SList li in site.Lists)
                    {
                        if (li.Migrate && site.Migrate)
                        {
                            listViewMigrationContent.Items.Add(new SListViewItem(li));
                        }
                        if (li.Migrate && !site.Migrate)
                        {
                            listsWithoutSite.Add(li);
                        }
                    }
                }

                if (listsWithoutSite.Count > 0)
                {
                    this.listViewMigrationContent.Items.Add("---Lists without Sites---");
                    foreach (SList li in listsWithoutSite)
                    {
                        this.listViewMigrationContent.Items.Add(new SListViewItem(li));
                    }
                }

                try
                {
                    this.destinationSiteCollection = this.contentLoader.LoadDestinationSiteCollection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                return true;
            });

            await t;
            this.waitForm.Hide();
            this.tabControMain.SelectedTab = this.tabPageMigrationPreparation;

        }

        /// <summary>
        /// Enables or disables a tab page
        /// </summary>
        /// <param name="page">the tab pabe</param>
        /// <param name="enable">enable or disable</param>
        private void EnableTab(TabPage page, bool enable)
        {
            foreach (Control ctl in page.Controls)
            {
                ctl.Enabled = enable;
            }
        }

        /// <summary>
        /// Checks all checkboxes, if Content-Type is checked.
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">the EventArgs itself</param>
        private void CheckBoxMigrateContentTypeCheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxMigrateContentType.Checked)
            {
                this.checkBoxMigrateGroup.Checked = true;
                this.checkBoxMigratePermissionlevels.Checked = true;
                this.checkBoxMigrateSiteColumns.Checked = true;
                this.checkBoxMigrateUser.Checked = true;
                this.checkBoxMigrateWorkflow.Checked = true;
            }
        }

        /// <summary>
        /// Migrates the elements.
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">EventArgs itself</param>
        private async void ButtonElementsMigrationClicked(object sender, EventArgs e)
        {
            try
            {
                this.ValidateInputFields();
            }
            catch (ArgumentNullException ex)
            {
                this.tabPageConfiguration.Show();
                this.tabControMain.SelectedTab = this.tabPageConfiguration;
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (this.source == null || this.destination == null)
            {
                this.tabPageConfiguration.Show();
                this.tabControMain.SelectedTab = this.tabPageConfiguration;
                MessageBox.Show("Please connect to the servers first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            this.tabPageMigrationProgress.Show();
            this.tabControMain.SelectedTab = this.tabPageMigrationProgress;

            ElementsMigrationWorker migrationWorker = new ElementsMigrationWorker(this.source, this.destination, this);
            bool finished = await migrationWorker.StartMigrationAsync(this.checkBoxMigrateContentType.Checked, this.checkBoxMigrateUser.Checked, this.checkBoxMigrateGroup.Checked, this.checkBoxMigrateSiteColumns.Checked, this.checkBoxMigratePermissionlevels.Checked, this.checkBoxMigrateWorkflow.Checked);

            if (finished)
            {
                this.UpdateProgressLog("*************** ELEMENTS MIGRATION FINISHED *******************");
            }
        }

        /// <summary>
        /// Mark possible migrate to elements
        /// </summary>
        /// <param name="node">node to start</param>
        /// <param name="type">type of the nodes to mark</param>
        private void MarkPossibleMigrateToElements(SpTreeNode node, string type)
        {
            if ((node.MigrationObject.DataObject.GetType() == typeof(Microsoft.SharePoint.Client.Web) && type.Equals("Site")) ||
                (node.MigrationObject.DataObject.GetType() == typeof(Microsoft.SharePoint.Client.List) && type.Equals("List")))
            {
                node.BackColor = Color.LightBlue;
            }
            else
            {
                node.BackColor = Color.Transparent;
            }

            foreach (SpTreeNode n in node.Nodes)
            {
                this.MarkPossibleMigrateToElements(n, type);
            }
        }

        /// <summary>
        /// Mark the corresponding migrate to element if set
        /// </summary>
        /// <param name="node">node to start from</param>
        /// <param name="corr">corresponding object</param>
        private void MarkCorrespondingMigrateToElement(SpTreeNode node, object corr)
        {
            if (node.MigrationObject.DataObject == corr)
            {
                node.BackColor = Color.Blue;
            }

            foreach (SpTreeNode n in node.Nodes)
            {
                this.MarkCorrespondingMigrateToElement(n, corr);
            }
        }

        /// <summary>
        /// Method is invoked, when the selected Element is changed
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the event</param>
        private void ListViewMigrationContent_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.listBoxMigrateTo.SelectionMode = SelectionMode.One;

            // reset color
            foreach (ListViewItem lvi in listViewMigrationContent.Items)
            {
                lvi.BackColor = Color.Transparent;
            }

            if (this.listViewMigrationContent.SelectedItems.Count > 0)
            {
                if (listViewMigrationContent.SelectedItems[0] is SListViewItem)
                {
                    this.listBoxMigrateTo.Enabled = true;
                    this.currentConfigurationElement = (SListViewItem)listViewMigrationContent.SelectedItems[0];
                    this.currentConfigurationElement.BackColor = Color.LightBlue;
                    IMigratable mo = this.currentConfigurationElement.MigrationObject;

                    this.listBoxMigrateTo.Items.Clear();

                    if (mo is SSite)
                    {
                        this.labelMigrateTo.Text = "Migrate to site collection:";
                        this.listBoxMigrateTo.Items.Add(this.destinationSiteCollection.Name);
                    }
                    else if (mo is SList)
                    {
                        this.labelMigrateTo.Text = "Migrate to site:";
                        // if the parent object is migrated, the list will be placed under the parent object
                        if (!((IMigratable)(((SList)mo).ParentObject)).Migrate)
                        {
                            foreach (SSite s in this.destinationSiteCollection.Sites)
                            {
                                this.listBoxMigrateTo.Items.Add(this.destinationSiteCollection.Name + ": " + s.Name);
                            }
                            foreach (SSite s in this.sourceSiteCollection.Sites)
                            {
                                if (s.Migrate)
                                {
                                    this.listBoxMigrateTo.Items.Add("From source server: " + s.Name);
                                }
                            }
                        }
                        else
                        {
                            this.listBoxMigrateTo.Items.Add(((IMigratable)(((SList)mo).ParentObject)).Name);
                        }
                    }
                }
                else
                {
                    this.listBoxMigrateTo.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Sets the textboxes for proxy to enabled=true or false.
        /// </summary>
        /// <param name="sender">the sender of the event</param>
        /// <param name="e">the event-arguments</param>
        private void CheckBoxProxyActivate_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxProxyActivate.Checked)
            {
                this.textBoxProxyUrl.Enabled = true;
                this.textBoxProxyUsername.Enabled = true;
                this.textBoxProxyPassword.Enabled = true;
            }
            else
            {
                this.textBoxProxyUrl.Enabled = false;
                this.textBoxProxyUsername.Enabled = false;
                this.textBoxProxyPassword.Enabled = false;
            }
        }

        /// <summary>
        /// Selected index changes
        /// </summary>
        /// <param name="sender">this is the sender</param>
        /// <param name="e">these are the eventargs</param>
        private void ListBoxMigrateTo_SelecedIndexChanged(object sender, EventArgs e)
        {
            if (this.listViewMigrationContent.SelectedItems.Count > 0)
            {
                string fromSourceServer = "From source server: ";
                if (this.currentConfigurationElement.MigrationObject is SList)
                {
                    //find the site:
                    SList list = (SList)this.currentConfigurationElement.MigrationObject;
                    string listBoxText = this.listBoxMigrateTo.SelectedItem.ToString();
                    
                    if (listBoxText.StartsWith(fromSourceServer))
                    {
                        listBoxText = listBoxText.Remove(0, fromSourceServer.Length);
                        foreach (SSite site in this.sourceSiteCollection.Sites)
                        {
                            if (site.Migrate && site.Name.Equals(listBoxText))
                            {
                                list.MigrateTo = site;
                            }
                        }
                    }
                    else
                    {
                        foreach (SSite site in this.destinationSiteCollection.Sites)
                        {
                            if (listBoxText.Equals(((SSiteCollection)site.ParentObject).Name + ": " + site.Name))
                            {
                                list.MigrateTo = site;
                            }
                        }
                    }

                }
                this.currentConfigurationElement.UpdateReadyForMigration();
                this.listViewMigrationContent.Update();
            }
        }

        private void FileMigrationTabClicked(object sender, EventArgs e)
        {
            // TODO validate values
            /*
            this.source = new ClientContext(this.textBoxFromHost.Text);
            this.destination = new ClientContext(this.textBoxToHost.Text);

            this.source.Credentials = new NetworkCredential(this.textBoxFromUserName.Text, this.textBoxFromPassword.Text, this.textBoxFromDomain.Text);
            this.destination.Credentials = new NetworkCredential(this.textBoxToUserName.Text, this.textBoxToPassword.Text, this.textBoxToDomain.Text);
            */
            int bandwith = (int) this.numericUpDownBandwith.Value;
            this.textBoxFileMigrationBandwith.Text = bandwith + "%";
            this.textBoxFileMigrationWebServiceAddress.Text = this.textBoxFileMigrationServiceURI.Text;

            this.fileMigrator = FileMigrationBuilder.GetNewFileMigrationBuilder().WithBandwith(bandwith).WithServiceAddress(new Uri(this.textBoxFileMigrationWebServiceAddress.Text)).WithSourceClientContext(this.source).WithTargetClientContext(destination).CreateMigrator();

            TreeNodeCollection treeNodeCollection = this.treeViewContentSelection.Nodes;
            HashSet<string> webUrls = treeNodeCollection.GetSelectedWebUrls();

            foreach (var item in webUrls)
            {
                
            }
            /*
            foreach (TreeNode item in this.treeViewContentSelection.Nodes)
            {
                IMigratable migratable = (IMigratable)item;
                if (migratable.GetType() == typeof (SSiteCollection))
                {
                    SSiteCollection siteCollection = (SSiteCollection)migratable;
                    foreach (SSite site in siteCollection.Sites)
                    {
                        if (site.Checked)
                        {
                            XmlAttribute attr = site.XmlData.Attributes["Url"];
                            String url = attr.Value;
                           //// Console.WriteLine("selected url = {0}", url);
                        }
                    }
                    
                }
              ////  Console.WriteLine("'{0}' is ready to migrate: {1}; type = {2}", migratable.Name, migratable.ReadyForMigration, migratable.GetType());
            }
            
            Label tempLabel = new Label();*/
        }
    }
}
