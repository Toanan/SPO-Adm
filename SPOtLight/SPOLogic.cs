using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.Core;
using Microsoft.SharePoint.Client;

namespace SPOtLight
{
    class SPOLogic
    {
        // Method - Returns authenticated context
        public ClientContext GetSiteContext(string site)
        {
            // Creating ClientContext and passing Credentials from CredentialManagement
            ClientContext ctx = new ClientContext(site);
            ctx.Credentials = CredentialManager.GetSharePointOnlineCredential("SPOtLight");

            return ctx;
        }// End Method
    }
}
