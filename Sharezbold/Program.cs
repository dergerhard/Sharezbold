//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Sharezbold.ContentMigration;

    /// <summary>
    /// Main Program
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            try
            {
                //Test t = new Test();
                //t.abc();

                //ContentLoader loader = new ContentLoader(@"http://ss13-css-009:31920/", "Administrator", "cssdev", "P@ssw0rd", @"http://ss13-css-007:5485/", @"http://ss13-css-007:8080/", "Administrator", "cssdev", "P@ssw0rd");
                //loader.LoadSourceData();
                
                //loader.test();
                //TEST t = new TEST();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.Message);
                MessageBox.Show("Unexpected Error: " + e.Message, "FATAL ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }	
        }
    }
}
