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
			ShleifNo = Device.IntAddress / 256;
			AddressOnShleif = Device.IntAddress % 256;
		}

		public string UsbChannel
		{
			get
			{
				var property = Device.Properties.FirstOrDefault(x => x.Name == "UsbChannel");
				if (property != null)
					return property.Value;
				else
					return null;
			}
		}

		public string SerialNo
		{
			get
			{
				var property = Device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
				if (property != null)
					return property.Value;
				else
					return null;
			}
		}

		public string Version
		{
			get
			{
				var property = Device.Properties.FirstOrDefault(x => x.Name == "Version");
				if (property != null)
					return property.Value;
				else
					return null;
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
				if (Device.Driver.DriverType != value.DriverType)
				{
					FiresecManager.FiresecConfiguration.ChangeDriver(Device, value);
					OnPropertyChanged("Device");
					OnPropertyChanged("Driver");
				}
			}
		}

		public int ShleifNo { get; private set; }
		public int AddressOnShleif { get; private set; }
	}
}