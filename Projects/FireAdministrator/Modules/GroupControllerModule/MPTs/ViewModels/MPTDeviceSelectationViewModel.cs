using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.ViewModels
{
	public class MPTDeviceSelectationViewModel : SaveCancelDialogViewModel
	{
		GKDevice oldSelectedDevice;
		public GKMPTDevice MptDevice { get; set; }
		public ObservableCollection<GKDevice> Devices { get { return DeviceSelectationViewModel.Devices; } }
		public GKDevice SelectedDevice
		{
			get { return DeviceSelectationViewModel.SelectedDevice; }
			set { DeviceSelectationViewModel.SelectedDevice = value; }
		}
		public MPTDeviceSelectationViewModel(GKMPTDevice mptDevice = null)
		{
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
			if (mptDevice == null)
			{
				Title = "Создание устройства МПТ";
				MptDevice = new GKMPTDevice { MPTDeviceType = AvailableMPTDeviceTypes.First().MPTDeviceType };
			}
			else
			{
				Title = "Настройка устройства МПТ";
				MptDevice = mptDevice;
			}
			CopyProperties();
		}
		void CopyProperties()
		{
			oldSelectedDevice = MptDevice.Device;
			SelectedMPTDeviceType = AvailableMPTDeviceTypes.FirstOrDefault(x => x.MPTDeviceType == MptDevice.MPTDeviceType);
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
						if (!(device.IsInMPT && device != oldSelectedDevice) || device.Driver.IsCardReaderOrCodeReader)
							devices.Add(device);
				}
				DeviceSelectationViewModel = new DeviceSelectationViewModel(oldSelectedDevice, devices);
				OnPropertyChanged(() => SelectedMPTDeviceType);
			}
		}
		protected override bool Save()
		{
			MPTViewModel.ChangeIsInMPT(oldSelectedDevice, false);
			MptDevice.Device = SelectedDevice;
			MptDevice.DeviceUID = SelectedDevice.UID;
			MptDevice.MPTDeviceType = SelectedMPTDeviceType.MPTDeviceType;
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedMPTDeviceType != null && SelectedDevice != null;
		}
	}
}