namespace Sharezbold.FileMigration
{
    using System;
    using System.Net;
    using System.Linq;
    using System.Collections;
    using WebReferenceCopy;
    using Microsoft.SharePoint.Client;

    internal class SharePointFileMigrator
    {
        private const string SHAREPOINT_2010_2013_COPY_SERVICE = @"_vti_bin/copy.asmx";
        private Copy sourceCopyService;
        private Copy destinationCopyService;

        private ClientContext srcContext;
        private ClientContext destContext;

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

            this.srcContext = sourceClientContext;
            this.destContext = destinationClientContext;
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

        public void CopyDocuments(string srcLibrary, string destLibrary)
        {
            // get the list and items
            Web srcWeb = srcContext.Web;
            List srcList = srcWeb.Lists.GetByTitle(srcLibrary);
            ListItemCollection col = srcList.GetItems(new CamlQuery());
            srcContext.Load(col);
            srcContext.ExecuteQuery();

            // get the new list
            Web destWeb = destContext.Web;
            destContext.Load(destWeb);
            destContext.ExecuteQuery();

            foreach (var doc in col)
            {
                try
                {
                    if (doc.FileSystemObjectType == FileSystemObjectType.File)
                    {
                        // get the file
                        File f = doc.File;
                        srcContext.Load(f);
                        srcContext.ExecuteQuery();

                        // build new location url
                        string nLocation = destWeb.ServerRelativeUrl.TrimEnd('/') + "/" + destLibrary.Replace(" ", "") + "/" + f.Name;

                        // read the file, copy the content to new file at new location
                        FileInformation fileInfo = File.OpenBinaryDirect(srcContext, f.ServerRelativeUrl);
                        File.SaveBinaryDirect(destContext, nLocation, fileInfo.Stream, true);
                    }

                    if (doc.FileSystemObjectType == FileSystemObjectType.Folder)
                    {
                        // load the folder
                        srcContext.Load(doc);
                        srcContext.ExecuteQuery();

                        // get the folder data, get the file collection in the folder
                        Folder folder = srcWeb.GetFolderByServerRelativeUrl(doc.FieldValues["FileRef"].ToString());
                        FileCollection fileCol = folder.Files;

                        // load everyting so we can access it
                        srcContext.Load(folder);
                        srcContext.Load(fileCol);
                        srcContext.ExecuteQuery();

                        // create the folder at the new document library if it doesn't already exist
                        AddNewFolder(destContext, srcLibrary.Replace(" ", ""), srcLibrary, "Folder", folder.Name);

                        foreach (File f in fileCol)
                        {
                            // load the file
                            srcContext.Load(f);
                            srcContext.ExecuteQuery();

                            string[] parts = null;
                            string id = null;

                            if (srcLibrary == "My Files")
                            {
                                // these are doc sets
                                parts = f.ServerRelativeUrl.Split('/');
                                id = parts[parts.Length - 2];
                            }
                            else
                            {
                                id = folder.Name;
                            }

                            // build new location url
                            string nLocation = destWeb.ServerRelativeUrl.TrimEnd('/') + "/" + destLibrary.Replace(" ", "") + "/" + id + "/" + f.Name;

                            // read the file, copy the content to new file at new location
                            FileInformation fileInfo = File.OpenBinaryDirect(srcContext, f.ServerRelativeUrl);
                            File.SaveBinaryDirect(destContext, nLocation, fileInfo.Stream, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("File Error = " + ex.ToString());
                }
            }
        }

        public void AddNewFolder(ClientContext clientContext, string listUrl, string listName, string docSetContentTypeName, string newDocSetName)
        {
                // get the web
                Web web = clientContext.Web;
                List list = clientContext.Web.Lists.GetByTitle(listName);
                clientContext.Load(clientContext.Site);
 
                // load the folder
                Folder f = web.GetFolderByServerRelativeUrl(listUrl + "/" + newDocSetName);
                clientContext.Load(f);
                bool alreadyExists = false;
 
                // check if the folder exists
                try
                {
                    clientContext.ExecuteQuery();
                    alreadyExists = true;
                }
                catch { }
 
                if (!alreadyExists)
                {
                    // folder doesn't exists so create it
                    ContentTypeCollection listContentTypes = list.ContentTypes;
                    clientContext.Load(listContentTypes, types => types.Include
                                                      (type => type.Id, type => type.Name,
                                                      type => type.Parent)) ;
                    
                    var result = clientContext.LoadQuery(listContentTypes.Where
                        (c => c.Name == docSetContentTypeName));
 
                    clientContext.ExecuteQuery();
 
                    ContentType targetDocumentSetContentType = result.FirstOrDefault();
 
                    // create the item
                    ListItemCreationInformation newItemInfo = new ListItemCreationInformation();
                    newItemInfo.UnderlyingObjectType = FileSystemObjectType.Folder;
                    newItemInfo.LeafName = newDocSetName;
                    ListItem newListItem = list.AddItem(newItemInfo);
 
                    // set title and content type
                    newListItem["ContentTypeId"] = targetDocumentSetContentType.Id.ToString();
                    newListItem["Title"] = newDocSetName;
                    newListItem.Update();
 
                    // execute it
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();
                }
            
        }
    }
}
