using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using FiresecClient;
using XFiresecAPI;
using Infrustructure.Plans.Devices;

namespace DeviceControls.XDevice
{
	public class XDevicePicture : BaseDevicePicture<LibraryXState, LibraryXFrame, XStateClass, XState>
	{
		public Brush GetBrush(XFiresecAPI.XDevice device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetBrush(driverUID);
		}
		public Brush GetDynamicBrush(XFiresecAPI.XDevice device)
		{
			return device == null || device.DriverUID == Guid.Empty || device.State == null ? GetBrush(device) : GetDynamicBrush(device.DriverUID, string.Empty, device.State);
		}

		protected override IEnumerable<ILibraryDevice<LibraryXState, LibraryXFrame, XStateClass>> EnumerateLibrary()
		{
			foreach (var device in XManager.DeviceLibraryConfiguration.XDevices)
				yield return device;
		}
		protected override XStateClass DefaultState
		{
			get { return XStateClass.No; }
		}
	}
}