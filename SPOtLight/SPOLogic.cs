using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.Core;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Online.SharePoint.TenantAdministration;

namespace SPOtLight
{
    class SPOLogic
    {
        //Constructor
        public SPOLogic(string CredManager)
        {
            this.CredManager = CredManager;
            
        }// End Constructor

        //Prop AdmSite Url
        public string CredManager { get; set; }

        // Method - Returns tenantSiteProps
        public SPOSitePropertiesEnumerable getTenantProp(string Url)
        {
            ClientContext ctx = GetSiteContext(Url);
            Tenant tenant = new Tenant(ctx);
            SPOSitePropertiesEnumerable prop = tenant.GetSitePropertiesFromSharePoint("0", true);
            ctx.Load(prop);
            ctx.ExecuteQuery();
            return prop;
        }

        // Method - Returns authenticated context
        public ClientContext GetSiteContext(string Url)
        {
            // Creating ClientContext and passing Credentials from CredentialManagement
            ClientContext ctx = new ClientContext(Url);
            ctx.Credentials = CredentialManager.GetSharePointOnlineCredential(CredManager);

            return ctx;
        }// End Method

        // Method - Returns webProps
        public Web getWebProps(string Url, string CredName)
        {
            // Creating ClientContext and passing Credentials from CredentialManagement
            using (ClientContext ctx = new ClientContext(Url))
            {
                ctx.Credentials = CredentialManager.GetSharePointOnlineCredential(CredName);
                //Retrieving Web.Title and Web.SiteUsers
                var web = ctx.Web;
                ctx.Load(web, w => w.SiteUsers, w => w.Title, w => w.WebTemplate, w => w.Configuration);
                ctx.ExecuteQuery();
                return web;
            }

            
        }// End Method

        // Method - Returns web.Lists
        public IEnumerable<List> getWebLists(string Url, string CredName)
        {
            // Using Clientcontext to avoid memory usage with no ctx.dispose()
            using (ClientContext ctx = new ClientContext(Url))
            {
                ctx.Credentials = CredentialManager.GetSharePointOnlineCredential(CredName);

                ListCollection lists = ctx.Web.Lists;

                ctx.Load(ctx.Web.Lists);
                ctx.ExecuteQuery();

                return ctx.Web.Lists;
            }
        }// End Method

    }
        
}
