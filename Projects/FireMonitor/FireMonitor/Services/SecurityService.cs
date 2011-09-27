using Infrastructure;

namespace FireMonitor
{
    public class SecurityService : ISecurityService
    {
        public bool Connect()
        {
            return Connect(LoginView.PasswordViewType.Connect);
        }

        public bool ReConnect()
        {
            return Connect(LoginView.PasswordViewType.Reconnect);
        }

        public bool Validate()
        {
            return Connect(LoginView.PasswordViewType.Validate);
        }

        bool Connect(LoginView.PasswordViewType passwordViewType)
        {
            var loginView = new LoginView();
            loginView.Initialize(passwordViewType);
            loginView.ShowDialog();
            return loginView.IsLoggedIn;
        }
    }
}