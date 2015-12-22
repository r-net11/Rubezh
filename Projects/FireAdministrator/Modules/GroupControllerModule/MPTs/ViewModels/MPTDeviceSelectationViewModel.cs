using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class MPTDeviceSelectationViewModel : SaveCancelDialogViewModel
	{
		GKDevice _selectedDevice;
		public ObservableCollection<GKDevice> Devices { get { return DeviceSelectationViewModel.Devices; } }
		public GKDevice SelectedDevice
		{
			get { return DeviceSelectationViewModel.SelectedDevice; }
			set { DeviceSelectationViewModel.SelectedDevice = value; }
		}
		public MPTDeviceSelectationViewModel(GKDevice selectedDevice,GKMPTDeviceType selectedGkMptDeviceType)
		{
			Title = "Настройка устройства МПТ";

			AvailableMPTDeviceTypes = new ObservableCollection<MPTDeviceTypeViewModel>();
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.DoNotEnterBoard));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.ExitBoard));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.AutomaticOffBoard));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.Speaker));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.HandStart));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.HandStop));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.HandAutomaticOn));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.HandAutomaticOff));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.Bomb));
			_selectedDevice = selectedDevice;
			SelectedMPTDeviceType = AvailableMPTDeviceTypes.FirstOrDefault(x => x.MPTDeviceType == selectedGkMptDeviceType);
		}
		DeviceSelectationViewModel _deviceSelectationViewModel;
		public DeviceSelectationViewModel DeviceSelectationViewModel
		{
			get { return _deviceSelectationViewModel; }
			private set
			{
				_deviceSelectationViewModel = value;
				OnPropertyChanged(() => DeviceSelectationViewModel);
			}
		}

		public ObservableCollection<MPTDeviceTypeViewModel> AvailableMPTDeviceTypes { get; private set; }

		MPTDeviceTypeViewModel _selectedMPTDeviceType;
		public MPTDeviceTypeViewModel SelectedMPTDeviceType
		{
			get { return _selectedMPTDeviceType; }
			set
			{
				_selectedMPTDeviceType = value;

				var devices = new List<GKDevice>();
				foreach (var device in GKManager.Devices)
				{
					if (GKMPTDevice.GetAvailableMPTDriverTypes(_selectedMPTDeviceType.MPTDeviceType).Any(x => device.DriverType == x))
						if (!(device.IsInMPT && device != _selectedDevice) || device.Driver.IsCardReaderOrCodeReader)
							devices.Add(device);
				}
				DeviceSelectationViewModel = new DeviceSelectationViewModel(_selectedDevice, devices);
				OnPropertyChanged(() => SelectedMPTDeviceType);
			}
		}

		protected override bool CanSave()
		{
			return SelectedMPTDeviceType != null && SelectedDevice != null;
		}
	}
}