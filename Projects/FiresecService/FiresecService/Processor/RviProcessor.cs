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
						var newDevices = RviClientHelper.GetRviDevices(server.Url, rviSettings.Login, rviSettings.Password, ConfigurationCashHelper.SystemConfiguration.Cameras, out isNotConnected);
						if (isNotConnected)
						{
							_rviStates.Add(new RviState(server, RviStatus.ConnectionLost));
							foreach (var rviDevice in server.RviDevices)
							{
								rviDevice.Status = RviStatus.ConnectionLost;
								_rviStates.Add(new RviState(rviDevice, RviStatus.ConnectionLost));
								foreach (var camera in rviDevice.Cameras)
								{
									camera.Status = RviStatus.ConnectionLost;
									_rviStates.Add(new RviState(camera, RviStatus.ConnectionLost, false, false, camera.RviStreams));
								}
							}
						}
						else
						{
							_rviStates.Add(new RviState(server, RviStatus.Connected));
							foreach (var oldDevice in server.RviDevices)
							{
								var newDevice = newDevices.FirstOrDefault(x => x.Uid == oldDevice.Uid);
								if (newDevice != null)
								{
									if (oldDevice.Status != newDevice.Status)
									{
										oldDevice.Status = newDevice.Status;
										_rviStates.Add(new RviState(oldDevice, oldDevice.Status));

									}
									foreach (var oldCamera in oldDevice.Cameras)
									{
										var newCamera = newDevice.Cameras.FirstOrDefault(x => x.UID == oldCamera.UID);
										if (newCamera != null)
										{
											if (oldCamera.Status != newCamera.Status || oldCamera.IsOnGuard != newCamera.IsOnGuard || oldCamera.IsRecordOnline != newCamera.IsRecordOnline)
											{
												oldCamera.Status = newCamera.Status;
												oldCamera.IsOnGuard = newCamera.IsOnGuard;
												oldCamera.IsRecordOnline = newCamera.IsRecordOnline;
												_rviStates.Add(new RviState(oldCamera, oldCamera.Status, oldCamera.IsOnGuard, oldCamera.IsRecordOnline, oldCamera.RviStreams));
											}
											if (oldCamera.RviStreams != newCamera.RviStreams)
											{
												oldCamera.RviStreams = newCamera.RviStreams;
												_rviStates.Add(new RviState(oldCamera, oldCamera.Status, oldCamera.IsOnGuard, oldCamera.IsRecordOnline, oldCamera.RviStreams));
											}
										}
										else
										{
											oldCamera.Status = RviStatus.Error;
											_rviStates.Add(new RviState(oldCamera, RviStatus.Error, false, false, oldCamera.RviStreams));
										}
									}
								}
								else
								{
									oldDevice.Status = RviStatus.Error;
									_rviStates.Add(new RviState(oldDevice, RviStatus.Error));
									foreach (var oldCamera in oldDevice.Cameras)
									{
										oldDevice.Status = RviStatus.Error;
										_rviStates.Add(new RviState(oldDevice, RviStatus.Error));
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
}