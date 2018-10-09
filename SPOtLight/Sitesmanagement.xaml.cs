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

        string mainpath = "https://toanan-admin.sharepoint.com/";

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
                    context.Load(web, w => w.SiteUsers, w => w.Title, w => w.Url);
                    context.ExecuteQuery();

                    var site = string.Format("{0} Url: {1}", web.Title, web.Url);
                    TBOut.Text += site + Environment.NewLine;

                    int admincount = web.SiteUsers.Where(u => u.IsSiteAdmin).Count();
                    if (admincount < 2)
                    {
                        TBOut.Text += string.Format("Le site n'est pas en compliance : {0} admin", admincount) + Environment.NewLine;
                    }
                    var admins = string.Join(";",web.SiteUsers.Where(u => u.IsSiteAdmin).Select(a => a.Title).ToList());

                    TBOut.Text += sp.Title + " => " + sp.Url + Environment.NewLine;
                    TBOut.Text += "Administrators  => " + admins + Environment.NewLine;
                    TBOut.Text += "---------------------------" + Environment.NewLine;
                }
            }
        }// End Method


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

                    var admins = string.Join(";", web.SiteUsers.Where(u => u.IsSiteAdmin).Select(a => a.Title).ToList());
                    var users = string.Join(";", web.SiteUsers.Where(u => !u.IsSiteAdmin).Select(a => a.Title).ToList());

                    TBOut.Text += sp.Title + " => " + sp.Url;
                    TBOut.Text += "Administrators  => " + admins;
                    TBOut.Text += "Users  =>  " + users;
                    TBOut.Text += "---------------------------";
                }
            }
        }// End Method
    }
}
