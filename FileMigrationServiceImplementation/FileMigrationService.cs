//-----------------------------------------------------------------------
// <copyright file="FileMigrationService.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
namespace Sharezbold.FileMigration.Service
{
    using Contract;
    using Microsoft.SharePoint.Administration;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Logic of the webservice for FileMigration.
    /// </summary>
    public class FileMigrationService : IFileMigration
    {
        /// <summary>
        /// Returns the max file size per extension as dictionary.
        /// </summary>
        /// <returns>max file size per extension</returns>
        public IDictionary<string, int> GetMaxFileSizePerExtension()
        {
            Console.WriteLine("Called GetMaxFileSizePerExtension()");
            IDictionary<string, int> maxFileSizes = null;
            try
            {
                SPWebApplication webApplication = new SPWebApplication();
                maxFileSizes = webApplication.MaximumFileSizePerExtension;
                foreach (var item in maxFileSizes)
                {
                    Console.WriteLine("The max file size of '{0}' is '{1}' MegaByte", item.Key, item.Value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

            return maxFileSizes;
        }

        /// <summary>
        /// Gets the maximal file size.
        /// </summary>
        /// <returns>max file size</returns>
        public int GetMaxFileSize()
        {
            Console.WriteLine("Called GetMaxFileSizePerExtension()");
            int maxFileSize = -1;
            try
            {
                maxFileSize = new SPWebApplication().MaximumFileSize;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

            Console.WriteLine("The max file size is {0}", maxFileSize);

            return maxFileSize;
        }

        /// <summary>
        /// Returns the max message size.
        /// </summary>
        /// <returns>max message size</returns>
        public int GetMaxMessageSize()
        {
            Console.WriteLine("Called GetMaxMessageSize()");
            int maxMessageSize = -1;
            try
            {
                maxMessageSize = SPWebService.ContentService.ClientRequestServiceSettings.MaxReceivedMessageSize;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

            Console.WriteLine("The max message size is {0}", maxMessageSize);

            return maxMessageSize;
        }
    }
}
