//-----------------------------------------------------------------------
// <copyright file="LoadingElementsException.cs" company="FH Wiener Neustadt">
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
    /// This class provides thread-safe logging to a listbox and a file
    /// </summary>
    public sealed class Logger
    {
        /// <summary>
        /// user log list
        /// </summary>
        private List<string> userLog;

        /// <summary>
        /// BindingSource for user log (for binding with e.g. listbox)
        /// </summary>
        private BindingSource userLogBindingSource = null;

        /// <summary>
        /// the file to wirte log messages to
        /// </summary>
        private TextWriter logfile = null;

        /// <summary>
        /// Specifies if the messages are sent only to debug. Can only be true, if the default constructor is used.
        /// </summary>
        private bool writeOnlyToDebug = false;

        /// <summary>
        /// Logger constructor for binding with listbox and log-file
        /// </summary>
        /// <param name="dataBind">The listbox object to display the log entries</param>
        /// <param name="logfileurl">The log file to save the log entries to</param>
        public Logger(ListBox dataBind, string logfileurl)
        {
            this.userLog = new List<string>();

            this.userLogBindingSource = new BindingSource();
            this.userLogBindingSource.DataSource = userLog;
            dataBind.DataSource = this.userLogBindingSource;

            try
            {
                logfile = new StreamWriter(logfileurl);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Logger: not possible to open log file. Reason: " + e.Message);
            }
            
        }

        /// <summary>
        /// Default constructor. All messages are displayed in debug.
        /// </summary>
        public Logger()
        {
            this.writeOnlyToDebug = true;
        }

        /// <summary>
        /// Writes a message to the log and syncs it with the list box. If default constructor was used, the message is displayed in the console
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="onlyLogFile">if true, it is only written to the log file</param>
        public void AddMessage(string message, bool onlyLogFile = false)
        {
            string msg = DateTime.Now.ToString("HH:mm:ss") + " " + message;

            if (this.writeOnlyToDebug)
            {
                Debug.WriteLine(msg);
            }
            else
            {
                if (onlyLogFile == false)
                {
                    lock (userLogBindingSource)
                    {
                        userLogBindingSource.Add(msg);
                    }
                }

                lock (logfile)
                {
                    logfile.WriteLine(msg);
                    logfile.Flush();
                }
            }
        }
    }
}
