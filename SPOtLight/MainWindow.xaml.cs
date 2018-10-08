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
        public MainWindow()
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
                new SPOtLightMenu().Show();
            }
        }
    }

    public class PasswordRepository
    {
        private const string PasswordName = "SPOtLight";

        public void  SaveCred (System.Security.SecureString password, string userName)
        {
            using (var cred = new CredentialManagement.Credential())
            {
                cred.SecurePassword = password;
                cred.Username = userName;
                cred.Target = PasswordName;
                cred.Type = CredentialManagement.CredentialType.Generic;
                cred.PersistanceType = CredentialManagement.PersistanceType.LocalComputer;
                cred.Save();
            }
        }

        public CredentialManagement.Credential GetCred()
        {
            var cred = new CredentialManagement.Credential();
            cred.Target = PasswordName;
            if (!cred.Exists())
            {
                MessageBox.Show(string.Format("Unable to find credential : {0}, please set up credentials", PasswordName));
            }
            cred.Load();
            return cred;


        }
    }
}
