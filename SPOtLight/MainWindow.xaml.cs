using System.Windows;

namespace SPOtLight
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Constructor
        public MainWindow()
        {
            InitializeComponent();
        }// End Constructor

        // Method - Store Credentials
        private void StorePW(object sender, RoutedEventArgs e)
        {
            // Tenant = null => return
            if (string.IsNullOrEmpty(Tenant.Text))
            {
                MessageBox.Show("Please provide a Tenant", "My app", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // We create the PasswordRepository
            var repo = new PasswordRepository();
            // Build the adminUrl from userInput
            string adminUrl = "https://" + Tenant.Text + "-admin.sharepoint.com";

            // If Login Sceanrio => Login
            if (string.IsNullOrEmpty(TBUN.Text))
            {
                // Get the credentials from Credential Manager
                CredentialManagement.Credential cred = repo.GetCred(adminUrl);
                if (cred.Exists())
                {
                    this.Hide();
                    new Sitesmanagement(adminUrl).Show();
                    return;
                }
                else
                {
                    MessageBox.Show("Please Register this Tenant");
                    return;
                }
            }

            // If Register Screnario => Register & Login
            if ((!string.IsNullOrEmpty(TBUN.Text)) && (PBPW.SecurePassword != null))
            {
                try
                {
                    repo.SaveCred(PBPW.SecurePassword, TBUN.Text, adminUrl);
                    this.Hide();
                    new Sitesmanagement(adminUrl).Show();
                    return;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, "My app", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            //else we ask for UserName and PassWord
            MessageBox.Show("Please provide a UserName and PassWord");
               
        }// End Method

        private void TBUN_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
