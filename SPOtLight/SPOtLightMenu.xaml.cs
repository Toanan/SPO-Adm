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
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Utilities;
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
            using (ClientContext ctx = GetContext(TBSite.Text))
            {
                // Calling to Web.Title, Lists and Admins
                ctx.Load(ctx.Web, w => w.Title, w => w.Lists, w => w.AssociatedOwnerGroup.Users);
                ctx.ExecuteQuery();

                // Showing results to TBOut - Title
                TBOut.Text = ctx.Web.Title + Environment.NewLine;

                // Showing results to TBOut - Lists Count
                TBOut.Text += ctx.Web.Lists.Count().ToString();

                // Showing results to TBOut - List Title
                foreach (var list in ctx.Web.Lists)
                {
                    TBOut.Text += list.Title + Environment.NewLine;
                }

                // Showing results to TBOut - Admins Count
                var admin = ctx.Web.AssociatedOwnerGroup.Users;
                TBOut.Text += string.Format("Nombre d'adm : {0}", admin.Count());

                // Showing results to TBOut - Admin Title
                foreach (var adm in admin)
                {
                    TBOut.Text += adm.Title + Environment.NewLine;
                }
            }
        }// End Method

        // Method - Returns authenticated context
        public ClientContext GetContext(string site)
        {
            
            // Creating ClientContext and passing Credentials from CredentialManagement
            ClientContext ctx = new ClientContext(site);
            ctx.Credentials = CredentialManager.GetSharePointOnlineCredential("SPOtLight");
            return ctx;       
        }// End Method

        // Method - BTN.Click - Create List
        private void CreateList(object sender, RoutedEventArgs e)
        {
            using (ClientContext ctx = GetContext(TBSite.Text))
            {
                ctx.Web.CreateList(ListTemplateType.DocumentLibrary, TBList.Text, false);
            }
            MessageBox.Show(string.Format("La liste : {0} à été créé dans le site {1}", TBList.Text, TBSite.Text));

        }
    }
}
