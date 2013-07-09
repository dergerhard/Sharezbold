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
    using Sharezbold.ContentMigration;
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
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
            
            // this.Size = new Size(this.Size.Width, this.Size.Height - 25); //Todo: use tablessControl
            this.treeViewContentSelection.CheckBoxes = true;

            // TODO: delete this
            TextReader reader = new StreamReader(@"C:\temp\test.xml");
            /*XmlSerializer serializer = new XmlSerializer(typeof(MigrationSettings));
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
            this.tabControl1.SelectedTab = this.tabPageMigrationProgress;
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
                        this.tabControl1.SelectedTab = this.tabPageContentSelection;
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
        }

        /// <summary>
        /// Loads the migration Tree
        /// </summary>
        private void LoadMigrateFromTree()
        {
            ContentMigrator cm = new ContentMigrator(this.source, this.destination);
            this.treeViewContentSelection.Nodes.Add(cm.GenerateMigrationTree());
                
            /*
            Web web = source.Web;

            // Retrieve all lists from the server. 
            source.Load(web.Lists,
                         lists => lists.Include(list => list.Title, // For Each list, retrieve Title and Id
                                                list => list.Id));

            // Execute query. 
            source.ExecuteQuery();

            // Enumerate the web.Lists. 
            foreach (List list in web.Lists)
            {
                Debug.WriteLine(list.Title);
            }

            try
            {
                
                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
                MessageBox.Show(exception.Message);
            }
            finally
            {
                //if (writer != null) writer.Close();
                //if (fileStream != null) fileStream.Close();
            }*/
        }

        /// <summary>
        /// Sets all elements to selected, when Content-Type is choosen.
        /// </summary>
        /// <param name="sender">the sender of the event</param>
        /// <param name="e">the event-args itself</param>
        private void checkBoxContentTypeMigration_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxContentTypeMigration.Checked)
            {
                this.checkBoxGroups.Checked = true;
                this.checkBoxPermissionLevels.Checked = true;
                this.checkBoxSiteColumns.Checked = true;
                this.checkBoxUsers.Checked = true;
                this.checkBoxWorkflows.Checked = true;
            }
        }

        private void buttonStartElementsMigration_Click(object sender, EventArgs e)
        {
            //// TODO start elements migration
        }
    }
}
