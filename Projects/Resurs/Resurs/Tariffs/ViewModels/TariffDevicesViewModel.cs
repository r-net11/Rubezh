using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace Resurs.ViewModels
{
	class TariffDevicesViewModel : SaveCancelDialogViewModel
	{
		public TariffDevicesViewModel(TariffDetailsViewModel tariff)
		{
			_tariff = tariff.Tariff;
			Title = "Выбор счётчиков для привязки";
			Devices = new ObservableCollection<TariffDeviceViewModel>();
			GetAllDevices(ResursDAL.DBCash.RootDevice);
			SelectedDevices = new ObservableCollection<TariffDeviceViewModel>();

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
		public ObservableCollection<TariffDeviceViewModel> Devices { get; set; }

		public ObservableCollection<TariffDeviceViewModel> SelectedDevices { get; set; }

		void GetAllDevices(Device parent)
		{
			foreach (var child in parent.Children)
			{
				if (child.DeviceType == DeviceType.Counter)
				{
					if (child.TariffType == _tariff.TariffType)
					{
						Devices.Add(new TariffDeviceViewModel(child));
					}
				}
				else
				{
					GetAllDevices(child);
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
