using System;
using System.Linq;
using System.Windows;
using OfficeDevPnP.Core;
using Microsoft.SharePoint.Client;

namespace SPOtLight
{
    /// <summary>
    /// Logique d'interaction pour SPOtLightMenu.xaml
    /// </summary>
    public partial class SPOtLightMenu : Window
    {
        public SPOtLightMenu()
        {
            InitializeComponent();
        }

        // Method - Btn.Click - Connect to SPO Site and retrive Basics Information
        private void ConnectSPO(object sender, RoutedEventArgs e)
        {
            //Using ClientContext - Retrive Basic Informaiton
            var spoL = new SPOLogic();
            using (PnPClientContext ctx = spoL.GetSiteContext(TBSite.Text))
            {
                // Calling to Web.Title, Lists and Admins
                ctx.Load(ctx.Web, w => w.Title, w => w.Lists, w => w.AssociatedOwnerGroup.Users);
                ctx.ExecuteQueryRetry();

                // Showing results to TBOut - Title
                TBOut.Text = "Site Name : " + ctx.Web.Title + Environment.NewLine;

                // Showing results to TBOut - Admins Count
                var admin = ctx.Web.AssociatedOwnerGroup.Users;
                TBOut.Text += string.Format("Amount of Admin : {0}", admin.Count() + Environment.NewLine);

                // Showing results to TBOut - Admin Title
                foreach (var adm in admin)
                {
                    TBOut.Text += adm.Title + Environment.NewLine;
                }

                // Showing results to TBOut - Lists Count
                TBOut.Text += "Amount of lists : " + ctx.Web.Lists.Count().ToString() + Environment.NewLine;

                // Showing results to TBOut - List Title
                foreach (var list in ctx.Web.Lists)
                {
                    TBOut.Text += list.Title + Environment.NewLine;
                }
            }
        }// End Method

        // Method - BTN.Click - Create List
        private void CreateList(object sender, RoutedEventArgs e)
        {
            //Using ClientContext - Retrive Basic Informaiton
            var spoL = new SPOLogic();
            using (PnPClientContext ctx = spoL.GetSiteContext(TBSite.Text))
            {
                try
                {
                    //Attempt to create the list
                    ctx.Web.CreateList(ListTemplateType.DocumentLibrary, TBList.Text, false);
                    MessageBox.Show(string.Format("List : {0} has been created in SPOSite : {1}", TBList.Text, TBSite.Text));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Unable to create list : {0}" + Environment.NewLine + "{1}",TBList.Text, ex.Message));
                }
            }
        }// End Method
    }
}
