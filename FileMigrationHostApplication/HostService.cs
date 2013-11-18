//-----------------------------------------------------------------------
// <copyright file="HostService.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.FileMigration.Host
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Threading.Tasks;
    using Contract;
    using Service;

    internal class HostService
    {
        private ServiceHost serviceHost;

        public HostService(Uri serviceUri)
        {
            serviceHost = new ServiceHost(typeof(FileMigrationService), serviceUri);
        }

        internal bool Start()
        {
            // ServiceHost svcHost = new ServiceHost(typeof(FileMigrationService), new Uri("http://sps2013003:12345/FileMigrationService"));
            try
            {
                // Check to see if the service host already has a ServiceMetadataBehavior
                ServiceMetadataBehavior smb = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                // If not, add one
                if (smb == null)
                {
                    smb = new ServiceMetadataBehavior();
                }

                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;

                serviceHost.Description.Behaviors.Add(smb);
                // Add application endpoint
                WSHttpBinding binding = new WSHttpBinding(SecurityMode.None);

                serviceHost.AddServiceEndpoint(typeof(IFileMigration), binding, "");

                // Open the service host to accept incoming calls
                serviceHost.Open();

                return true;
                // The service can now be accessed.
                /*
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();*/
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine("There was a communication problem. " + commProblem.Message);

                return false;
            }
        }

        internal async Task<bool> StartAsync()
        {
            return Start();
        }

        internal bool Stop()
        {
            try
            {
                serviceHost.Close();
                return true;
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine("There was a communication problem. " + commProblem.Message);

                return false;
            }
        }

        internal async Task<bool> StopAsync()
        {
            return Stop();
        }
    }
}
