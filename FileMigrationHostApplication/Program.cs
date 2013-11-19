//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration.Host
{
    using System;

    /// <summary>
    /// The entrypoint class of the program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entrypoint of the application.
        /// </summary>
        /// <returns>status codes of the program. 0 is success.</returns>
        public static int Main()
        {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.BuildTheUriWithUserInput();

            HostService hostService = new HostService(uriBuilder.Uri);
            bool started = hostService.Start();

            if (started)
            {
                bool stopped = false;

                Console.WriteLine("Started the service. The uri of the service is '{0}'. Write <stop> to stop the service.", uriBuilder.Uri.ToString());
                while (!stopped)
                {
                    string command = Console.ReadLine();
                    if (command.Equals("stop"))
                    {
                        stopped = hostService.Stop();
                        if (!stopped)
                        {
                            Console.WriteLine("Could not stop the Service!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please write <stop> to stop the service.");
                        stopped = false;
                    }
                }
            }
            else
            {
                Console.WriteLine("Could not start the service! Press <ENTER> for exit.");
                Console.ReadKey();
                return 1;
            }

            Console.WriteLine("Service stopped! Please press <ENTER> to exit the program.");
            Console.ReadKey();

            return 0;
        }
    }
}
