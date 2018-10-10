using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System.Threading;

namespace SPOtLight
{
    /// <summary>
    /// Logique d'interaction pour Sitesmanagement.xaml
    /// </summary>
    public partial class Sitesmanagement : Window
    {
        public Sitesmanagement()
        {
            InitializeComponent();
        }

        string mainpath = "https://toanan-admin.sharepoint.com";

        // Method - Btn.Click - Connect to SPO Site and retrive Basics Information
        private void ConnectSPOAdm(object sender, RoutedEventArgs e)
        {
            //Using ClientContext - Retrive Basic Informaiton
            var spoL = new SPOLogic();
            using (PnPClientContext ctx = spoL.GetSiteContext(mainpath))
            {
                Tenant tenant = new Tenant(ctx);
                var prop = tenant.GetSiteProperties(0, true);
                ctx.Load(prop);
                ctx.ExecuteQuery();

                foreach (SiteProperties sp in prop)
                {
                    PnPClientContext context = new PnPClientContext(sp.Url);
                    context.Credentials = sp.Context.Credentials;
                    var web = context.Web;
                    context.Load(web, w => w.Url);
                    context.ExecuteQuery();

                    LBSites.Items.Add(web.Url);
                }
            }
        }// End Method

        private void getSiteProps(string Url)
        {  
            var spoL = new SPOLogic();
            Task.Run(() =>
            {
                using (PnPClientContext ctx = spoL.GetSiteContext(Url))
                {
                    var web = ctx.Web;
                    ctx.Load(web, w => w.SiteUsers, w => w.Title, w => w.Url);
                    ctx.ExecuteQuery();

                    TBOut.Dispatcher.Invoke(() =>
                    {
                        TBOut.Text = "SiteName : " + ctx.Web.Title + Environment.NewLine;
                        TBOut.Text += "Admin count : " + ctx.Web.SiteUsers.Where(u => u.IsSiteAdmin).Count() + Environment.NewLine;

                        var admins = ctx.Web.SiteUsers.Where(u => u.IsSiteAdmin);
                        foreach (var admin in admins)
                        {
                            TBOut.Text += admin.Title + Environment.NewLine;
                        }
                    });
                }
            }); 
        }

        private void getSubWebs(string path)
        {
                string mainpath = "https://toanan-admin.sharepoint.com/";
                //Using ClientContext - Retrive Basic Informaiton
                var spoL = new SPOLogic();
            using (PnPClientContext ctx = spoL.GetSiteContext(mainpath))
            {

                Tenant tenant = new Tenant(ctx);
                var prop = tenant.GetSiteProperties(0, true);
                ctx.Load(prop);
                ctx.ExecuteQuery();
                foreach (SiteProperties sp in prop)
                {
                    PnPClientContext context = new PnPClientContext(sp.Url);
                    context.Credentials = sp.Context.Credentials;
                    var web = context.Web;
                    context.Load(web, w => w.SiteUsers);
                    context.ExecuteQuery();
                }
            }
        }// End Method

        private void LBSitesChanged(object sender, SelectionChangedEventArgs e)
        {
            getSiteProps(LBSites.SelectedValue.ToString());
        }
    }
}
