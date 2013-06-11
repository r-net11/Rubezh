using System;
using System.Linq;
using System.IO;
using Common;
using FS2Client;
using Infrastructure.Common;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static bool IsFS2Enabled
		{
			get
			{
#if DEBUG
				return File.Exists("C:/FS2.txt");
#endif
				return false;
			}
		}

		public static FS2ClientContract FS2ClientContract { get; private set; }

		public static void InitializeFS2()
		{
			try
			{
				FS2ClientContract = new FS2ClientContract(AppSettingsManager.FS2ServerAddress);
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecManager.InitializeFiresecDriver");
			}
		}

		public static void FS2UpdateDeviceStates()
		{
			var deviceStates = FS2ClientContract.GetDeviceStates();
			if (!deviceStates.HasError && deviceStates.Result != null)
			{
				foreach (var deviceState in deviceStates.Result)
				{
					var device = Devices.FirstOrDefault(x => x.UID == deviceState.DeviceUID);
					if (device != null)
					{
						device.DeviceState.States = deviceState.SerializableStates;
						device.DeviceState.SerializableParentStates = deviceState.SerializableParentStates;
						device.DeviceState.SerializableChildStates = deviceState.SerializableChildStates;
						device.DeviceState.SerializableParameters = deviceState.SerializableParameters;
					}
				}
			}
		}
	}
}