
namespace Sharezbold
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// 1. Automatic binding with listbox
    /// 2. developer messages that are not shown to user
    /// 3. write to log file
    /// 
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
                Console.WriteLine("Logger: not possible to open log file. Reason: " + e.Message);
            }
            
        }

        /// <summary>
        /// Writes a message to the log and syncs it with the list box
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="onlyLogFile">if true, it is only written to the log file</param>
        public void addMessage(string message, bool onlyLogFile = false)
        {
            string msg = DateTime.Now.ToString("HH:mm:ss") + " " + message;

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
            }
        }
    }
}
