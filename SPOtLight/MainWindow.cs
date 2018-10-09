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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SPOtLight;
using Microsoft.SharePoint.Client;

namespace SPOtLight
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Mainwindows()
        {
            InitializeComponent();
        }

        private void StorePW(object sender, RoutedEventArgs e)
        {
            var repo = new PasswordRepository();
            var tryConnect = new SPOtLightMenu();

            if (!string.IsNullOrEmpty(TBUN.Text))
            {  
                repo.SaveCred(PBPW.SecurePassword, TBUN.Text);
                this.Hide();
            }
            CredentialManagement.Credential cred = repo.GetCred();
            if (cred.Exists())
            {
                this.Hide();
                new Sitesmanagement().Show();
            }
        }
    }
}
