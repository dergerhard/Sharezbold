namespace Sharezbold.Util
{
    using System.Net;

    /// <summary>
    /// Data for the sharepoint-information.
    /// </summary>
    public class SharepointInformation
    {
        public string Address{ get; set; }
        public NetworkCredential Credentials { get; set; }
    }
}
