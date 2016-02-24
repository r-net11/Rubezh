using RubezhAPI;
using RubezhAPI.Models;
using RviClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FiresecService
{
	public static class RviProcessor
	{
		static Thread _thread;
		static AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
		static List<RviState> _rviStates = new List<RviState>();
		public static void Start()
		{
			_thread = new Thread(OnRun);
			_thread.Start();
		}
		public static void Stop()
		{
			if (_autoResetEvent != null)
			{
				_autoResetEvent.Set();
				if (_thread != null)
				{
					_thread.Join(TimeSpan.FromSeconds(2));
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
			_autoResetEvent = new AutoResetEvent(false);
			while (true)
			{
				if (_autoResetEvent.WaitOne(TimeSpan.FromSeconds(1)))
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
							_rviStates.Add(new RviState(server, RviStatus.ConnectionLost));
							foreach (var rviDevice in server.RviDevices)
							{
								rviDevice.Status = RviStatus.ConnectionLost;
								_rviStates.Add(new RviState(rviDevice, RviStatus.ConnectionLost));
							}
						}
						else
						{
							_rviStates.Add(new RviState(server, RviStatus.Connected));
							foreach (var newDevice in newDevices)
							{
								var oldDevice = server.RviDevices.FirstOrDefault(x => x.Uid == newDevice.Uid && x.Status != newDevice.Status);
								if (oldDevice != null)
								{
									oldDevice.Status = newDevice.Status;
									_rviStates.Add(new RviState(newDevice, newDevice.Status));
								}
							}
						}
					}
				}
				if (_rviStates.Count != 0)
				{
					var rviCallbackResult = new RviCallbackResult();
					rviCallbackResult.RviStates.AddRange(_rviStates);
					_rviStates.Clear();
					FiresecService.Service.FiresecService.NotifyRviObjectStateChanged(rviCallbackResult);
				}
			}
		}
	}
}