using System.Windows;

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
                repo.SaveCred(PBPW.SecurePassword, TBUN.Text, TBAdminSite.Text);
                this.Hide();
                
            }
            CredentialManagement.Credential cred = repo.GetCred(TBAdminSite.Text);
            if (cred.Exists())
            {
                this.Hide();
                new Sitesmanagement(TBAdminSite.Text).Show();
            }
        }
    }
}
