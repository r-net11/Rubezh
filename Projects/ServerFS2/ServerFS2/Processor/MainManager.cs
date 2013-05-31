using FiresecAPI.Models;

namespace ServerFS2.Processor
{
	public static class MainManager
	{
		public static void StartMonitoring()
		{

		}

		public static void StopMonitoring()
		{

		}

		public static void SuspendMonitoring()
		{

		}

		public static void ResumeMonitoring()
		{

		}

		public static void DeviceDatetimeSync(Device device, bool isUSB)
		{
			try
			{
				SuspendMonitoring();
				ServerHelper.SynchronizeTime(device);
			}
			catch { throw; }
			finally
			{
				ResumeMonitoring();
			}
		}

		public static DeviceConfiguration DeviceReadConfig(Device device, bool isUSB)
		{
			try
			{
				SuspendMonitoring();
				return ServerHelper.GetDeviceConfig(device);
			}
			catch { throw; }
			finally
			{
				ResumeMonitoring();
			}
		}
	}
}