using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DevicesMenuViewModel
    {
        public RelayCommand CopyCommand { get; set; }
        public RelayCommand CutCommand { get; set; }
        public RelayCommand PasteCommand { get; set; }
        public RelayCommand AutoDetectCommand { get; set; }
        public RelayCommand ReadDeviceCommand { get; set; }
        public RelayCommand WriteDeviceCommand { get; set; }
        public RelayCommand WriteAllDeviceCommand { get; set; }
        public RelayCommand SynchronizeDeviceCommand { get; set; }
        public RelayCommand RebootDeviceCommand { get; set; }

        public RelayCommand UpdateSoftCommand { get; set; }
        public RelayCommand GetDescriptionCommand { get; set; }
        public RelayCommand SetPasswordCommand { get; set; }
        public RelayCommand GetDeveceJournalCommand { get; set; }
    }
}