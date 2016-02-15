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
				if (ConfigurationCashHelper.SystemConfiguration != null && ConfigurationCashHelper.SystemConfiguration.RviSettings != null && ConfigurationCashHelper.SystemConfiguration.RviServers != null)
				{
					var rviSettings = ConfigurationCashHelper.SystemConfiguration.RviSettings;
					if (AutoResetEvent.WaitOne(TimeSpan.FromSeconds(1)))
					{
						return;
					}
					foreach (var server in ConfigurationCashHelper.SystemConfiguration.RviServers)
					{
						if (RviClientHelper.IsConnected(server.Url, rviSettings.Login, rviSettings.Password))
						{
							CreateRviServerCallbackResult(server, RviStatus.Connected);
							var newDevices = RviClientHelper.GetRviDevicesWithoutChannels(server.Url, rviSettings.Login, rviSettings.Password);
							foreach (var device in newDevices)
							{
								var oldDevice = server.RviDevices.FirstOrDefault(x => x.Uid != device.Uid && x.Status != device.Status);
								if (oldDevice != null)
								{
									oldDevice.Status = RviStatus.Connected;
									CreateRviDeviceCallbackResult(oldDevice, RviStatus.Connected);
								}
							}
						}
						else
						{
							CreateRviServerCallbackResult(server, RviStatus.ConnectionLost);
							foreach (var rviDevice in server.RviDevices)
							{
								rviDevice.Status = RviStatus.ConnectionLost;
								CreateRviDeviceCallbackResult(rviDevice, RviStatus.ConnectionLost);
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