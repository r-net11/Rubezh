using System;
using System.Collections.Generic;
using System.Windows.Media;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using Infrustructure.Plans.Devices;

namespace DeviceControls
{
	public class SKDDevicePicture : BaseDevicePicture<SKDLibraryState, SKDLibraryFrame, XStateClass, SKDDeviceState>
	{
		internal SKDDevicePicture()
		{
		}

		public Brush GetBrush(FiresecAPI.SKD.SKDDevice device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetBrush(driverUID);
		}
		public Brush GetDynamicBrush(FiresecAPI.SKD.SKDDevice device)
		{
			return device == null || device.DriverUID == Guid.Empty || device.State == null ? GetBrush(device) : GetDynamicBrush(device.DriverUID, device.State);
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