using System;
using System.Collections.Generic;
using System.Windows.Media;
using FiresecAPI.GK;
using FiresecClient;
using Infrustructure.Plans.Devices;

namespace DeviceControls
{
	public class XDevicePicture : BaseDevicePicture<LibraryXState, LibraryXFrame, XStateClass, XState>
	{
		internal XDevicePicture()
		{
		}

		public Brush GetBrush(XDevice device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetBrush(driverUID);
		}
		public Brush GetDynamicBrush(XDevice device)
		{
			return device == null || device.DriverUID == Guid.Empty || device.State == null ? GetBrush(device) : GetDynamicBrush(device.DriverUID, device.State);
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