using FireMonitor.Services;
using Infrastructure;

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
    }
}