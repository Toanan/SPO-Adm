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

        
        #region Init
        //Constructor
        public Sitesmanagement(string Url)
        {
            InitializeComponent();
            CBSiteTemplate.SelectedIndex = 0;
            // Set UrlProps
            this.CredManager = Url;
            
        }// End Constructor

        // Method - Window.Loaded() - Set TenantProps & Show Sites to Treeview
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SPOLogic sp = new SPOLogic(CredManager);
            // Task - SetTenantProps

            this.TenantProp = sp.getTenantProp(CredManager);
            if (this.TenantProp == null)
            {
                new MainWindow().Show();
                this.Hide();
            }
            else
            {
                ShowSites();
            }
        }// End Method
        #endregion

        #region Props
        // Props - UrlProps
        public string CredManager { get; set; }

        // Props - TenantProps
        public SPOSitePropertiesEnumerable TenantProp { get; set; }
        #endregion

        #region TreeView Sites and Lists
        // Method to Call for SharePoint Site Lists - onInitialise Window
        public void GetSiteLists(string Url)
        {
            var spoL = new SPOLogic(Url);
            IEnumerable<Microsoft.SharePoint.Client.List> lists = spoL.getWebLists(Url, CredManager).Where(l => !l.Hidden);

            //LBLists.Items.Clear();
            foreach (Microsoft.SharePoint.Client.List lst in lists)
            {
                //LBLists.Items.Add(lst.Title + " - (" + lst.ItemCount + ")");
            }
        }// End Method

        // Method - OnLoad() - Add Sites to TreeView - Launch as a Task
        private void ShowSites()
        {
            // Start Dispatcher
            SiteView.Dispatcher.Invoke(() =>
            {
                // Clear TreeViewItems
                SiteView.Items.Clear();
                // Get All sites in TenantProp
                foreach (var subweb in TenantProp)
                {
                    // Creating the TreeViewItem + props
                    var item = new TreeViewItem
                    {
                        // Set the header
                        Header = subweb.Url,
                        // Set the full path
                        Tag = subweb.Url,
                    };
                    // Adding dumy item.items
                    item.Items.Add(null);

                    // Listen out for item being expanded
                    item.Expanded += Folder_Expanded;

                    // Add it to the TreeView
                    SiteView.Items.Add(item);                   
                }
            });
        }// End Method

        // Method - TreeViewItem.Expand Listener - Call for Site Lists
        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;

            // If the item only contains the dumy data
            if (item.Items.Count != 1 || item.Items == null)
                return;
            //Clear dummy item
            item.Items.Clear();

            // Get Site library
            var SitePath = (string)item.Tag;

            Task.Factory.StartNew(() =>
            {
                // Call for the expended site Web
                var spoL = new SPOLogic(CredManager);
                // Filter on not hidden file
                IEnumerable<Microsoft.SharePoint.Client.List> lists = spoL.getWebLists(SitePath, CredManager).Where(l => !l.Hidden);

                item.Dispatcher.Invoke(() =>
                {
                    // Creating TreeeViewIems from lists
                    foreach (var list in lists)
                    {
                        var subitem = new TreeViewItem
                        {
                            Header = list.Title + " (" + list.ItemCount + ") - " + list.BaseTemplate.ToString() ,
                            Tag = list.BaseTemplate.ToString(),
                        };

                        item.Items.Add(subitem);
                    }
                });// End Dispatch
            });// End Task        
        }// End Method

        // Method - Refresh.onClick() - Call for showsite
        private void RefreshSites(object sender, RoutedEventArgs e)
        {
            SPOLogic sp = new SPOLogic(CredManager);
            // Task - SetTenantProps and show sites
            Task.Factory.StartNew(() =>
            {
                this.TenantProp = sp.getTenantProp(CredManager);
                ShowSites();
            });

            //Clear Ui
            TBOut.Content = "";
            SiteView.Items.Clear();
        }// End Method
        #endregion

        #region Site Creation
        //Method to create a Modern Project / Communication Site
        private async void createSite(string SiteTemplate)
        {
            if (string.IsNullOrEmpty(TBSiteName.Text))
            {
                MessageBox.Show("Please give a site name", "My app" ,MessageBoxButton.OK ,MessageBoxImage.Warning);
                return;
            }
            
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
        #endregion


        // Method to Call for SharePoint Site Props (Title and SiteUsers) - Task()
        private void GetSiteProps(string Url)
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

        public static string GetTag(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            return path;
        }

        private void LoginPage_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Hide();
        }
    }
}
