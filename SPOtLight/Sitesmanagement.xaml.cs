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
        //Constructor
        public Sitesmanagement(string Url)
        { 
            InitializeComponent();
            string AdmUrl = Url;
            ConnectSPOAdm(AdmUrl);
        }// End Constructor

        // Method - OnWindowInitialise() - Connect to SPO Site and retrive Basics Information // Copy from BtnConnect.click()
        private void ConnectSPOAdm(string Url)
        {
            //Using ClientContext - Retrive Basic Informaiton
            var spoL = new SPOLogic();
            using (PnPClientContext ctx = spoL.GetSiteContext(Url))
            {
                // Retrieving Tenant props
                Tenant tenant = new Tenant(ctx);
                var prop = tenant.GetSiteProperties(0, true);
                ctx.Load(prop);
                ctx.ExecuteQuery();

                // Iterating SubWebs to retrieve web.Url
                foreach (SiteProperties sp in prop)
                {
                    PnPClientContext context = new PnPClientContext(sp.Url);
                    context.Credentials = sp.Context.Credentials;
                    var web = context.Web;
                    context.Load(web, w => w.Url);
                    context.ExecuteQuery();

                    // Pushing Web.Url to LBSites
                    LBSites.Items.Add(web.Url);
                }
            }
        }// End Method

        // Method LBSites.OnChange() ==> Call for Site props (getSiteProps())
        private void LBSitesChanged(object sender, SelectionChangedEventArgs e)
        {
            getSiteProps(LBSites.SelectedValue.ToString());
        }// End Method

        // Method to Call for SharePoint Site Title and SiteUsers 
        private void getSiteProps(string Url)
        {
            var spoL = new SPOLogic();
            // Threading the call
            Task.Run(() =>
            {
                using (PnPClientContext ctx = spoL.GetSiteContext(Url))
                {
                    //Retrieving Web.Title and Web.SiteUsers
                    var web = ctx.Web;
                    ctx.Load(web, w => w.SiteUsers, w => w.Title);
                    ctx.ExecuteQuery();

                    // Threading push to TBOut.Text
                    TBOut.Dispatcher.Invoke(() =>
                    {
                        // Pushing SiteName, Admin count and Admin.Title to TBOut
                        TBOut.Text = "SiteName : " + ctx.Web.Title + Environment.NewLine;
                        TBOut.Text += "Admin count : " + ctx.Web.SiteUsers.Where(u => u.IsSiteAdmin).Count() + Environment.NewLine;

                        var admins = ctx.Web.SiteUsers.Where(u => u.IsSiteAdmin);
                        foreach (var admin in admins)
                        {
                            TBOut.Text += admin.Title + Environment.NewLine;
                        }
                    });// End Threading push to TBOut
                }
            }); // End Task
        }// End Method
    }
}
