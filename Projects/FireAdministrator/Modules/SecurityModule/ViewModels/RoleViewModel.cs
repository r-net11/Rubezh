using FiresecAPI.Models;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class RoleViewModel : BaseViewModel
    {
        public RoleViewModel(UserRole role)
        {
            Role = role;
        }

        UserRole _role;
        public UserRole Role
        {
            get { return _role; }
            set
            {
                _role = value;
                OnPropertyChanged("Role");
            }
        }
    }
}