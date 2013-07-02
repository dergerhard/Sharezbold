namespace SharezboldUI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Serialization;
    using SharezboldUI.Settings;
    
    /// <summary>
    /// main form of the UI
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// next allowed tab page
        /// </summary>
        private TabPage allowNext;

        /// <summary>
        /// previous allowed tab page
        /// </summary>
        private TabPage allowPrevious;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm" />
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
            this.allowNext = this.tabPageContentSelection;
            this.allowPrevious = null;
            this.Size = new Size(this.Size.Width, this.Size.Height - 25);
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
            this.allowNext = this.tabPageMigrationProgress;
            this.allowPrevious = this.tabPageContentSelection;
        }

        /// <summary>
        /// copies settings to the user interface
        /// </summary>
        /// <param name="settings">settings to apply to UI</param>
        private void SettingsToUI(MigrationSettings settings)
        {
            textBoxFromDomain.Text = settings.FromDomain;
            textBoxFromHost.Text = settings.FromHost;
            textBoxFromUserName.Text = settings.FromUserName;
            textBoxFromPassword.Text = settings.FromPassword;

            textBoxToDomain.Text = settings.ToDomain;
            textBoxToHost.Text = settings.ToHost;
            textBoxToUserName.Text = settings.ToUserName;
            textBoxToPassword.Text = settings.ToPassword;
        }

        /// <summary>
        /// copies settings from the user interface to the MigrationSettings class
        /// </summary>
        /// <returns>settings from UI</returns>
        private MigrationSettings UIToSettings()
        {
            MigrationSettings settings = new MigrationSettings();
            settings.FromDomain = textBoxFromDomain.Text;
            settings.FromHost = textBoxFromHost.Text;
            settings.FromUserName = textBoxFromUserName.Text;
            settings.FromPassword = textBoxFromPassword.Text;

            settings.ToDomain = textBoxToDomain.Text;
            settings.ToHost = textBoxToHost.Text;
            settings.ToUserName = textBoxToUserName.Text;
            settings.ToPassword = textBoxToPassword.Text;
            return settings;
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
                    // TODO: Logger.....(xe.tostring)
                    MessageBox.Show("XML reading error. The settings file is corrupted!");
                }
                catch (Exception ex)
                {
                    // TODO: Logger.....(xe.tostring)
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
                    MessageBox.Show("XML writing error. The settings file could not be written correctly!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not write file to disk. Original error: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Previous button of content selection clicked
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event of the sender</param>
        private void ButtonSelectionPrevious_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPageConfiguration;
            this.allowPrevious = null;
            this.allowNext = this.tabPageContentSelection;
        }

        /// <summary>
        /// Next Button of configuration page clicked
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">event of sender</param>
        private void ButtonConfigurationNext_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPageContentSelection;
            this.allowNext = this.tabPageMigrationProgress;
            this.allowPrevious = this.tabPageConfiguration;
        }
    }
}
