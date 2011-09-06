using FiresecAPI.Models;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class GroupViewModel : BaseViewModel
    {
        public GroupViewModel(UserGroup group)
        {
            Group = group;
        }

        UserGroup _group;
        public UserGroup Group
        {
            get { return _group; }
            set
            {
                _group = value;
                OnPropertyChanged("Group");
            }
        }
    }
}