using System.Windows;

namespace SPOtLight
{
    public class PasswordRepository
    {
        // Method - SaveCredentials to CredentialManager
        public void SaveCred(System.Security.SecureString password, string userName, string AdmUrl)
        {
            using (var cred = new CredentialManagement.Credential())
            {
                cred.SecurePassword = password;
                cred.Username = userName;
                cred.Target = AdmUrl;
                cred.Type = CredentialManagement.CredentialType.Generic;
                cred.PersistanceType = CredentialManagement.PersistanceType.LocalComputer;
                cred.Save();
            }
        }// End Method

        // Method - GetCredentials from CredentialManager
        public CredentialManagement.Credential GetCred(string AdmUrl)
        {
            var cred = new CredentialManagement.Credential();
            cred.Target = AdmUrl;
            if (!cred.Exists())
            {
                MessageBox.Show(string.Format("Unable to find credential : {0}, please set up credentials", AdmUrl));
            }
            cred.Load();
            return cred;


        }// End Method
    }
}
