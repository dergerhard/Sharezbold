
namespace Sharezbold.FileMigration
{
    using System;
    using Microsoft.SharePoint.Client;

    internal class Validator
    {
        internal static void ValidateIfWebExists(ClientContext clientContext, Web web)
        {
            try
            {
                clientContext.Load(web);
                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                throw new ValidationException("Can't connect to the web '" + web.Title + "'.");
            }
        }
    }
}
