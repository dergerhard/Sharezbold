

namespace Sharezbold.FileMigration.Host
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class MainWindow : Form
    {
        private HostService hostService;
        private bool serviceStarted;
        public MainWindow()
        {
            InitializeComponent();

            serviceStarted = false;

            this.comboBoxProtocol.SelectedItem = "http://";
            this.comboBoxProtocol.Enabled = false;
        }

        private async void ButtonStartStopServiceClick(object sender, EventArgs e)
        {
            if (!serviceStarted)
            {
                TrimTextInTextBoxes();
                ValidateTextInTextBoxes();

                string uriAsString = GetInputValuesAsUri();

                hostService = new HostService(new Uri(uriAsString));
                Task<bool> task = hostService.StartAsync();
                bool started = await task;

                if (started)
                {
                    this.rectangleShapeStatus.BackColor = Color.Green;
                    this.buttonStartStopService.Text = "Stop Service";
                }
            }
            else
            {
                Task<bool> task = hostService.StopAsync();
                bool stopped = await task;

                if (stopped)
                {
                    this.rectangleShapeStatus.BackColor = Color.Red;
                    this.buttonStartStopService.Text = "Start Service";
                    hostService = null;
                }
            }
        }

        private void TrimTextInTextBoxes()
        {
            this.textBoxServiceName.Text = this.textBoxServiceName.Text.Trim();
            this.textBoxPort.Text = this.textBoxPort.Text.Trim();
            this.textBoxServiceAddress.Text = this.textBoxServiceAddress.Text.Trim();
        }

        private void ValidateTextInTextBoxes()
        {
            if (this.textBoxServiceName.Text == null)
            {
                throw new FileMigrationHostException("The Service-Name must not be null!");
            }

            string portNumber = this.textBoxPort.Text;
            int tempPortNumber;
            if (portNumber == null)
            {
                throw new FileMigrationHostException("The Port-Number must not be null!");
            }
            else if (!int.TryParse(portNumber, out tempPortNumber))
            {
                throw new FileMigrationHostException("The Port-Number must not be numberic!");
            }

            if (this.textBoxServiceAddress.Text == null)
            {
                throw new FileMigrationHostException("The Service-Address must not be null!");
            }
        }

        private string GetInputValuesAsUri()
        {
            StringBuilder stringBuilder = new StringBuilder((string) this.comboBoxProtocol.SelectedItem);
            stringBuilder.Append(this.textBoxServiceAddress.Text);
            stringBuilder.Append(":");
            stringBuilder.Append(this.textBoxPort.Text);
            stringBuilder.Append("/");
            stringBuilder.Append(this.textBoxServiceName.Text);

            return stringBuilder.ToString();
        }
    }
}
