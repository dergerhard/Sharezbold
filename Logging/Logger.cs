//-----------------------------------------------------------------------
// <copyright file="Logger.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// This class provides thread-safe logging to a list box and a file
    /// </summary>
    public sealed class Logger
    {
        /// <summary>
        /// user log list
        /// </summary>
        private List<string> userLog;

        /// <summary>
        /// BindingSource for user log (for binding with e.g. list box)
        /// </summary>
        private BindingSource userLogBindingSource = null;

        /// <summary>
        /// the file to write log messages to
        /// </summary>
        private TextWriter logfile = null;

        /// <summary>
        /// Specifies if the messages are sent only to debug. Can only be true, if the default constructor is used.
        /// </summary>
        private bool writeOnlyToDebug = false;

        /// <summary>
        /// The list box
        /// </summary>
        private ListBox dataBindBox = null;

        /// <summary>
        /// Specifies the number of tabs in front of the log text.
        /// </summary>
        private uint indent = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class. Logger constructor for binding with list box and log-file
        /// </summary>
        /// <param name="dataBind">The list box object to display the log entries</param>
        /// <param name="logfileurl">The log file to save the log entries to</param>
        public Logger(ListBox dataBind, string logfileurl)
        {
            this.userLog = new List<string>();
            this.dataBindBox = dataBind;

            this.userLogBindingSource = new BindingSource();
            this.userLogBindingSource.DataSource = this.userLog;
            this.dataBindBox.DataSource = this.userLogBindingSource;

            try
            {
                if (!Directory.Exists(logfileurl))
                {
                    Directory.CreateDirectory(logfileurl);
                }
                this.logfile = new StreamWriter(Path.Combine(logfileurl, "SharezboldLog.txt"));
            }
            catch (Exception e)
            {
                Debug.WriteLine("Logger: not possible to open log file. Reason: " + e.Message);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class. Default constructor. All messages are displayed in debug.
        /// </summary>
        public Logger()
        {
            this.writeOnlyToDebug = true;
        }
                
        /// <summary>
        /// Gets or sets the number of tabs in front of the log text. Maximum is 5. The indention stays the same, till it is set to another value.
        /// </summary>
        public uint Indent
        {
            get 
            { 
                return this.indent; 
            }

            set
            {
                if (value > 5)
                {
                    this.indent = 5;
                }
                else
                {
                    this.indent = value;
                }
            }
        }

        /// <summary>
        /// Writes a message to the log and syncs it with the list box. If default constructor was used, the message is displayed in the console
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="onlyLogFile">if true, it is only written to the log file</param>
        public void AddMessage(string message, bool onlyLogFile = false)
        {
            if (!message.Equals(string.Empty))
            {
                string timeIndent = new string(' ', 5);
                string tabs = new string(' ', ((int)this.indent) * 10);
                string msg = DateTime.Now.ToString("HH:mm:ss") + timeIndent + tabs + message;

                if (this.writeOnlyToDebug)
                {
                    Debug.WriteLine(msg);
                }
                else
                {
                    if (onlyLogFile == false)
                    {
                        lock (this.userLogBindingSource)
                        {
                            this.userLogBindingSource.Add(msg);
                            this.dataBindBox.SetSelected(this.dataBindBox.Items.Count - 1, true);
                        }
                    }

                    lock (this.logfile)
                    {
                        this.logfile.WriteLine(msg);
                        this.logfile.Flush();
                    }
                }
            }
        }
    }
}
