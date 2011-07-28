using Infrastructure.Common;
using FiresecAPI.Models;

namespace SecurityModule.ViewModels
{
    public class GroupViewModel : BaseViewModel
    {
        public GroupViewModel(UserGroup group)
        {
            Name = group.Name;
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
    }
}
