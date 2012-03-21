using GroupControllerModule.Models;
using Infrastructure.Common;
using System.Collections.ObjectModel;

namespace GroupControllerModule.ViewModels
{
    public class DeviceDetailsViewModel : SaveCancelDialogContent
    {
        XDevice Device;

        public DeviceDetailsViewModel(XDevice device)
        {
            Title = "Свойства устройства";
            Device = device;

            Clauses = new ObservableCollection<ClauseViewModel>();
        }

        ObservableCollection<ClauseViewModel> _clauses;
        public ObservableCollection<ClauseViewModel> Clauses
        {
            get { return _clauses; }
            set
            {
                _clauses = value;
                OnPropertyChanged("Clauses");
            }
        }

        protected override void Save(ref bool cancel)
        {
        }
    }
}