//-----------------------------------------------------------------------
// <copyright file="UriBuilder.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration.Host
{
    using System;
    using System.Text;
    using Extensions;

    /// <summary>
    /// This class builds the uri.
    /// </summary>
    internal class UriBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UriBuilder"/> class.
        /// </summary>
        internal UriBuilder()
        {
        }

        /// <summary>
        /// Gets the uri.
        /// </summary>
        internal Uri Uri { get; private set; }

        /// <summary>
        /// Gets or sets the protocol.
        /// </summary>
        private string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the service address.
        /// </summary>
        private string ServiceAddress { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        private string Port { get; set; }

        /// <summary>
        /// Gets or sets the servicename.
        /// </summary>
        private string ServiceName { get; set; }

        /// <summary>
        /// Builds the uri with the user input.
        /// </summary>
        internal void BuildTheUriWithUserInput()
        {
            Console.WriteLine("Have to build the uri for the Service (like http://hostname:12345/FileMigrationService).");

            this.ReadTheProtocol();
            this.ReadTheServiceAddress();
            this.ReadThePort();
            this.ReadTheServiceName();

            this.BuildUri();
        }

        /// <summary>
        /// Reads the protocol.
        /// </summary>
        private void ReadTheProtocol()
        {
            this.Protocol = "http";
            Console.WriteLine("Using the DEFAULT protocol '{0}'", this.Protocol);
        }

        /// <summary>
        /// Reads the service address.
        /// </summary>
        private void ReadTheServiceAddress()
        {
            string hostname = System.Environment.MachineName;
            Console.WriteLine("Please enter the service-address. (DEFAULT: {0})", hostname);
            this.ServiceAddress = Console.ReadLine().Trim();

            try
            {
                this.ServiceAddress.IsNullOrEmpty();
            }
            catch (ValidationException)
            {
                Console.WriteLine("Using the DEFAULT name.");
                this.ServiceAddress = hostname;
            }
        }

        /// <summary>
        /// Reads the port.
        /// </summary>
        private void ReadThePort()
        {
            string port = "12345";
            Console.WriteLine("Please enter the port-number. (DEFAULT: {0})", port);
            this.Port = Console.ReadLine().Trim();

            try
            {
                this.Port.IsNumeric();
            }
            catch (ValidationException)
            {
                Console.WriteLine("Using the DEFAULT port.");
                this.Port = port;
            }
        }

        /// <summary>
        /// Reads the service name.
        /// </summary>
        private void ReadTheServiceName()
        {
            this.ServiceName = "FileMigrationService";
            Console.WriteLine("Using the DEFAULT service name '{0}'", this.ServiceName);
        }

        /// <summary>
        /// Builds the uri.
        /// </summary>
        private void BuildUri()
        {
            StringBuilder builder = new StringBuilder(this.Protocol);
            builder.Append("://");
            builder.Append(this.ServiceAddress);
            builder.Append(":");
            builder.Append(this.Port);
            builder.Append("/");
            builder.Append(this.ServiceName);

            this.Uri = new Uri(builder.ToString());
        }
    }
}
