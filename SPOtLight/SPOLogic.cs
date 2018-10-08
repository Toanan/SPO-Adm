using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.Core;

namespace SPOtLight
{
    class SPOLogic
    {
        // Method - Returns authenticated context
        public ClientContext GetContext(string site)
        {

            // Creating ClientContext and passing Credentials from CredentialManagement
            ClientContext ctx = new ClientContext(site);
            ctx.Credentials = CredentialManager.GetSharePointOnlineCredential("SPOtLight");
            return ctx;
        }// End Method
    }
}
