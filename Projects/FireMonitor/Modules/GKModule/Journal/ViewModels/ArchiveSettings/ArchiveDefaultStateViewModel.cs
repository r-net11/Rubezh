using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Models;

namespace GKModule.ViewModels
{
    public class ArchiveDefaultStateViewModel : BaseViewModel
    {
        public ArchiveDefaultStateViewModel(ArchiveDefaultStateType archiveDefaultStateType)
        {
            ArchiveDefaultStateType = archiveDefaultStateType;
        }

        public ArchiveDefaultStateType ArchiveDefaultStateType { get; private set; }

        bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;

                if (_isActive)
                    ServiceFactory.Events.GetEvent<ArchiveDefaultStateCheckedEvent>().Publish(this);

                OnPropertyChanged("IsActive");
            }
        }
    }
}