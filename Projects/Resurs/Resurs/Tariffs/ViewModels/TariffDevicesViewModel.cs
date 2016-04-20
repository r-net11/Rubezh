using Infrastructure.Common.Windows.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Resurs.ViewModels
{
	class TariffDevicesViewModel : SaveCancelDialogViewModel
	{
		public TariffDevicesViewModel(Tariff tariff)
		{
			_tariff = tariff;
			Title = "Выбор счётчиков для привязки";
			Devices = new ObservableCollection<TariffDeviceViewModel>();
			GetAllDevices();
			SelectedDevices = new List<TariffDeviceViewModel>();

			foreach (var device in Devices)
			{
				if (device.Device.TariffUID != null)
				{
					if (device.Device.TariffUID == _tariff.UID)
					{
						device.IsChecked = true;
					}
					else
					{
						device.HasTariff = true;
					}
				}
			}
		}
		Tariff _tariff;
		public ObservableCollection<TariffDeviceViewModel> Devices { get; private set; }

		public List<TariffDeviceViewModel> SelectedDevices { get; private set; }

		void GetAllDevices()
		{
			foreach (var device in DbCache.Devices)
			{
				if (device.DeviceType == DeviceType.Counter)
				{
					if (device.TariffType == _tariff.TariffType)
					{
						Devices.Add(new TariffDeviceViewModel(device));
					}
				}
			}
		}

		protected override bool Save()
		{
			foreach (var item in Devices)
			{
				if (item.Device.TariffUID == _tariff.UID)
				{
					item.Device.TariffUID = null;
				}
				if (item.IsChecked)
				{
					SelectedDevices.Add(item);
				}
			}
			return base.Save();
		}
	}
}
