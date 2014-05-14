using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DirectionDeviceSelectationViewModel : SaveCancelDialogViewModel
	{
		public DirectionDeviceSelectationViewModel()
		{
			Title = "Выбор устройства";

			var devices = new List<XDevice>();
			foreach (var device in XManager.Devices)
			{
				if (device.Driver.IsControlDevice)
				{
					device.AllParents.ForEach(x => { devices.Add(x); });
					devices.Add(device);
				}
			}

			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in devices)
			{
				var deviceViewModel = new DeviceViewModel(device);
				deviceViewModel.IsExpanded = device.Driver.IsControlDevice;
				Devices.Add(deviceViewModel);
			}

			foreach (var device in Devices.Where(x => x.Device.Parent != null))
			{
				var parent = Devices.FirstOrDefault(x => x.Device.BaseUID == device.Device.Parent.BaseUID);
				parent.AddChild(device);
			}

			SelectedDevice = Devices.FirstOrDefault(x => x.HasChildren == false);
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}

		protected override bool CanSave()
		{
			if (SelectedDevice != null)
				return SelectedDevice.HasChildren == false;
			return false;
		}
	}
}