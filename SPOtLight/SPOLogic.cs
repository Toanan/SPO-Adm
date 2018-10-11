using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.Core;
using Microsoft.SharePoint.Client;

namespace SPOtLight
{
    class SPOLogic
    {
        //Constructor
        public SPOLogic(string CredManager)
        {
            this.CredManager = CredManager;
            
        }// End Constructor

        public string CredManager { get; set; }

        // Method - Returns authenticated context
        public ClientContext GetSiteContext(string site)
        {
            // Creating ClientContext and passing Credentials from CredentialManagement
            ClientContext ctx = new ClientContext(site);
            ctx.Credentials = CredentialManager.GetSharePointOnlineCredential(CredManager);

            return ctx;
        }// End Method
    }
}
