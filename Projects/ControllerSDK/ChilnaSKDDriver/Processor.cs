using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD;

namespace ChinaSKDDriver
{
	public static class Processor
	{
		public static SKDConfiguration SKDConfiguration { get; private set; }
		public static List<DeviceProcessor> DeviceProcessors { get; private set; }

		public static void Run(SKDConfiguration skdConfiguration)
		{
			DeviceProcessors = new List<DeviceProcessor>();
			SKDConfiguration = skdConfiguration;

			ChinaSKDDriverNativeApi.NativeWrapper.WRAP_Initialize();

			foreach (var device in skdConfiguration.RootDevice.Children)
			{
				var deviceProcessor = new DeviceProcessor(device);
				DeviceProcessors.Add(deviceProcessor);
				deviceProcessor.Run();
			}
		}

		public static string GetdeviceInfo(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				var deviceSoftwareInfo = deviceProcessor.Wrapper.GetDeviceSoftwareInfo();
				if (deviceSoftwareInfo != null)
				{
					return deviceSoftwareInfo.DeviceType + "\n" + deviceSoftwareInfo.SoftwareBuildDate.ToString() + "\n" + deviceSoftwareInfo.SoftwareVersion;
				}
			}
			return null;
		}

		public static bool SyncronyseTime(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				return deviceProcessor.Wrapper.SetDateTime(DateTime.Now);
			}
			return false;
		}

		public static string GetPassword(Guid deviceUID)
		{
			return "123";
		}

		public static bool SetPassword(Guid deviceUID, string password)
		{
			return true;
		}
	}
}