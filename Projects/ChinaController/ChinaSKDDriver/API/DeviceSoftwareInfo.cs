using System;

namespace StrazhDeviceSDK.API
{
	public class DeviceSoftwareInfo
	{
		public DateTime SoftwareBuildDate { get; set; }

		public string DeviceType { get; set; }

		public string SoftwareVersion { get; set; }
	}
}