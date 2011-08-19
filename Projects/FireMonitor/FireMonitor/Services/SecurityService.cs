using FireMonitor.Services;
using Infrastructure;
using FiresecClient;

namespace FireMonitor
{
    public class SecurityService : ISecurityService
    {
        public bool Check()
        {
            var passwordView = new PasswordView();
            passwordView.ShowDialog();
            return passwordView.IsAutorised;
        }

        public void ChangeUser()
        {
            var passwordView = new PasswordView();
            passwordView.InitializeReconnect();
            passwordView.ShowDialog();
            FiresecManager.Reconnect(passwordView.UserName, "");
        }
    }
}