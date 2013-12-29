using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    class FirmWareUpdateViewModel : DialogViewModel
    {
        public FirmWareUpdateViewModel(List<XDevice> devices)
        {
            Title = "Выберете устройства, у которых следуют обновить ПО";
            UpdatedDevices = Initialize(devices);
            UpdateCommand = new RelayCommand(OnUpdate, CanUpdate);
        }
        public List<UpdatedDeviceViewModel> UpdatedDevices { get; set; }
        List<UpdatedDeviceViewModel> Initialize(List<XDevice> devices)
        {
            return devices.Select(device => new UpdatedDeviceViewModel(device)).ToList();
        }

        string updateButtonToolTip = "Обновить";
        public string UpdateButtonToolTip
        {
            get
            {
                return updateButtonToolTip;
            }
            set
            {
                updateButtonToolTip = value;
                OnPropertyChanged("UpdateButtonToolTip");
            }
        }
        public RelayCommand UpdateCommand { get; private set; }
        void OnUpdate()
        {
            Close(true);
        }

        bool CanUpdate()
        {
            if (UpdatedDevices.Any(x => x.IsChecked))
            {
                UpdateButtonToolTip = "Обновить";
                return true;
            }
            UpdateButtonToolTip = "Выберите хотя бы одно устройство";
            return false;
        }
    }
}
