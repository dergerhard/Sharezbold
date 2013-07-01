namespace SharezboldUI
{
    using SharezboldUI.Settings;
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

    /// <summary>
    /// main formular of the ui
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Exit the application
        /// </summary>
        /// <param name="sender">sender of the action</param>
        /// <param name="e">the event which was executed</param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonFromNext_Click(object sender, EventArgs e)
        {
        }

        private void ButtonToPrevious_Click(object sender, EventArgs e)
        {
        }

        private void ButtonToNext_Click(object sender, EventArgs e)
        {
        }

        private void TextBoxContentSelection_Click(object sender, EventArgs e)
        {
        }

        private void ButtonStartMigration_Click(object sender, EventArgs e)
        {
        }

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
                    SettingsToUI(settings);
                }
                catch (XmlException xe)
                {
                    MessageBox.Show("XML reading error. The settings file is corrupted!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

        }

        

        private void SaveMigrationProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MigrationSettings settings = UIToSettings();

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML files (*.xml)|*.xml";
            saveFileDialog1.Title = "Save the current profile";
            saveFileDialog1.ShowDialog();
            
            if (saveFileDialog1.FileName != "")
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


    }
}
