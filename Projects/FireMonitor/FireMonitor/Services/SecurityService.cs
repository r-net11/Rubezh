using FireMonitor.Services;
using Infrastructure;
using FiresecClient;

namespace FireMonitor
{
    public class SecurityService : ISecurityService
    {
        public bool Check()
        {
            PasswordView passwordView = new PasswordView();
            passwordView.ShowDialog();
            return passwordView.IsAutorised;
        }

        public void ChangeUser()
        {
            PasswordView passwordView = new PasswordView();
            passwordView.InitializeReconnect();
            passwordView.ShowDialog();
            FiresecManager.Reconnect(passwordView.UserName);
        }
    }
}