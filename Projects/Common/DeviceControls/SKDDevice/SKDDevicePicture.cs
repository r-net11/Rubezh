using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using FiresecAPI;
using XFiresecAPI;
using Infrustructure.Plans.Devices;

namespace DeviceControls.SKDDevice
{
	public class SKDDevicePicture : BaseDevicePicture<SKDLibraryState, SKDLibraryFrame, XStateClass, SKDDeviceState>
	{
		public Brush GetBrush(FiresecAPI.SKDDevice device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetBrush(driverUID);
		}
		public Brush GetDynamicBrush(FiresecAPI.SKDDevice device)
		{
			return device == null || device.DriverUID == Guid.Empty || device.State == null ? GetBrush(device) : GetDynamicBrush(device.DriverUID, string.Empty, device.State);
		}

		protected override IEnumerable<ILibraryDevice<SKDLibraryState, SKDLibraryFrame, XStateClass>> EnumerateLibrary()
		{
			foreach (var device in SKDManager.SKDLibraryConfiguration.Devices)
				yield return device;
		}
		protected override XStateClass DefaultState
		{
			get { return XStateClass.No; }
		}
	}
}