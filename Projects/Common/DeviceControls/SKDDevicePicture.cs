using System;
using System.Collections.Generic;
using System.Windows.Media;
using StrazhAPI.GK;
using StrazhAPI.SKD;
using Infrustructure.Plans.Devices;

namespace DeviceControls
{
	public class SKDDevicePicture : BaseDevicePicture<SKDLibraryState, SKDLibraryFrame, SKDDeviceState>
	{
		internal SKDDevicePicture()
		{
		}

		public Brush GetBrush(SKDDevice device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetBrush(driverUID);
		}
		public Brush GetDynamicBrush(SKDDevice device)
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