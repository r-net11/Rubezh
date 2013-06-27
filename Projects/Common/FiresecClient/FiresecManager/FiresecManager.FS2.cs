using System;
using System.IO;
using System.Linq;
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
				return File.Exists("C:/FS2.txt");
			}
		}

		public static FS2ClientContract FS2ClientContract { get; private set; }

		public static void InitializeFS2()
		{
			try
			{
				FS2ClientContract = new FS2ClientContract(ConnectionSettingsManager.FS2ServerAddress);
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

			var zoneStates = FS2ClientContract.GetZoneStates();
			if (!zoneStates.HasError && zoneStates.Result != null)
			{
				foreach (var zoneState in zoneStates.Result)
				{
					var zone = Zones.FirstOrDefault(x => x.UID == zoneState.ZoneUID);
					if (zone != null)
					{
						zone.ZoneState.StateType = zoneState.StateType;
					}
				}
			}
		}
	}
}