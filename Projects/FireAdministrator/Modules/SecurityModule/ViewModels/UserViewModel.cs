using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        public UserViewModel(User user)
        {
            User = user;
        }

        User _user;
        public User User
        {
            get { return _user; }
            set
            {
                _user = value;

                RoleName = string.Empty;
                if (value != null)
                {
                    var role = FiresecManager.SecurityConfiguration.UserRoles.FirstOrDefault(x => x.Id == value.RoleId);
                    if (role != null)
                        RoleName = role.Name;
                }

                OnPropertyChanged("User");
                OnPropertyChanged("RoleName");
            }
        }

        public string RoleName { get; private set; }
    }
}