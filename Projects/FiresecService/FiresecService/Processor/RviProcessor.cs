using RubezhAPI;
using RubezhAPI.Models;
using RviClient;
using System;
using System.Linq;
using System.Threading;

namespace FiresecService
{
	public static class RviProcessor
	{
		static Thread Thread;
		static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
		public static void Start()
		{
			Thread = new Thread(OnRun);
			Thread.Start();
		}
		public static void Stop()
		{
			if (AutoResetEvent != null)
			{
				AutoResetEvent.Set();
				if (Thread != null)
				{
					Thread.Join(TimeSpan.FromSeconds(2));
				}
			}
		}
		public static void SetNewConfig()
		{
			Stop();
			Start();
		}
		static void OnRun()
		{
			AutoResetEvent = new AutoResetEvent(false);
			while (true)
			{
				if (AutoResetEvent.WaitOne(TimeSpan.FromSeconds(1)))
				{
					return;
				}
				if (ConfigurationCashHelper.SystemConfiguration != null && ConfigurationCashHelper.SystemConfiguration.RviSettings != null && ConfigurationCashHelper.SystemConfiguration.RviServers != null)
				{
					var rviSettings = ConfigurationCashHelper.SystemConfiguration.RviSettings;
					foreach (var server in ConfigurationCashHelper.SystemConfiguration.RviServers)
					{
						bool isNotConnected;
						var newDevices = RviClientHelper.GetRviDevicesWithoutChannels(server.Url, rviSettings.Login, rviSettings.Password, out isNotConnected);
						if (isNotConnected)
						{
							CreateRviServerCallbackResult(server, RviStatus.ConnectionLost);
							server.RviDevices.ForEach(rviDevice => CreateRviDeviceCallbackResult(rviDevice, RviStatus.ConnectionLost));
						}
						else
						{
							CreateRviServerCallbackResult(server, RviStatus.Connected);
							foreach (var newDevice in newDevices)
							{
								if (server.RviDevices.Any(x => x.Uid == newDevice.Uid))
									CreateRviDeviceCallbackResult(newDevice, newDevice.Status);
							}
						}
					}
				}
			}
		}
		static void CreateRviServerCallbackResult(RviServer server, RviStatus status)
		{
			var rviServerCallbackResult = new RviCallbackResult();
			rviServerCallbackResult.RviStates.Add(new RviState(server, status));
			FiresecService.Service.FiresecService.NotifyRviObjectStateChanged(rviServerCallbackResult);
		}
		static void CreateRviDeviceCallbackResult(RviDevice device, RviStatus status)
		{
			var rviDeviceCallbackResult = new RviCallbackResult();
			rviDeviceCallbackResult.RviStates.Add(new RviState(device, status));
			FiresecService.Service.FiresecService.NotifyRviObjectStateChanged(rviDeviceCallbackResult);
		}
	}
}