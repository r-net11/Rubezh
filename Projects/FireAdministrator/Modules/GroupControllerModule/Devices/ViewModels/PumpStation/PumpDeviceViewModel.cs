using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;
using FiresecClient;

namespace GKModule.ViewModels
{
    public class PumpDeviceViewModel : BaseViewModel
    {
        public XPumpStationPump PumpStationPump { get; private set; }
        public XDevice Device { get; private set; }

        public PumpDeviceViewModel(XPumpStationPump pumpStationPump)
        {
            AvailablePumpStationPumpTypes = Enum.GetValues(typeof(XPumpStationPumpType)).Cast<XPumpStationPumpType>().ToList();
            PumpStationPump = pumpStationPump;
            Device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == pumpStationPump.DeviceUID);
            SelectedPumpStationPumpType = pumpStationPump.PumpStationPumpType;
        }

        public List<XPumpStationPumpType> AvailablePumpStationPumpTypes { get; set; }

        XPumpStationPumpType _selectedPumpStationPumpType;
        public XPumpStationPumpType SelectedPumpStationPumpType
        {
            get { return _selectedPumpStationPumpType; }
            set
            {
                value = _selectedPumpStationPumpType;
                OnPropertyChanged("SelectedPumpStationPumpType");
            }
        }
    }
}