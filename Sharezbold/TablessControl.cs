//-----------------------------------------------------------------------
// <copyright file="TablessControl.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gerhard Liebmann (86240@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// TabControl without tabs
    /// </summary>
    public class TablessControl : TabControl
    {
        /// <summary>
        /// disables the tabs
        /// </summary>
        /// <param name="m">message of i don't know, look up stack overflow</param>
        protected override void WndProc(ref Message m)
        {
            // Hide tabs by trapping the TCM_ADJUSTRECT message
            if (m.Msg == 0x1328 && !this.DesignMode)
            {
                m.Result = (IntPtr)1;
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
