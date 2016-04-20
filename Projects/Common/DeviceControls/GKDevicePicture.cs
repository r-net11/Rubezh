using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Plans.Devices;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DeviceControls
{
	public class GKDevicePicture : BaseDevicePicture<GKLibraryState, GKLibraryFrame, GKState>
	{
		internal GKDevicePicture()
		{
		}

		public Brush GetBrush(GKDevice device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetBrush(driverUID);
		}
		public Brush GetDynamicBrush(GKDevice device)
		{
			return device == null || device.DriverUID == Guid.Empty || device.State == null ? GetBrush(device) : GetDynamicBrush(device.DriverUID, device.State);
		}

		protected override IEnumerable<ILibraryDevice<GKLibraryState, GKLibraryFrame, XStateClass>> EnumerateLibrary()
		{
			foreach (var device in GKManager.DeviceLibraryConfiguration.GKDevices)
				yield return device;
		}
		protected override XStateClass DefaultState
		{
			get { return XStateClass.No; }
		}
	}
}