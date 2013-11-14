using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sharezbold.ContentMigration
{
    public class TEST
    {
        public TEST()
        {
            //http://ss13-css-009:23838/testtt/Lists/MyJobApplications/_vti_bin/Views.asmx

            string url = @"http://sps2013003";
            //string url = @"http://ss13-css-009:23838";
            //string url = @"http://ss13-css-009:23838/testtt/Lists/MyJobApplications";
            string urlViews = url + @"/_vti_bin/Views.asmx";
            string urlWebs = url + @"/_vti_bin/Webs.asmx";

            var SrcCredentials = new CredentialCache();
            SrcCredentials.Add(new Uri(url), "NTLM", new NetworkCredential("Administrator", "P@ssw0rd", "CSSDEV"));


            var DstWebs = new Sharezbold.ContentMigration.WebsWS.Webs();
            DstWebs.Url = urlWebs;
            DstWebs.Credentials = SrcCredentials;


            var DstViews = new Sharezbold.ContentMigration.ViewsWS.Views();
            DstViews.Url = urlViews;
            DstViews.Credentials = SrcCredentials;

            try
            {
                DstWebs.GetWebCollection();
                DstViews.GetViewCollection("MyJobApplications");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
