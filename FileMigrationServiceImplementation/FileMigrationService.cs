
namespace Sharezbold.FileMigration.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Contract;
    using Microsoft.SharePoint.Administration;

    public class FileMigrationService : IFileMigration
    {
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


        // TODO: implement it
        public void BlockedFileExtension()
        {
            Console.WriteLine("Called BlockedFileExtension()");
            SPWebApplication webApplication = new SPWebApplication();
            System.Collections.ObjectModel.Collection<string> blocked = webApplication.BlockedFileExtensions;
            foreach (string entry in blocked)
            {
                Console.WriteLine("blocked entry is {0}", entry);
            }
        }
    }
}
