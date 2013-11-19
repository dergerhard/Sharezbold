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

    /// <summary>
    /// HostService is responsible for connecting and closing the connection to the FileMigrationService.
    /// </summary>
    internal class HostService
    {
        /// <summary>
        /// Instance of the ServiceHost.
        /// </summary>
        private ServiceHost serviceHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostService"/> class.
        /// </summary>
        /// <param name="serviceUri">the uri of the file migration service</param>
        public HostService(Uri serviceUri)
        {
            this.serviceHost = new ServiceHost(typeof(FileMigrationService), serviceUri);
        }

        /// <summary>
        /// Starts and opens the connection to the service
        /// </summary>
        /// <returns>true if success, otherwise false</returns>
        internal bool Start()
        {
            try
            {
                //// Check to see if the service host already has a ServiceMetadataBehavior
                ServiceMetadataBehavior smb = this.serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                //// If not, add one
                if (smb == null)
                {
                    smb = new ServiceMetadataBehavior();
                }

                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;

                this.serviceHost.Description.Behaviors.Add(smb);
                //// Add application endpoint
                WSHttpBinding binding = new WSHttpBinding(SecurityMode.None);

                this.serviceHost.AddServiceEndpoint(typeof(IFileMigration), binding, string.Empty);

                //// Open the service host to accept incoming calls
                this.serviceHost.Open();

                return true;
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine("There was a communication problem. " + commProblem.Message);

                return false;
            }
        }

        /// <summary>
        /// Stops the connection to the service.
        /// </summary>
        /// <returns>true if success, otherwise false</returns>
        internal bool Stop()
        {
            try
            {
                this.serviceHost.Close();
                return true;
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine("There was a communication problem. " + commProblem.Message);

                return false;
            }
        }
    }
}
