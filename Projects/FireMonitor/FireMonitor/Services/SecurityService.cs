using FireMonitor.ViewModels;
using Infrastructure;

namespace FireMonitor
{
    public class SecurityService : ISecurityService
    {
        public bool Validate()
        {
            return ServiceFactory.LoginService.ExecuteValidate();
        }
    }
}