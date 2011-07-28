using Infrastructure.Common;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public class GroupDeviceViewModel : BaseViewModel
    {
        public Device Device { get; private set; }

        public void Initialize(Device device)
        {
            Device = device;
        }

        //public void Initialize(Firesec.Groups.RCGroupPropertiesDevice device)
        //{
        //    Device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.UID == device.UID);

        //    IsInversion = device.Inverse == "1" ? true : false;
        //    OnDelay = Convert.ToInt32(device.DelayOn);
        //    OffDelay = Convert.ToInt32(device.DelayOff);
        //}

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
