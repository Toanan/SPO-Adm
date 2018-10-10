using System.Windows;

namespace SPOtLight
{
    public class PasswordRepository
    {

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
        }

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


        }
    }
}
