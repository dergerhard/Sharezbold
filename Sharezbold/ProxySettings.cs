//-----------------------------------------------------------------------
// <copyright file="ProxySettings.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold
{
    using System;

    /// <summary>
    /// Class which represents the settings of the proxy.
    /// </summary>
    internal class ProxySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxySettings"/> class.
        /// </summary>
        /// <param name="address">address of proxy</param>
        /// <param name="username">username for proxy</param>
        /// <param name="password">password for proxy</param>
        public ProxySettings(string address = null, string username = null, string password = null)
        {
        }

        /// <summary> 
        /// Gets or sets the address of the proxy. 
        /// </summary> 
        internal Uri Address { get; set; }

        /// <summary> 
        /// Gets or sets the usernam of the proxy. 
        /// </summary> 
        internal string Username { get; set; }

        /// <summary> 
        /// Gets or sets the password of the proxy. 
        /// </summary> 
        internal string Password { get; set; }
    }
}
