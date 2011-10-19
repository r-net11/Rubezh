using FiresecAPI.Models;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class GuardUserViewModel : BaseViewModel
    {
        public GuardUserViewModel(GuardUser guardUser)
        {
            GuardUser = guardUser;
        }

        GuardUser _guardUser;
        public GuardUser GuardUser
        {
            get { return _guardUser; }
            set
            {
                _guardUser = value;
                OnPropertyChanged("GuardUser");
            }
        }
    }
}
