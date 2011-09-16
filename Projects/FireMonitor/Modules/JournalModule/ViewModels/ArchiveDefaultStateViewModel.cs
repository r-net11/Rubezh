using FiresecAPI.Models;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class ArchiveDefaultStateViewModel : BaseViewModel
    {
        public ArchiveDefaultStateViewModel(ArchiveDefaultStateType archiveDefaultStateType)
        {
            ArchiveDefaultStateType = archiveDefaultStateType;
        }

        public ArchiveDefaultStateType ArchiveDefaultStateType { get; private set; }

        bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                _IsActive = value;
                OnPropertyChanged("IsActive");
            }
        }
    }
}