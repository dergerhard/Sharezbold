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

    /// <summary>
    /// Delegate for loading the source tree
    /// </summary>
    public delegate void ApplyConfigurationAndLoadSourceTreeDelegate();

    /// <summary>
    /// Delegate for updating main ui when ApplyConfigurationAndLoadSourceTreeDelegate is finished
    /// </summary>
    /// <param name="node">the resulting source node</param>
    public delegate void ApplyConfigurationAndLoadSourceTreeFinishedDelegate(SpTreeNode node);

    /// <summary>
    /// Delegate for loading destination tree
    /// </summary>
    public delegate void LoadDestinationTreeDelegate();

    /// <summary>
    /// Delegate for updating main ui when LoadDestinationTreeDelegate is finished
    /// </summary>
    /// <param name="node">the root node</param>
    public delegate void LoadDestinationTreeFinishedDelegate(SpTreeNode node);

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
        private SpTreeNode sourceTreeRoot;

        /// <summary>
        /// Root node of the destination migration tree
        /// </summary>
        private SpTreeNode destinationTreeRoot;

        /// <summary>
        /// As the name says.. current (selected) configuration element
        /// </summary>
        private SpListViewItem currentConfigurationElement = null;

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
                // todo
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
        private void ButtonConfigurationNext_Click(object sender, EventArgs e)
        {
            try
            {
                // check if all values are set:
                this.ValidateInputFields();

                this.treeViewContentSelection.Nodes.Clear();
                this.UIToSettings();
                this.waitForm.Show();
                this.EnableTab(this.tabPageConfiguration, false);

                // needed for async loading
                ApplyConfigurationAndLoadSourceTreeDelegate w = this.ApplyConfigurationAndLoadSourceTree;
                w.BeginInvoke(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// Tries to connect to the server and loads the migration tree
        /// </summary>
        private void ApplyConfigurationAndLoadSourceTree()
        {
            try
            {
                this.ConnectToSource();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new LoginFailedException("Could not connect to source SharePoint. Please check your login Data");
            }

            try
            {
                this.ConnectToDestination();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new LoginFailedException("Could not connect to destination SharePoint. Please check your login Data");
            }

            ContentDownloader cm = new ContentDownloader(this.source);
            SpTreeNode node = cm.GenerateMigrationTree();

            // need to use invoke to be thread safe
            this.Invoke(new ApplyConfigurationAndLoadSourceTreeFinishedDelegate(this.ApplyConfigurationAndLoadSourceTreeFinished), new object[] { node });
        }

        /// <summary>
        /// This method is needed for threading and called when ApplyConfigurationAndLoadSourceTreeFinished is finished
        /// </summary>
        /// <param name="node">is the loaded root node - will be applied to treeviewContentSelection</param>
        private void ApplyConfigurationAndLoadSourceTreeFinished(SpTreeNode node)
        {
            this.sourceTreeRoot = node;
            this.treeViewContentSelection.Nodes.Add(this.sourceTreeRoot);
            this.waitForm.Hide();
            this.tabControMain.SelectedTab = this.tabPageContentSelection;
        }

        /// <summary>
        /// loads the tree where you can migrate to
        /// </summary>
        private void LoadDestinationTree()
        {
            ContentDownloader downloader = new ContentDownloader(this.destination);
            SpTreeNode node = downloader.GenerateMigrationTree(false);

            // is needed for async loading
            this.Invoke(new LoadDestinationTreeFinishedDelegate(this.LoadDestinationTreeFinished), new object[] { node });
        }

        /// <summary>
        /// Is called when LoadDestinationTree finised. Applys root node to treeview and selects next tab
        /// </summary>
        /// <param name="node">the node</param>
        private void LoadDestinationTreeFinished(SpTreeNode node)
        {
            this.destinationTreeRoot = node;
            this.treeViewMigrateTo.Nodes.Add(this.destinationTreeRoot);
            this.treeViewMigrateTo.ExpandAll();
            this.waitForm.Hide();
            this.tabControMain.SelectedTab = this.tabPageMigrationPreparation;
        }

        /// <summary>
        /// Connects to the source, provides context
        /// </summary>
        private void ConnectToSource()
        {
            this.UIToSettings();
            this.source = new ClientContext(this.settings.FromHost);
            var cc = new CredentialCache();
            cc.Add(new Uri(this.source.Url), "NTLM", new NetworkCredential(this.settings.FromUserName, this.settings.FromPassword, this.settings.FromDomain));
            this.source.Credentials = cc;
            this.source.ExecuteQuery();
        }

        /// <summary>
        /// Connects to the destination, provides context
        /// </summary>
        private void ConnectToDestination()
        {
            this.UIToSettings();
            this.destination = new ClientContext(this.settings.ToHost);
            var cc = new CredentialCache();
            cc.Add(new Uri(this.destination.Url), "NTLM", new NetworkCredential(this.settings.ToUserName, this.settings.ToPassword, this.settings.ToDomain));
            this.destination.Credentials = cc;
            this.destination.ExecuteQuery();
        }

        /// <summary>
        /// Checks all child nodes recursively
        /// </summary>
        /// <param name="treeNode">the root to start checking</param>
        /// <param name="nodeChecked">the check state (true if it should be checked)</param>
        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
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
                ((SpTreeNode)e.Node).MigrationObject.Skip = !e.Node.Checked;
                if (e.Node.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(e.Node, e.Node.Checked);
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
        private void ButtonConfigureMigration_Click(object sender, EventArgs e)
        {
            // Site collections not supported
            /*if (this.sourceTreeRoot.Checked)
            {
                listViewMigrationContent.Items.Add(new SpListViewItem(this.sourceTreeRoot.MigrationObject));
            }*/

            // Generate the ListView with the source elements to configure
            foreach (TreeNode web in this.sourceTreeRoot.Nodes)
            {
                if (web.Checked)
                {
                    listViewMigrationContent.Items.Add(new SpListViewItem(((SpTreeNode)web).MigrationObject));
                }

                foreach (TreeNode li in web.Nodes)
                {
                    if (li.Checked)
                    {
                        listViewMigrationContent.Items.Add(new SpListViewItem(((SpTreeNode)li).MigrationObject));
                    }

                    foreach (TreeNode lii in li.Nodes)
                    {
                        if (lii.Checked)
                        {
                            listViewMigrationContent.Items.Add(new SpListViewItem(((SpTreeNode)lii).MigrationObject));
                        }
                    }
                }
            }

            try
            {
                // load "migrate to" elements
                this.treeViewMigrateTo.Nodes.Clear();
                this.waitForm.Show();
                this.EnableTab(this.tabPageContentSelection, false);

                // LoadDestinationTree is started as thread
                LoadDestinationTreeDelegate del = this.LoadDestinationTree;
                del.BeginInvoke(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                this.tabControMain.SelectedTab = tabPageConfiguration;
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (this.destination == null)
            {
                this.ConnectToDestination();
            }

            this.tabPageMigrationProgress.Show();
            this.tabControMain.SelectedTab = tabPageMigrationProgress;

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

            if (listViewMigrationContent.SelectedItems.Count > 0)
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
            SpTreeNode selectedNode = (SpTreeNode)treeViewMigrateTo.SelectedNode;

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
    }
}
