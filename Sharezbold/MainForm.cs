﻿//-----------------------------------------------------------------------
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

    /// <summary>
    /// Delegate for loading the source tree
    /// </summary>
    ////public delegate void ApplyConfigurationAndLoadSourceTreeDelegate();

    /// <summary>
    /// Delegate for updating main ui when ApplyConfigurationAndLoadSourceTreeDelegate is finished
    /// </summary>
    /// <param name="node">the resulting source node</param>
    ////public delegate void ApplyConfigurationAndLoadSourceTreeFinishedDelegate(SpTreeNode node);

    /// <summary>
    /// Delegate for loading destination tree
    /// </summary>
    ////public delegate void LoadDestinationTreeDelegate();

    /// <summary>
    /// Delegate for updating main ui when LoadDestinationTreeDelegate is finished
    /// </summary>
    /// <param name="node">the root node</param>
    ////public delegate void LoadDestinationTreeFinishedDelegate(SpTreeNode node);

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
        /// Root node of the source migration tree
        /// </summary>
        private SpTreeNode sourceTreeRoot; //eliminate

        /// <summary>
        /// Root site collection to transfer
        /// </summary>
        private SSiteCollection sourceSiteCollection;

        /// <summary>
        /// Root node of the destination migration tree
        /// </summary>
        private SpTreeNode destinationTreeRoot;

        /// <summary>
        /// As the name says.. current (selected) configuration element
        /// </summary>
        private SpListViewItem currentConfigurationElement = null;

        /// <summary>
        /// Holds all web services
        /// </summary>
        private WebService webServices;

        /// <summary>
        /// Responsible for loading data from and to the servers with SOAP
        /// </summary>
        private ContentLoader contentLoader;

        /// <summary>
        /// Defines whether site colecction migration is possible
        /// </summary>
        private bool isSiteCollectionMigrationPossible = false;

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
        }

        /// <summary>
        /// Updates the progresslog.
        /// </summary>
        /// <param name="logItem">item to log</param>
        internal void UpdateProgressLog(string logItem)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(this.UpdateProgressLog), logItem);
            }
            else
            {
                this.listBoxMigrationLog.Items.Add(logItem);
                this.listBoxMigrationLog.Update();
            }
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
        private void ButtonStartMigration_Click(object sender, EventArgs e)
        {
            bool readyForMigration = true;
            foreach (ListViewItem lvi in listViewMigrationContent.Items)
            {
                if (!((SpListViewItem)lvi).MigrationObject.ReadyForMigration)
                {
                    readyForMigration = false;
                    break;
                }
            }

            if (readyForMigration)
            {
                this.tabControMain.SelectedTab = this.tabPageMigrationProgress;
                this.EnableTab(this.tabPageMigrationPreparation, false);
                //// todo
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
            this.textBoxToUserName.Text = settings.ToUserName;
            this.textBoxToPassword.Text = settings.ToPassword;
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
            this.settings.ToUserName = this.textBoxToUserName.Text;
            this.settings.ToPassword = this.textBoxToPassword.Text;
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
                }
                catch (XmlException xe)
                {
                    Debug.WriteLine(xe.ToString());
                    MessageBox.Show("XML reading error. The settings file is corrupted!");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
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
                }
                catch (XmlException xe)
                {
                    Debug.WriteLine(xe.ToString());
                    MessageBox.Show("XML writing error. The settings file could not be written correctly!");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    MessageBox.Show("Error: Could not write file to disk. Original error: " + ex.Message);
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
                //this.treeViewContentSelection.Nodes.Add(this.sourceTreeRoot);
                this.treeViewContentSelection.Nodes.Add(this.sourceSiteCollection);
                this.waitForm.Hide();
                this.tabControMain.SelectedTab = this.tabPageContentSelection;
            }
            catch (Exception ex)
            {
                // on exception - hide wait form, go back to configuration
                this.waitForm.Hide();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // TODO: Central Administration HOST
            this.webServices = new WebService(this.settings.FromHost, this.settings.FromUserName, this.settings.FromDomain, this.settings.FromPassword, this.settings.ToHost, @"http://ss13-css-007:8080/", this.settings.ToUserName, this.settings.ToDomain, this.settings.ToPassword);
            this.contentLoader = new ContentLoader(this.webServices);
                
            try
            {
                this.waitForm.SpecialText = "Trying to connect to source";
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
                this.waitForm.SpecialText = "Trying to connect to destination";
                await this.ConnectToDestination();
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

            this.waitForm.SpecialText = "Generating migration tree";
            Task<SSiteCollection> t = Task.Factory.StartNew(() =>
            {
                //ContentDownloader cm = new ContentDownloader(this.source);
                //return cm.GenerateMigrationTree();
                //WebService ws = new WebService(@"http://ss13-css-009:31920/", "Administrator", "cssdev", "P@ssw0rd", @"http://ss13-css-007:5485/", @"http://ss13-css-007:8080/", "Administrator", "cssdev", "P@ssw0rd");
                //ContentLoader loader = new ContentLoader(ws);
                this.isSiteCollectionMigrationPossible = contentLoader.IsSiteCollectionMigrationPossible;
                return this.contentLoader.LoadSourceData();
                
            });

            //this.sourceTreeRoot = await t;
            this.sourceSiteCollection = await t;

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
            this.source = new ClientContext(this.settings.FromHost);
            this.SetProxy(this.source);
            var cc = new CredentialCache();
            cc.Add(new Uri(this.source.Url), "NTLM", new NetworkCredential(this.settings.FromUserName, this.settings.FromPassword, this.settings.FromDomain));
            this.source.Credentials = cc;
            
            Task<bool> t = Task.Factory.StartNew(() =>
            {
                this.source.ExecuteQuery();
                return true;
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
            this.destination = new ClientContext(this.settings.ToHost);
            this.SetProxy(this.destination);
            var cc = new CredentialCache();
            cc.Add(new Uri(this.destination.Url), "NTLM", new NetworkCredential(this.settings.ToUserName, this.settings.ToPassword, this.settings.ToDomain));
            this.destination.Credentials = cc;

            Task<bool> t = Task.Factory.StartNew(() =>
            {
                this.destination.ExecuteQuery();
                return true;
            });

            return await t;
        }

        /// <summary>
        /// Sets the proxy for the connection to the server.
        /// </summary>
        /// <param name="clientContext">the clientcontext of the server</param>
        private void SetProxy(ClientContext clientContext)
        {
            if (this.checkBoxProxyActivate.Checked)
            {
                clientContext.ExecutingWebRequest += (sen, args) =>
                {
                    System.Net.WebProxy myProxy = new System.Net.WebProxy();
                    myProxy.Address = new Uri(this.textBoxProxyUrl.Text.Trim());

                    myProxy.Credentials = new System.Net.NetworkCredential(this.textBoxProxyUsername.Text.Trim(), this.textBoxProxyPassword.Text.Trim());
                    args.WebRequestExecutor.WebRequest.Proxy = myProxy;
                };
            }
        }

        /// <summary>
        /// Checks all child nodes recursively
        /// </summary>
        /// <param name="treeNode">the root to start checking</param>
        /// <param name="nodeChecked">the check state (true if it should be checked)</param>
        /*private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                ((SpTreeNode)node).MigrationObject.Skip = !nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    this.CheckAllChildNodes(node, nodeChecked);
                }
            }
        }*/

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
                if ((this.isSiteCollectionMigrationPossible && e.Node is SSiteCollection) || !(e.Node is SSiteCollection))
                    ((IMigratable)e.Node).Migrate = e.Node.Checked;
                else
                {
                    e.Node.Checked = false;
                    MessageBox.Show("You can't migrate the site collection because there is already a site collection on the destination server present! Migration of site collections is only possible with empty destination web application!", "Error");
                }

                if (e.Node is SSiteCollection && e.Node.Checked && this.isSiteCollectionMigrationPossible)
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
            /*
            try
            {
                // load "migrate to" elements
                this.treeViewMigrateTo.Nodes.Clear();
                this.waitForm.Show();
                this.EnableTab(this.tabPageContentSelection, false);

                // async starting of destination tree loading
                this.waitForm.SpecialText = "Loading destination tree";
                SpTreeNode node = await this.LoadDestinationTree();

                this.destinationTreeRoot = node;
                this.treeViewMigrateTo.Nodes.Add(this.destinationTreeRoot);
                this.treeViewMigrateTo.ExpandAll();
                this.waitForm.Hide();
                this.tabControMain.SelectedTab = this.tabPageMigrationPreparation;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
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
            // reset color
            foreach (ListViewItem lvi in listViewMigrationContent.Items)
            {
                lvi.BackColor = Color.Transparent;
            }

            if (this.listViewMigrationContent.SelectedItems.Count > 0)
            {
                // set labels and mark elements
                this.currentConfigurationElement = (SpListViewItem)listViewMigrationContent.SelectedItems[0];
                this.currentConfigurationElement.BackColor = Color.LightBlue;
                MigrationObject mo = this.currentConfigurationElement.MigrationObject;

                if (mo.DataObject.GetType() == typeof(Web))
                {
                    this.labelElementType.Text = "Site";
                    this.labelLegalType.Text = "Site Collection";
                    this.destinationTreeRoot.BackColor = Color.LightBlue;
                }
                else if (mo.DataObject.GetType() == typeof(List))
                {
                    this.labelElementType.Text = "List";
                    this.labelLegalType.Text = "Site";
                    this.MarkPossibleMigrateToElements(this.destinationTreeRoot, "Site");
                }
                else
                {
                    this.labelElementType.Text = "List Item";
                    this.labelLegalType.Text = "List";
                    this.MarkPossibleMigrateToElements(this.destinationTreeRoot, "List");
                }

                this.labelElementName.Text = mo.Identifier;

                // mark corresponding element if set
                if (mo.ReadyForMigration)
                {
                    this.MarkCorrespondingMigrateToElement(this.destinationTreeRoot, mo.DestinationObject);
                }
            }
        }

        /// <summary>
        /// Update the elements to migrate, if possible
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the event</param>
        private void TreeViewMigrateTo_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SpTreeNode selectedNode = (SpTreeNode)this.treeViewMigrateTo.SelectedNode;

            if ((selectedNode.MigrationObject.DataObject.GetType() == typeof(Web) && this.labelLegalType.Text == "Site") ||
                (selectedNode.MigrationObject.DataObject.GetType() == typeof(List) && this.labelLegalType.Text == "List"))
            {
                this.currentConfigurationElement.MigrationObject.DestinationObject = selectedNode.MigrationObject.DataObject;
            }
            else
            {
                this.currentConfigurationElement.MigrationObject.DestinationObject = null;
            }

            this.currentConfigurationElement.UpdateReadyForMigration();
            this.listViewMigrationContent.Update();
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
    }
}
