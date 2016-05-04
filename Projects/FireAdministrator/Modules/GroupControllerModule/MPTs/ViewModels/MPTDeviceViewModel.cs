using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using System.Collections.Generic;
using System.Linq;

namespace GKModule.ViewModels
{
	public class MPTDeviceViewModel : BaseViewModel
	{
		public GKMPTDevice MPTDevice { get; set; }
		public bool IsCodeReader { get; set; }

		public MPTDeviceViewModel(GKMPTDevice mptDevice)
		{
			MPTDevice = mptDevice;
			Device = mptDevice.Device;
			MPTDevicePropertiesViewModel = new MPTDevicePropertiesViewModel(Device, false);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
		}

		GKDevice _device;
		public GKDevice Device
		{
			get { return _device; }
			set
			{
				_device = value;
				if (_device != null)
				{
					IsCodeReader = _device.Driver.IsCardReaderOrCodeReader;
				}
				OnPropertyChanged(() => IsCodeReader);
				OnPropertyChanged(() => Device);
				OnPropertyChanged(() => Description);
			}
		}

		MPTDevicePropertiesViewModel _mptDevicePropertiesViewModel;
		public MPTDevicePropertiesViewModel MPTDevicePropertiesViewModel
		{
			get { return _mptDevicePropertiesViewModel; }
			set
			{
				_mptDevicePropertiesViewModel = value;
				OnPropertyChanged(() => MPTDevicePropertiesViewModel);
			}
		}

		public string PresentationZone
		{
			get
			{
				return GKManager.GetPresentationZoneOrLogic(Device);
			}
		}

		public GKMPTDeviceType MPTDeviceType
		{
			get { return MPTDevice.MPTDeviceType; }
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var mptCodeReaderDetailsViewModel = new MPTCodeReaderDetailsViewModel(MPTDevice.CodeReaderSettings, MPTDeviceType);
			if (DialogService.ShowModalWindow(mptCodeReaderDetailsViewModel))
			{
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string Description
		{
			get { return Device != null ? Device.Description : null; }
			set
			{
				if (Device == null)
					return;

				Device.Description = value;
				OnPropertyChanged(() => Description);

				var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device.UID == Device.UID);
				if (deviceViewModel != null)
				{
					deviceViewModel.OnPropertyChanged("Description");
				}
				Device.OnChanged();

				ServiceFactory.SaveService.GKChanged = true;
			}
		}
	}
}