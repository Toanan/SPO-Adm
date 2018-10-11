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
            this.CredManager = Url;
            Task.Factory.StartNew(() =>
            {
                this.SiteProp = getSiteProp(CredManager);
                ConnectSPOAdm(CredManager);
            });
        }// End Constructor

        public string CredManager { get; set; }



        public SPOSitePropertiesEnumerable SiteProp { get; set; }

        private SPOSitePropertiesEnumerable getSiteProp(string Url)
        {

            var spoL = new SPOLogic(CredManager);
            ClientContext ctx = spoL.GetSiteContext(Url);
            Tenant tenant = new Tenant(ctx);
            var prop = tenant.GetSitePropertiesFromSharePoint("0", true);
            ctx.Load(prop);
            ctx.ExecuteQuery();
            return prop;
        }

        // Method - OnWindowInitialise() - Connect to SPO Site and retrive Basics Information // Copy from BtnConnect.click()
        private void ConnectSPOAdm(string Url)
        {
            LBSites.Dispatcher.Invoke(() =>
            {
                LBSites.Items.Clear();
                foreach (var subweb in SiteProp)
                {                  
                        LBSites.Items.Add(subweb.Url);                   
                }
            });

            /*
            //Using ClientContext - Retrive Basic Informaiton
            var spoL = new SPOLogic(CredManager);
            Task.Run(() =>
            {
                // Using Clientcontext to avoid memory usage with no ctx.dispose()
                using (ClientContext ctx = spoL.GetSiteContext(Url))
                {
                    // Retrieving Tenant props
                    Tenant tenant = new Tenant(ctx);
                    var prop = tenant.GetSitePropertiesFromSharePoint("0", true);
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

                        LBSites.Dispatcher.Invoke(() =>
                        {
                            // Pushing Web.Url to LBSites
                            LBSites.Items.Add(web.Url);
                            ctx.Dispose();
                        });                     
                    }
                }
            });
            */
        }// End Method

        // Method LBSites.OnChange() ==> Call for Site props (getSiteProps())
        private void LBSitesChanged(object sender, SelectionChangedEventArgs e)
        {
            if(LBSites.SelectedValue != null)
            {
                //Reloading UI and [TODO] canceling pending operation to prevent spam
                getSiteProps(LBSites.SelectedValue.ToString());
                getSiteLists(LBSites.SelectedValue.ToString());
            }  
        }// End Method

        // Method to Call for SharePoint Site Props (Title and SiteUsers) onInitialise Window
        private void getSiteProps(string Url)
        {
            var spoL = new SPOLogic(CredManager);
            // Threading the call using System.Task
            var task = Task.Factory.StartNew(() =>
            {
                // Using Clientcontext to avoid memory usage with no ctx.dispose()
                using (ClientContext ctx = spoL.GetSiteContext(Url))
                {
                    //Retrieving Web.Title and Web.SiteUsers
                    var web = ctx.Web;
                    ctx.Load(web, w => w.SiteUsers, w => w.Title, w => w.WebTemplate, w => w.Configuration);
                    ctx.ExecuteQuery();

                    // Dispatch to TBOut control
                    TBOut.Dispatcher.Invoke(() =>
                    {
                        // Pushing SiteName, Admin count and Admin.Title to TBOut
                        TBOut.Text = "SiteName : " + ctx.Web.Title + Environment.NewLine;
                        TBOut.Text += "BaseTemplate : " + ctx.Web.WebTemplate + "#" + ctx.Web.Configuration.ToString() + Environment.NewLine;
                        TBOut.Text += "Admin count : " + ctx.Web.SiteUsers.Where(u => u.IsSiteAdmin).Count() + Environment.NewLine;

                        var admins = ctx.Web.SiteUsers.Where(u => u.IsSiteAdmin);
                        foreach (var admin in admins)
                        {
                            TBOut.Text += admin.Title + Environment.NewLine;
                        }
                    });// End Disptach to TBOut control
                }
            }); // End Task

        }// End Method

        // Method to Call for SharePoint Site Lists - onInitialise Window
        private void getSiteLists(string Url)
        {
            var spoL = new SPOLogic(CredManager);
            // Threading the call using System.Task
            Task.Run(() =>
            {
                
                // Using Clientcontext to avoid memory usage with no ctx.dispose()
                using (ClientContext ctx = spoL.GetSiteContext(Url))
                {
                    ListCollection lists = ctx.Web.Lists;
                    var listsQuery = from lst in lists
                                 where lst.Hidden != true
                                 select lst;
                    IEnumerable<Microsoft.SharePoint.Client.List> listcollection = ctx.LoadQuery(listsQuery);
                    ctx.ExecuteQuery();

                    // Threading push to TBOut.Text
                    LBLists.Dispatcher.Invoke(() =>
                    {
                        LBLists.Items.Clear();
                        foreach (Microsoft.SharePoint.Client.List lst in listcollection)
                        {
                            LBLists.Items.Add(lst.Title + " - " + lst.ItemCount + " ListItem");
                        }
                    });
                }
            }); // End Task
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

        private void RefreshSites(object sender, RoutedEventArgs e)
        {
            getSiteProp(CredManager);
            ConnectSPOAdm(CredManager);
            TBOut.Clear();
            LBLists.Items.Clear();
        }
    }
}
