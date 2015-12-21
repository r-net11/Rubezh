using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using RubezhAPI;
using System;

namespace GKModule.ViewModels
{
	public class MPTDeviceSelectationViewModel : SaveCancelDialogViewModel
	{
		public MPTDeviceSelectationViewModel(GKMPTDeviceType selectedGkMptDeviceType)
		{
			Title = "Выбор типа устройства";

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
			SelectedMPTDeviceType = AvailableMPTDeviceTypes.FirstOrDefault(x => x.MPTDeviceType == selectedGkMptDeviceType);
		}
		DeviceSelectationViewModel _deviceSelectationViewModel;
		public DeviceSelectationViewModel DeviceSelectationViewModel
		{
			get { return _deviceSelectationViewModel; }
			set
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
						if (!device.IsInMPT || device.Driver.IsCardReaderOrCodeReader)
							devices.Add(device);
				}
				DeviceSelectationViewModel = new DeviceSelectationViewModel(null, devices);

				OnPropertyChanged(() => SelectedMPTDeviceType);
			}
		}

		protected override bool CanSave()
		{
			return SelectedMPTDeviceType != null && DeviceSelectationViewModel.SelectedDevice != null;
		}
	}
}