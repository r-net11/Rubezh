using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace ClientFS2.ViewModels
{
	public class DeviceViewModel : TreeItemViewModel<DeviceViewModel>
	{
		public Device Device { get; private set; }
		public DeviceViewModel(Device device)
		{
			Device = device;
		}
        public int UsbChannel 
        { 
            get
            {
                var parentChannel = Device.ParentChannel; 
                return parentChannel != null ? parentChannel.IntAddress : 0;
            }
        }
        public string SerialNo
		{
			get
			{
				var property = Device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
				return property != null ? property.Value : null;
			}
		}
        public string Version
		{
			get
			{
				var property = Device.Properties.FirstOrDefault(x => x.Name == "Version");
				return property != null ? property.Value : null;
			}
		}
		public string Address
		{
			get { return Device.PresentationAddress; }
			set
			{
				Device.SetAddress(value);
				if (Driver.IsChildAddressReservedRange)
				{
					foreach (var deviceViewModel in Children)
					{
						deviceViewModel.OnPropertyChanged("Address");
					}
				}
				OnPropertyChanged("Address");
			}
		}
		public bool IsUsed
		{
			get { return !Device.IsNotUsed; }
			set
			{
				FiresecManager.FiresecConfiguration.SetIsNotUsed(Device, !value);
				OnPropertyChanged("IsUsed");
				OnPropertyChanged("ShowOnPlan");
				OnPropertyChanged("PresentationZone");
				OnPropertyChanged("EditingPresentationZone");
			}
		}
		public Driver Driver
		{
			get { return Device.Driver; }
			set
			{
			    if (Device.Driver.DriverType == value.DriverType) return;
			    FiresecManager.FiresecConfiguration.ChangeDriver(Device, value);
			    OnPropertyChanged("Device");
			    OnPropertyChanged("Driver");
			}
		}
		public int ShleifNo { get { return Device.IntAddress/256; } }
        public int AddressOnShleif { get { return Device.IntAddress % 256; } }
	}
}