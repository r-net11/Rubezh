using FiresecAPI.Models;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class PermissionViewModel : BaseViewModel
    {
        public PermissionViewModel(PermissionType permissionType)
        {
            PermissionType = permissionType;
        }

        public PermissionType PermissionType { get; private set; }

        bool _isEnable;
        public bool IsEnable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                OnPropertyChanged("IsEnable");
            }
        }
    }
}