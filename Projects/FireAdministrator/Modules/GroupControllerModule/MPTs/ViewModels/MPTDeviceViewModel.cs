using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public partial class MPTDeviceViewModel : BaseViewModel
	{
		public MPTDevice MPTDevice { get; private set; }
		public XDevice Device { get; private set; }

		public MPTDeviceViewModel(MPTDevice mptDevice)
		{
			MPTDevice = mptDevice;
			Device = mptDevice.Device;
			AvailableMPTDeviceTypes = new ObservableCollection<MPTDeviceType>(MPTDevice.GetAvailableMPTDeviceTypes(MPTDevice.Device.DriverType));
		}

		public string PresentationZone
		{
			get
			{
				if (Device.IsNotUsed)
					return null;
				return XManager.GetPresentationZone(Device);
			}
		}

		public ObservableCollection<MPTDeviceType> AvailableMPTDeviceTypes { get; private set; }

		public MPTDeviceType MPTDeviceType
		{
			get { return MPTDevice.MPTDeviceType; }
			set
			{
				MPTDevice.MPTDeviceType = value;
				OnPropertyChanged("MPTDeviceType");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string Description
		{
			get { return Device.Description; }
			set
			{
				Device.Description = value;
				Device.OnChanged();
				OnPropertyChanged("Description");

				var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device.UID == Device.UID);
				if (deviceViewModel != null)
				{
					deviceViewModel.OnPropertyChanged("Description");
				}

				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public bool HasDelay
		{
			get { return Device.DriverType != XDriverType.RSR2_AM_1; }
		}

		public bool HasHold
		{
			get { return Device.DriverType == XDriverType.RSR2_MVK8; }
		}

		public int Delay
		{
			get { return MPTDevice.Delay; }
			set
			{
				MPTDevice.Delay = value;
				OnPropertyChanged("Delay");
				SetDeviceProperty("Задержка на включение, с", value);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public int Hold
		{
			get { return MPTDevice.Hold; }
			set
			{
				MPTDevice.Hold = value;
				OnPropertyChanged("Hold");
				SetDeviceProperty("Время удержания, с", value);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		void SetDeviceProperty(string propertyName, int value)
		{
			var property = Device.Properties.FirstOrDefault(x => x.Name == propertyName);
			if (property == null)
			{
				property = new XProperty()
				{
					Name = propertyName,
					DriverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == propertyName)
				};
				Device.Properties.Add(property);
			}
			property.Value = (ushort)value;
			Device.OnChanged();
			var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device.UID == Device.UID);
			if (deviceViewModel != null)
			{
				deviceViewModel.UpdateProperties();
			}
		}
	}
}