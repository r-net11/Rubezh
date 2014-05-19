using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	class FirmWareUpdateViewModel : DialogViewModel
	{
		public FirmWareUpdateViewModel(List<XDevice> devices)
		{
			Title = "Выберете устройства, у которых следуют обновить ПО";
			UpdatedDevices = Initialize(devices);
			SelectAllCommand = new RelayCommand(OnSelectAll);
			DeSelectAllCommand = new RelayCommand(OnDeSelectAll);
			UpdateCommand = new RelayCommand(OnUpdate, CanUpdate);
		}

		public RelayCommand SelectAllCommand { get; private set; }
		void OnSelectAll()
		{
			foreach (var device in UpdatedDevices)
			{
				device.IsChecked = true;
			}
		}
		public RelayCommand DeSelectAllCommand { get; private set; }
		void OnDeSelectAll()
		{
			foreach (var device in UpdatedDevices)
			{
				device.IsChecked = false;
			}
		}
		public List<UpdatedDeviceViewModel> UpdatedDevices { get; set; }
		List<UpdatedDeviceViewModel> Initialize(List<XDevice> devices)
		{
			return devices.Select(device => new UpdatedDeviceViewModel(device)).ToList();
		}

		string updateButtonToolTip = "Обновить";
		public string UpdateButtonToolTip
		{
			get
			{
				return updateButtonToolTip;
			}
			set
			{
				updateButtonToolTip = value;
				OnPropertyChanged("UpdateButtonToolTip");
			}
		}
		public RelayCommand UpdateCommand { get; private set; }
		void OnUpdate()
		{
			Close(true);
		}

		bool CanUpdate()
		{
			if (UpdatedDevices.Any(x => x.IsChecked))
			{
				UpdateButtonToolTip = "Обновить";
				return true;
			}
			UpdateButtonToolTip = "Выберите хотя бы одно устройство";
			return false;
		}
	}
}
