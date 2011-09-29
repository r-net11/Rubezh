using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using JournalModule.Events;

namespace JournalModule.ViewModels
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
                {
                    ServiceFactory.Events.GetEvent<ArchiveDefaultStateCheckedEvent>().Publish(this);
                }

                OnPropertyChanged("IsActive");
            }
        }
    }
}