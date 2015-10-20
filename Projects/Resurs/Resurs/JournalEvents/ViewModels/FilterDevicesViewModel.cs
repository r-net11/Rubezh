using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class FilterDevicesViewModel
	{

		public FilterDevicesViewModel(Filter filter)
		{
			BildTree();
			if (AllDevices != null)
			{
				AllDevices.ForEach(x =>
				{
					if (filter.DeviceUIDs.Contains(x.Device.UID))
						x.IsChecked = true;
				});
			}
			
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
				foreach (var child in RootDevice.Children)
					child.IsExpanded = true;
			}
		}

		public FilterDeviceViewModel RootDevice { get; set; }

		List<FilterDeviceViewModel> AllDevices { get; set; }

		void BildTree()
		{
			RootDevice = AddConsumerInternal(DBCash.RootDevice);
			FillAllDevices();
		}

		public FilterDeviceViewModel[] RootDevices
		{
			get { return new[] { RootDevice }; }
		}

		private FilterDeviceViewModel AddConsumerInternal(Device device, FilterDeviceViewModel parentDeviceViewModel =null)
		{
			var deviceViewModel = new FilterDeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddConsumerInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}

		public void FillAllDevices()
		{
			AllDevices = new List<FilterDeviceViewModel>();
			AddChildPlainDevices(RootDevice);
		}
		void AddChildPlainDevices(FilterDeviceViewModel parentViewModel)
		{
			AllDevices.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainDevices(childViewModel);
		}

		public List<Guid?> GetDeviceUIDs()
		{
			List<Guid?> DeviceUIDs = new List<Guid?>();
			foreach (var device in AllDevices)
			{
				if (device.IsChecked)
					DeviceUIDs.Add(device.Device.UID);
			}
			return DeviceUIDs;
		}
	}
}
