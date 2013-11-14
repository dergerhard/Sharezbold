

namespace Sharezbold.FileMigration.Host
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Extensions;

    internal class UriBuilder
    {
        private string Protocol { get; set; }
        private string ServiceAddress { get; set; }
        private string Port { get; set; }
        private string ServiceName { get; set; }

        internal Uri Uri { get; private set; }

        internal void BuildTheUriWithUserInput()
        {
            Console.WriteLine("Have to build the uri for the Service (like http://hostname:12345/FileMigrationService).");

            this.ReadTheProtocol();
            this.ReadTheServiceAddress();
            this.ReadThePort();
            this.ReadTheServiceName();

            BuildUri();
        }

        private void ReadTheProtocol()
        {
            this.Protocol = "http";
            Console.WriteLine("Using the DEFAULT protocol '{0}'", this.Protocol);
        }

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

        private void ReadThePort()
        {
            string port = "12345";
            Console.WriteLine("Please enter the port-number. (DEFAULT: {0})", port);
            this.Port = Console.ReadLine().Trim();

            try
            {
                this.Port.IsNumberic();
            }
            catch (ValidationException)
            {
                Console.WriteLine("Using the DEFAULT port.");
                this.Port = port;
            }
        }

        private void ReadTheServiceName()
        {
            this.ServiceName = "FileMigrationService";
            Console.WriteLine("Using the DEFAULT service name '{0}'", this.ServiceName);
        }

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
