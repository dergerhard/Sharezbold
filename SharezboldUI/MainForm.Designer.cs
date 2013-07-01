namespace SharezboldUI
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.dateiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbConfigSource = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFromHost = new System.Windows.Forms.TextBox();
            this.textBoxFromUserName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxFromDomain = new System.Windows.Forms.TextBox();
            this.textBoxFromPassword = new System.Windows.Forms.TextBox();
            this.tbConfigDestination = new System.Windows.Forms.TabPage();
            this.textBoxContentSelection = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonFromNext = new System.Windows.Forms.Button();
            this.tbMigrationProgress = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxToHost = new System.Windows.Forms.TextBox();
            this.textBoxToUserName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxToDomain = new System.Windows.Forms.TextBox();
            this.textBoxToPassword = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonToNext = new System.Windows.Forms.Button();
            this.buttonToPrevious = new System.Windows.Forms.Button();
            this.treeViewContentSelection = new System.Windows.Forms.TreeView();
            this.listBoxMigrationLog = new System.Windows.Forms.ListBox();
            this.buttonStartMigration = new System.Windows.Forms.Button();
            this.loadMigrationProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMigrationProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tbConfigSource.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tbConfigDestination.SuspendLayout();
            this.textBoxContentSelection.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tbMigrationProgress.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dateiToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1059, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // dateiToolStripMenuItem
            // 
            this.dateiToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadMigrationProfileToolStripMenuItem,
            this.saveMigrationProfileToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.dateiToolStripMenuItem.Name = "dateiToolStripMenuItem";
            this.dateiToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.dateiToolStripMenuItem.Text = "Datei";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Beenden";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tbConfigSource);
            this.tabControl1.Controls.Add(this.tbConfigDestination);
            this.tabControl1.Controls.Add(this.textBoxContentSelection);
            this.tabControl1.Controls.Add(this.tbMigrationProgress);
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1047, 522);
            this.tabControl1.TabIndex = 1;
            // 
            // tbConfigSource
            // 
            this.tbConfigSource.Controls.Add(this.buttonFromNext);
            this.tbConfigSource.Controls.Add(this.groupBox1);
            this.tbConfigSource.Location = new System.Drawing.Point(4, 22);
            this.tbConfigSource.Name = "tbConfigSource";
            this.tbConfigSource.Padding = new System.Windows.Forms.Padding(3);
            this.tbConfigSource.Size = new System.Drawing.Size(1039, 496);
            this.tbConfigSource.TabIndex = 0;
            this.tbConfigSource.Text = "Migrate From";
            this.tbConfigSource.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxFromHost, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxFromUserName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBoxFromDomain, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBoxFromPassword, 1, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(19, 30);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(377, 109);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Host:";
            // 
            // textBoxFromHost
            // 
            this.textBoxFromHost.Location = new System.Drawing.Point(67, 3);
            this.textBoxFromHost.Name = "textBoxFromHost";
            this.textBoxFromHost.Size = new System.Drawing.Size(300, 20);
            this.textBoxFromHost.TabIndex = 2;
            // 
            // textBoxFromUserName
            // 
            this.textBoxFromUserName.Location = new System.Drawing.Point(67, 29);
            this.textBoxFromUserName.Name = "textBoxFromUserName";
            this.textBoxFromUserName.Size = new System.Drawing.Size(300, 20);
            this.textBoxFromUserName.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Domain:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Password:";
            // 
            // textBoxFromDomain
            // 
            this.textBoxFromDomain.Location = new System.Drawing.Point(67, 55);
            this.textBoxFromDomain.Name = "textBoxFromDomain";
            this.textBoxFromDomain.Size = new System.Drawing.Size(300, 20);
            this.textBoxFromDomain.TabIndex = 6;
            // 
            // textBoxFromPassword
            // 
            this.textBoxFromPassword.Location = new System.Drawing.Point(67, 81);
            this.textBoxFromPassword.Name = "textBoxFromPassword";
            this.textBoxFromPassword.PasswordChar = '*';
            this.textBoxFromPassword.Size = new System.Drawing.Size(300, 20);
            this.textBoxFromPassword.TabIndex = 7;
            // 
            // tbConfigDestination
            // 
            this.tbConfigDestination.Controls.Add(this.buttonToPrevious);
            this.tbConfigDestination.Controls.Add(this.buttonToNext);
            this.tbConfigDestination.Controls.Add(this.groupBox2);
            this.tbConfigDestination.Location = new System.Drawing.Point(4, 22);
            this.tbConfigDestination.Name = "tbConfigDestination";
            this.tbConfigDestination.Size = new System.Drawing.Size(1039, 496);
            this.tbConfigDestination.TabIndex = 2;
            this.tbConfigDestination.Text = "Migrate To";
            this.tbConfigDestination.UseVisualStyleBackColor = true;
            // 
            // textBoxContentSelection
            // 
            this.textBoxContentSelection.Controls.Add(this.buttonStartMigration);
            this.textBoxContentSelection.Controls.Add(this.treeViewContentSelection);
            this.textBoxContentSelection.Location = new System.Drawing.Point(4, 22);
            this.textBoxContentSelection.Name = "textBoxContentSelection";
            this.textBoxContentSelection.Padding = new System.Windows.Forms.Padding(3);
            this.textBoxContentSelection.Size = new System.Drawing.Size(1039, 496);
            this.textBoxContentSelection.TabIndex = 1;
            this.textBoxContentSelection.Text = "Content Selection";
            this.textBoxContentSelection.UseVisualStyleBackColor = true;
            this.textBoxContentSelection.Click += new System.EventHandler(this.TextBoxContentSelection_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(6, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1028, 157);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source Configuration";
            // 
            // buttonFromNext
            // 
            this.buttonFromNext.Location = new System.Drawing.Point(958, 178);
            this.buttonFromNext.Name = "buttonFromNext";
            this.buttonFromNext.Size = new System.Drawing.Size(75, 23);
            this.buttonFromNext.TabIndex = 2;
            this.buttonFromNext.Text = "next";
            this.buttonFromNext.UseVisualStyleBackColor = true;
            this.buttonFromNext.Click += new System.EventHandler(this.ButtonFromNext_Click);
            // 
            // tbMigrationProgress
            // 
            this.tbMigrationProgress.Controls.Add(this.listBoxMigrationLog);
            this.tbMigrationProgress.Location = new System.Drawing.Point(4, 22);
            this.tbMigrationProgress.Name = "tbMigrationProgress";
            this.tbMigrationProgress.Size = new System.Drawing.Size(1039, 496);
            this.tbMigrationProgress.TabIndex = 3;
            this.tbMigrationProgress.Text = "Migration Progress";
            this.tbMigrationProgress.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBoxToHost, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBoxToUserName, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label8, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.textBoxToDomain, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.textBoxToPassword, 1, 3);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(19, 30);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(377, 109);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Host:";
            // 
            // textBoxToHost
            // 
            this.textBoxToHost.Location = new System.Drawing.Point(67, 3);
            this.textBoxToHost.Name = "textBoxToHost";
            this.textBoxToHost.Size = new System.Drawing.Size(300, 20);
            this.textBoxToHost.TabIndex = 2;
            // 
            // textBoxToUserName
            // 
            this.textBoxToUserName.Location = new System.Drawing.Point(67, 29);
            this.textBoxToUserName.Name = "textBoxToUserName";
            this.textBoxToUserName.Size = new System.Drawing.Size(300, 20);
            this.textBoxToUserName.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Username:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Domain:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 78);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Password:";
            // 
            // textBoxToDomain
            // 
            this.textBoxToDomain.Location = new System.Drawing.Point(67, 55);
            this.textBoxToDomain.Name = "textBoxToDomain";
            this.textBoxToDomain.Size = new System.Drawing.Size(300, 20);
            this.textBoxToDomain.TabIndex = 6;
            // 
            // textBoxToPassword
            // 
            this.textBoxToPassword.Location = new System.Drawing.Point(67, 81);
            this.textBoxToPassword.Name = "textBoxToPassword";
            this.textBoxToPassword.PasswordChar = '*';
            this.textBoxToPassword.Size = new System.Drawing.Size(300, 20);
            this.textBoxToPassword.TabIndex = 7;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel2);
            this.groupBox2.Location = new System.Drawing.Point(8, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1014, 152);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Destination Configuration";
            // 
            // buttonToNext
            // 
            this.buttonToNext.Location = new System.Drawing.Point(947, 173);
            this.buttonToNext.Name = "buttonToNext";
            this.buttonToNext.Size = new System.Drawing.Size(75, 23);
            this.buttonToNext.TabIndex = 3;
            this.buttonToNext.Text = "next";
            this.buttonToNext.UseVisualStyleBackColor = true;
            this.buttonToNext.Click += new System.EventHandler(this.ButtonToNext_Click);
            // 
            // buttonToPrevious
            // 
            this.buttonToPrevious.Location = new System.Drawing.Point(866, 173);
            this.buttonToPrevious.Name = "buttonToPrevious";
            this.buttonToPrevious.Size = new System.Drawing.Size(75, 23);
            this.buttonToPrevious.TabIndex = 4;
            this.buttonToPrevious.Text = "previous";
            this.buttonToPrevious.UseVisualStyleBackColor = true;
            this.buttonToPrevious.Click += new System.EventHandler(this.ButtonToPrevious_Click);
            // 
            // treeViewContentSelection
            // 
            this.treeViewContentSelection.Location = new System.Drawing.Point(8, 15);
            this.treeViewContentSelection.Name = "treeViewContentSelection";
            this.treeViewContentSelection.Size = new System.Drawing.Size(1012, 417);
            this.treeViewContentSelection.TabIndex = 0;
            // 
            // listBoxMigrationLog
            // 
            this.listBoxMigrationLog.FormattingEnabled = true;
            this.listBoxMigrationLog.Location = new System.Drawing.Point(8, 15);
            this.listBoxMigrationLog.Name = "listBoxMigrationLog";
            this.listBoxMigrationLog.Size = new System.Drawing.Size(1014, 446);
            this.listBoxMigrationLog.TabIndex = 0;
            // 
            // buttonStartMigration
            // 
            this.buttonStartMigration.Location = new System.Drawing.Point(910, 438);
            this.buttonStartMigration.Name = "buttonStartMigration";
            this.buttonStartMigration.Size = new System.Drawing.Size(110, 23);
            this.buttonStartMigration.TabIndex = 1;
            this.buttonStartMigration.Text = "start migration";
            this.buttonStartMigration.UseVisualStyleBackColor = true;
            this.buttonStartMigration.Click += new System.EventHandler(this.ButtonStartMigration_Click);
            // 
            // loadMigrationProfileToolStripMenuItem
            // 
            this.loadMigrationProfileToolStripMenuItem.Name = "loadMigrationProfileToolStripMenuItem";
            this.loadMigrationProfileToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.loadMigrationProfileToolStripMenuItem.Text = "Load Migration profile";
            this.loadMigrationProfileToolStripMenuItem.Click += new System.EventHandler(this.LoadMigrationProfileToolStripMenuItem_Click);
            // 
            // saveMigrationProfileToolStripMenuItem
            // 
            this.saveMigrationProfileToolStripMenuItem.Name = "saveMigrationProfileToolStripMenuItem";
            this.saveMigrationProfileToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.saveMigrationProfileToolStripMenuItem.Text = "Save Migration profile";
            this.saveMigrationProfileToolStripMenuItem.Click += new System.EventHandler(this.SaveMigrationProfileToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1059, 561);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Sharezbold-UI";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tbConfigSource.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tbConfigDestination.ResumeLayout(false);
            this.textBoxContentSelection.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tbMigrationProgress.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem dateiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tbConfigSource;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxFromHost;
        private System.Windows.Forms.TextBox textBoxFromUserName;
        private System.Windows.Forms.TabPage textBoxContentSelection;
        private System.Windows.Forms.TabPage tbConfigDestination;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxFromDomain;
        private System.Windows.Forms.TextBox textBoxFromPassword;
        private System.Windows.Forms.Button buttonFromNext;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonToPrevious;
        private System.Windows.Forms.Button buttonToNext;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxToHost;
        private System.Windows.Forms.TextBox textBoxToUserName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxToDomain;
        private System.Windows.Forms.TextBox textBoxToPassword;
        private System.Windows.Forms.TreeView treeViewContentSelection;
        private System.Windows.Forms.TabPage tbMigrationProgress;
        private System.Windows.Forms.ListBox listBoxMigrationLog;
        private System.Windows.Forms.Button buttonStartMigration;
        private System.Windows.Forms.ToolStripMenuItem loadMigrationProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMigrationProfileToolStripMenuItem;
    }
}

