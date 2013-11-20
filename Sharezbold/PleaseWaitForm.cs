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

        /// <summary>
        /// Gets or sets the SpecialText, which is an additional loding information label
        /// </summary>
        public string SpecialText 
        { 
            get 
            { 
                return this.labelSpecialText.Text; 
            } 
            
            set 
            { 
                this.labelSpecialText.Text = value;  
            }
        }

        /// <summary>
        /// CreatParams is here used to avoid the possibility of closing the window
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_NOCLOSE = 0x200;

                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_NOCLOSE;
                return cp;
            }
        }
    }
}
