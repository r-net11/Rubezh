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

		public static SKDDeviceInfo GetdeviceInfo(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				SKDDeviceInfo deviceInfo = new SKDDeviceInfo();
				var deviceSoftwareInfo = deviceProcessor.Wrapper.GetDeviceSoftwareInfo();
				if (deviceSoftwareInfo != null)
				{
					deviceInfo.DeviceType = deviceSoftwareInfo.DeviceType;
					deviceInfo.SoftwareBuildDate = deviceSoftwareInfo.SoftwareBuildDate;
					deviceInfo.SoftwareVersion = deviceSoftwareInfo.SoftwareVersion;
				}
				var deviceNetInfo = deviceProcessor.Wrapper.GetDeviceNetInfo();
				if (deviceNetInfo != null)
				{
					deviceInfo.IP = deviceNetInfo.IP;
					deviceInfo.SubnetMask = deviceNetInfo.SubnetMask;
					deviceInfo.DefaultGateway = deviceNetInfo.DefaultGateway;
					deviceInfo.MTU = deviceNetInfo.MTU;
				}
				deviceInfo.CurrentDateTime = deviceProcessor.Wrapper.GetDateTime();
				return deviceInfo;
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
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				return deviceProcessor.Wrapper.GetProjectPassword();
			}
			return null;
		}

		public static bool SetPassword(Guid deviceUID, string password)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				return deviceProcessor.Wrapper.SetProjectPassword(password);
			}
			return false;
		}
	}
}