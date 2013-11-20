//-----------------------------------------------------------------------
// <copyright file="Connector.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold
{
    using System;
    using System.Net;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Class to connect to a SharePoint with ClientContext.
    /// </summary>
    internal class Connector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Connector"/> class.
        /// </summary>
        public Connector()
        {
        }

        /// <summary>
        /// Connect to the ClientContext and returns an instance of it.
        /// </summary>
        /// <param name="host">host to connect</param>
        /// <param name="username">the username</param>
        /// <param name="password">the password</param>
        /// <param name="domain">domain of SharePoint</param>
        /// <param name="proxySettings">settings of proxy</param>
        /// <returns>instance of ClientContext if connection works, otherwise null will be returned</returns>
        internal ClientContext ConnectToClientContext(string host, string username, string password, string domain, ProxySettings proxySettings = null)
        {
            ClientContext clientContext = new ClientContext(host);
            this.SetProxy(clientContext, proxySettings);
            var cc = new CredentialCache();
            cc.Add(new Uri(clientContext.Url), "NTLM", new NetworkCredential(username, password, domain));
            clientContext.Credentials = cc;

            try
            {
                clientContext.ExecuteQuery();
                return clientContext;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the proxy for the connection to the server.
        /// </summary>
        /// <param name="clientContext">the clientcontext of the server</param>
        /// <param name="proxySettings">settings of proxy</param>
        private void SetProxy(ClientContext clientContext, ProxySettings proxySettings)
        {
            if (proxySettings != null)
            {
                clientContext.ExecutingWebRequest += (sen, args) =>
                {
                    System.Net.WebProxy myProxy = new System.Net.WebProxy();
                    myProxy.Address = proxySettings.Address;
                    myProxy.Credentials = new System.Net.NetworkCredential(proxySettings.Username, proxySettings.Password);
                    args.WebRequestExecutor.WebRequest.Proxy = myProxy;
                };
            }
        }
    }
}
