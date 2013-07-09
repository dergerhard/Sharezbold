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
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();

            // this.Size = new Size(this.Size.Width, this.Size.Height - 25); //Todo: use tablessControl
            this.treeViewContentSelection.CheckBoxes = true;

            // TODO: delete this
            /*TextReader reader = new StreamReader(@"C:\temp\test.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(MigrationSettings));
            MigrationSettings settings = (MigrationSettings)serializer.Deserialize(reader);
            reader.Close();
            this.SettingsToUI(settings);*/
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
            //this.tabControl1.SelectedTab = this.tabPageMigrationProgress;
            treeViewContentSelection.ExpandAll();
            treeViewContentSelectionDisabled = true;

            treeViewContentSelection.SelectedNode = treeViewContentSelection.Nodes[0];


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
            this.settings.FromDomain = textBoxFromDomain.Text;
            this.settings.FromHost = textBoxFromHost.Text;
            this.settings.FromUserName = textBoxFromUserName.Text;
            this.settings.FromPassword = textBoxFromPassword.Text;

            this.settings.ToDomain = textBoxToDomain.Text;
            this.settings.ToHost = textBoxToHost.Text;
            this.settings.ToUserName = textBoxToUserName.Text;
            this.settings.ToPassword = textBoxToPassword.Text;
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

                treeViewContentSelection.Nodes.Clear();
                this.UIToSettings();
                this.waitForm.Show();

                // this is your presumably long-running method
                Action exec = this.ApplyConfigurationAndLoadTree;
                BackgroundWorker b = new BackgroundWorker();

                // set the worker to try to login
                b.DoWork += (object sender1, DoWorkEventArgs e1) =>
                {
                    exec.Invoke();
                };

                // set the worker to close your progress form when it's completed
                b.RunWorkerCompleted += (object sender2, RunWorkerCompletedEventArgs e2) =>
                {
                    this.waitForm.Hide();
                    if (e2.Error != null)
                    {
                        MessageBox.Show(e2.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        this.tabControMain.SelectedTab = this.tabPageContentSelection;
                    }
                };

                // start the worker
                b.RunWorkerAsync();
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
        private void ApplyConfigurationAndLoadTree()
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

            this.LoadMigrateFromTree();
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
            this.destination = new ClientContext(this.settings.FromHost);
            var cc = new CredentialCache();
            cc.Add(new Uri(this.source.Url), "NTLM", new NetworkCredential(this.settings.ToUserName, this.settings.ToPassword, this.settings.ToDomain));
            this.source.Credentials = cc;
            this.source.ExecuteQuery();
        }

        /// <summary>
        /// Loads the migration Tree
        /// </summary>
        private void LoadMigrateFromTree()
        {
            ContentMigrator cm = new ContentMigrator(this.source, this.destination);
            sourceTreeRoot = cm.GenerateMigrationTree();
            this.treeViewContentSelection.Nodes.Add(sourceTreeRoot);
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
            // The code only executes if the user caused the checked state to change.
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
            // disable for migration configuration
            if (treeViewContentSelectionDisabled)
                e.Cancel = true;
        }

        /// <summary>
        /// Loads the items to configure to the listViewMigrationContent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonConfigureMigration_Click(object sender, EventArgs e)
        {
            if (sourceTreeRoot.Checked)
            {
                listViewMigrationContent.Items.Add(new SpListViewItem(sourceTreeRoot.MigrationObject));
            }
            foreach (TreeNode web in sourceTreeRoot.Nodes)
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
                }
            }

            //listViewMigrationContent.Items.AddRange(list);
            //a.GetType() == typeof(Dog)
        }

        /// <summary>
        /// Checks all checkboxes, if Content-Type is checked.
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">the EventArgs itself</param>
        private void checkBoxMigrateContentType_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMigrateContentType.Checked)
            {
                checkBoxMigrateGroup.Checked = true;
                checkBoxMigratePermissionlevels.Checked = true;
                checkBoxMigrateSiteColumns.Checked = true;
                checkBoxMigrateUser.Checked = true;
                checkBoxMigrateWorkflow.Checked = true;
            }
        }

        /// <summary>
        /// Migrates the elements.
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">EventArgs itself</param>
        private void buttonElementsMigration_Click(object sender, EventArgs e)
        {
            try
            {
                this.ValidateInputFields();
            }
            catch (ArgumentNullException ex)
            {
                this.tabPageConfiguration.Show();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (destination == null)
            {
                this.ConnectToDestination();
            }

            ElementsMigrationWorker migrationWorker = new ElementsMigrationWorker(source, destination);
            migrationWorker.StartMigration(checkBoxMigratePermissionlevels.Checked, checkBoxMigrateUser.Checked, checkBoxMigrateGroup.Checked, checkBoxMigrateSiteColumns.Checked, checkBoxMigratePermissionlevels.Checked, checkBoxMigrateWorkflow.Checked);
        }
    }
}
