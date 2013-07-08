//-----------------------------------------------------------------------
// <copyright file="PleaseWaitForm.cs" company="FH Wiener Neustadt">
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
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// Used to indicate a work in progress
    /// </summary>
    public partial class PleaseWaitForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PleaseWaitForm"/> class.
        /// </summary>
        public PleaseWaitForm()
        {
            this.InitializeComponent();
        }
    }
}
