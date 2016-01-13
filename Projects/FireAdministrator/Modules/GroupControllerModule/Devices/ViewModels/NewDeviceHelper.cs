using System;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public static class NewDeviceHelper
	{
		public static DeviceViewModel AddDevice(GKDevice device, DeviceViewModel parentDeviceViewModel, bool isAddDevice = true, bool isStartList = false)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (isAddDevice)
			{
				if (isStartList)
					parentDeviceViewModel.AddChildFirst(deviceViewModel);
				else
					parentDeviceViewModel.AddChild(deviceViewModel);
				foreach (var childDevice in device.Children)
				{
					AddDevice(childDevice, deviceViewModel);
				}
			}
			else
			{
				parentDeviceViewModel.InsertChild(deviceViewModel);

				foreach (var childDevice in device.Children)
				{
					AddDevice(childDevice, deviceViewModel, !isAddDevice);
				}
			}
			return deviceViewModel;
		}
	}
}