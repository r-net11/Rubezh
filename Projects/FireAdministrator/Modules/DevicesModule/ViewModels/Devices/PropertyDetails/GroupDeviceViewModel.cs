using Infrastructure.Common;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public class GroupDeviceViewModel : BaseViewModel
    {
        public Device Device { get; private set; }

        public void Initialize(Device device)
        {
            Device = device;
        }

        public void Initialize(PDUGroupDevice device)
        {
            Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.DeviceUID);

            IsInversion = device.IsInversion;
            OnDelay = device.OnDelay;
            OffDelay = device.OffDelay;
        }

        bool _isInversion;
        public bool IsInversion
        {
            get { return _isInversion; }
            set
            {
                _isInversion = value;
                OnPropertyChanged("IsInversion");
            }
        }

        int _onDelay;
        public int OnDelay
        {
            get { return _onDelay; }
            set
            {
                _onDelay = value;
                OnPropertyChanged("OnDelay");
            }
        }

        int _offDelay;
        public int OffDelay
        {
            get { return _offDelay; }
            set
            {
                _offDelay = value;
                OnPropertyChanged("OffDelay");
            }
        }
    }
}
