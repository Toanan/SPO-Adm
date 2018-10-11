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
using OfficeDevPnP.Core.Sites;
using Microsoft.SharePoint.Client;
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
            CBSiteTemplate.SelectedIndex = 0;
            // Set UrlProps
            this.CredManager = Url;
            SPOLogic sp = new SPOLogic(Url);
            // Task - SetTenantProps
            Task.Factory.StartNew(() =>
            {
                this.TenantProp = sp.getTenantProp(Url);
                showSites(Url);
            });
        }// End Constructor

        // Props - UrlProps
        public string CredManager { get; set; }

        // Props - TenantProps
        public SPOSitePropertiesEnumerable TenantProp { get; set; }

        // Method - OnWindowInitialise() - Connect to SPO Site and retrive Basics Information // oninit()
        private void showSites(string Url)
        {
            LBSites.Dispatcher.Invoke(() =>
            {
                LBSites.Items.Clear();
                foreach (var subweb in TenantProp)
                {                  
                        LBSites.Items.Add(subweb.Url);                   
                }
            });
        }// End Method

        // Method LBSites.OnChange() ==> Call for Site props and Lists props (getSiteProps() + getSiteLists())
        private void LBSitesChanged(object sender, SelectionChangedEventArgs e)
        {
            if(LBSites.SelectedValue != null)
            {
                //Reloading UI and [TODO] canceling pending operation to prevent spam
                Task.Factory.StartNew(() =>
                {
                    // Dispatch to TBOut control
                    TBOut.Dispatcher.Invoke(() =>
                    {
                        getSiteProps(LBSites.SelectedValue.ToString());
                    });// End Dispatch
                });// End Task
                Task.Factory.StartNew(() =>
                {
                    // Dispatch to TBOut control
                    LBLists.Dispatcher.Invoke(() =>
                    {
                        getSiteLists(LBSites.SelectedValue.ToString());
                    });// End Dispatch
                });// End Task
                //getSiteLists(LBSites.SelectedValue.ToString());
            }
        }// End Method

        // Method to Call for SharePoint Site Props (Title and SiteUsers) - Task()
        private void getSiteProps(string Url)
        {  
                    var spoL = new SPOLogic(Url);
                    Web web = spoL.getWebProps(Url, CredManager);

                    // Pushing SiteName, Admin count and Admin.Title to TBOut
                    TBOut.Content = "SiteName : " + web.Title + Environment.NewLine;
                    TBOut.Content += "BaseTemplate : " + web.WebTemplate + "#" + web.Configuration.ToString() + Environment.NewLine;
                    TBOut.Content += "Admin count : "+ web.SiteUsers.Where(u => u.IsSiteAdmin).Count() + Environment.NewLine;

                    var admins = web.SiteUsers.Where(u => u.IsSiteAdmin);
                    foreach (var admin in admins)
                    {
                        TBOut.Content += admin.Title + Environment.NewLine;
                    }
        }// End Method

        // Method to Call for SharePoint Site Lists - onInitialise Window
        private void getSiteLists(string Url)
        {
            var spoL = new SPOLogic(Url);
            IEnumerable<Microsoft.SharePoint.Client.List> lists = spoL.getWebLists(Url ,CredManager).Where(l => !l.Hidden);

            LBLists.Items.Clear();
            foreach (Microsoft.SharePoint.Client.List lst in lists)
            {
                LBLists.Items.Add(lst.Title + " - (" + lst.ItemCount + ")");
            }
        }// End Method

        //Method to create a Modern Project / Communication Site
        private async void createSite (string SiteTemplate)
        {
            var spoL = new SPOLogic(CredManager);
            ClientContext ctx = spoL.GetSiteContext(CredManager);
            if (SiteTemplate == "Team")
            {
                var sitecontext = await ctx.CreateSiteAsync(new TeamSiteCollectionCreationInformation
                {
                    Description = "",
                    DisplayName = TBSiteName.Text,
                    Alias = TBSiteName.Text,
                    IsPublic = true,
                    //Classification="IT"   
                });
            }
            else
            {
                var communicationContext = await ctx.CreateSiteAsync(new CommunicationSiteCollectionCreationInformation
                {
                    Title = TBSiteName.Text, // Mandatory
                    Description = "", // Mandatory
                    Lcid = 1033, // Mandatory
                    //AllowFileSharingForGuestUsers = false, // Optional
                    //Classification = "classification", // Optional
                    SiteDesign = CommunicationSiteDesign.Topic, // Mandatory
                    Url = "https://toanan.sharepoint.com/sites/" + TBSiteName.Text, // Mandatory
                });
            }
            ctx.Dispose();
        }// End Method

        // Method - AddSite.onClick() - Call for createSite()
        private void AddSite_onClick(object sender, RoutedEventArgs e)
        {
            createSite(CBSiteTemplate.SelectedValue.ToString());
        }// End Method

        // Method - Refresh.onClick() - Call for showsite
        private void RefreshSites(object sender, RoutedEventArgs e)
        {
            SPOLogic sp = new SPOLogic(CredManager);
            // Task - SetTenantProps and show sites
            Task.Factory.StartNew(() =>
            {
                this.TenantProp = sp.getTenantProp(CredManager);
                showSites(CredManager); 
            });

            //Clear Ui
            TBOut.Content = "";
            LBLists.Items.Clear();
        }// End Method
    }
}
