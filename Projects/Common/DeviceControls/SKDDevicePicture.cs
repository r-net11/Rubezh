using StrazhAPI.GK;
using StrazhAPI.Plans.Devices;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DeviceControls
{
	public class SKDDevicePicture : BaseDevicePicture<SKDLibraryState, SKDLibraryFrame, SKDDeviceState>
	{
		internal SKDDevicePicture()
		{
		}

		public Brush GetBrush(SKDDevice device)
		{
			var driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetBrush(driverUID);
		}
		public Brush GetDynamicBrush(SKDDevice device)
		{
			return device == null || device.DriverUID == Guid.Empty || device.State == null ? GetBrush(device) : GetDynamicBrush(device.DriverUID, device.State);
		}

		protected override IEnumerable<ILibraryDevice<SKDLibraryState, SKDLibraryFrame>> EnumerateLibrary()
		{
			return SKDManager.SKDLibraryConfiguration.Devices;
		}

		protected override XStateClass DefaultState
		{
			get { return XStateClass.No; }
		}
	}
}