//-----------------------------------------------------------------------
// <copyright file="Application.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------
/*
using Microsoft.SharePoint.Client;

namespace Sharezbold.FileMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    public class Application
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Start File-Migration");
            ClientContext source = new ClientContext("http://sps2010003");
            ClientContext target = new ClientContext("http://sps2013003");
            NetworkCredential credentialsSource = new NetworkCredential("Administrator", "P@ssw0rd", "CSSSPS2013003");
            NetworkCredential credentialsTargat = new NetworkCredential("Administrator", "P@ssw0rd", "CSSSPS2010003");

            source.Credentials = credentialsSource;
            target.Credentials = credentialsTargat;

            Web web = source.Web;
            ListCollection lists = web.Lists;

            source.Load(web);
            source.Load(lists);
            source.ExecuteQuery();
            target.Load(target.Web);
            target.ExecuteQuery();

            Console.WriteLine("Sharepoint 2010 version = {0}", target.ServerVersion.Major);
            Console.WriteLine("Sharepoint 2013 version = {0}", source.ServerVersion.Major);

            SharePoint2010And2013Migrator migrator = FileMigrationBuilder.GetNewFileMigrationBuilder().WithServiceAddress(new Uri("http://sps2013003:12345/FileMigrationService")).WithSourceClientContext(source).WithTargetClientContext(target).CreateMigrator();
            migrator.MigrateFilesOfWeb(source.Web, target.Web);
            
            Console.WriteLine("finished program");
            Console.ReadKey();
        }
    }
}
*/