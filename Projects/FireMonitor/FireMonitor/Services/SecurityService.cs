using FireMonitor.ViewModels;
using Infrastructure;

namespace FireMonitor
{
    public class SecurityService : ISecurityService
    {
        public bool Validate()
        {
            var loginViewModel = new LoginViewModel(LoginViewModel.PasswordViewType.Validate);
            return ServiceFactory.UserDialogs.ShowModalWindow(loginViewModel);
        }
    }
}