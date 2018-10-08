using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SPOtLight
{
    public class PasswordRepository
    {
        private const string PasswordName = "SPOtLight";

        public void SaveCred(System.Security.SecureString password, string userName)
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
