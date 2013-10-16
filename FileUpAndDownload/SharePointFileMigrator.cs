namespace Sharezbold.FileMigration
{
    using System;
    using System.Net;
    using WebReferenceCopy;
    using Microsoft.SharePoint.Client;

    internal class SharePointFileMigrator
    {
        private const string SHAREPOINT_2010_2013_COPY_SERVICE = @"_vti_bin/copy.asmx";
        private Copy sourceCopyService;
        private Copy destinationCopyService;

        public SharePointFileMigrator(ClientContext sourceClientContext, ClientContext destinationClientContext)
        {
            sourceCopyService = new Copy();
            destinationCopyService = new Copy();

            sourceCopyService.Url = PrepareServiceUrl(sourceClientContext.Url);
            destinationCopyService.Url = PrepareServiceUrl(destinationClientContext.Url);

            CredentialCache srcCredentials = new CredentialCache();
            srcCredentials.Add(new Uri(sourceClientContext.Url), "NTLM", (NetworkCredential)sourceClientContext.Credentials);

            CredentialCache destCredentials = new CredentialCache();
            destCredentials.Add(new Uri(destinationClientContext.Url), "NTLM", (NetworkCredential)destinationClientContext.Credentials);
            
            sourceCopyService.Credentials = srcCredentials;
            destinationCopyService.Credentials = destCredentials;
        }

        private string PrepareServiceUrl(string url)
        {
            url = url.Trim();

            if (url.EndsWith("/") )
            {
                url = url + SHAREPOINT_2010_2013_COPY_SERVICE;
            }
            else
            {
                url = url + "/" + SHAREPOINT_2010_2013_COPY_SERVICE;
            }

            return url;
        }

        internal void Migrate(string pathToSourceFile, string pathToDestinationFile)
        {
            FieldInformation myFieldInfo = new FieldInformation();
            FieldInformation[] myFieldInfoArray = { myFieldInfo };
            byte[] myByteArray;

            uint myGetUint = sourceCopyService.GetItem(pathToSourceFile,
                out myFieldInfoArray, out myByteArray);

            CopyResult myCopyResult1 = new CopyResult();
            CopyResult myCopyResult2 = new CopyResult();
            CopyResult[] myCopyResultArray = { myCopyResult1,  myCopyResult2 };

            try
            {
                string[] destionationFiles = { pathToDestinationFile};
                uint myCopyUint = destinationCopyService.CopyIntoItems(pathToSourceFile, destionationFiles,
                    myFieldInfoArray, myByteArray, out myCopyResultArray);
                if (myCopyUint == 0)
                {
                    int idx = 0;
                    foreach (CopyResult myCopyResult in myCopyResultArray)
                    {
                        string opString = (idx + 1).ToString();
                        if (myCopyResultArray[idx].ErrorMessage == null)
                        {
                            Console.WriteLine("Copy operation " + opString +
                                "completed.\r\n" + "Destination: " +
                                myCopyResultArray[idx].DestinationUrl);
                        }
                        else
                        {
                            Console.WriteLine("Copy operation " + opString +
                                " failed.\r\n" + "Error: " +
                                myCopyResultArray[idx].ErrorMessage + "\r\n" +
                                "Code: " + myCopyResultArray[idx].ErrorCode);
                        }
                        idx++;
                    }
                }
            }
            catch (Exception exc)
            {
                int idx = 0;
                foreach (CopyResult myCopyResult in myCopyResultArray)
                {
                    idx++;
                    if (myCopyResult.DestinationUrl == null)
                    {
                        string idxString = idx.ToString();
                        Console.WriteLine("Copy operation " + idxString +
                            " failed.\r\n" + "Description: " + exc.Message);
                    }
                }

            }
        }
    }
}
