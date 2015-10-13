using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.ObjectModel;

namespace Resurs.ViewModels
{
	class TariffDevicesViewModel : SaveCancelDialogViewModel
	{
		public TariffDevicesViewModel(TariffDetailsViewModel tariff)
		{
			Title = "Выбор счётчиков для привязки";
			RootDevice = AddDeviceInternal(ResursDAL.DBCash.RootDevice, null);
			Devices = new ObservableCollection<DeviceViewModel>();
			AddChildPlainDevices(RootDevice);
		}

		public DeviceViewModel RootDevice { get; set; }
		private DeviceViewModel AddDeviceInternal(Device device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}
		void AddChildPlainDevices(DeviceViewModel parentViewModel)
		{
			Devices.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainDevices(childViewModel);
		}

		public ObservableCollection<DeviceViewModel> Devices;
		public ObservableCollection<DeviceViewModel> SelectedDevices;
		
		protected override bool Save()
		{
			return base.Save();
		}
	}
}
