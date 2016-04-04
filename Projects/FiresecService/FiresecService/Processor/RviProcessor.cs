using RubezhAPI;
using RubezhAPI.Journal;
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
		public static List<RviState> GetRviStates()
		{
			var rviStates = new List<RviState>();
			if (ConfigurationCashHelper.SystemConfiguration != null && ConfigurationCashHelper.SystemConfiguration.RviSettings != null && ConfigurationCashHelper.SystemConfiguration.RviServers != null
						&& ConfigurationCashHelper.SystemConfiguration.Cameras != null)
			{
				var rviSettings = ConfigurationCashHelper.SystemConfiguration.RviSettings;
				foreach (var server in ConfigurationCashHelper.SystemConfiguration.RviServers)
				{
					bool isNotConnected;
					var newDevices = RviClientHelper.GetRviDevices(server.Url, rviSettings.Login, rviSettings.Password, ConfigurationCashHelper.SystemConfiguration.Cameras, out isNotConnected);
					if (isNotConnected)
					{
						rviStates.Add(new RviState(server, RviStatus.ConnectionLost));
						foreach (var rviDevice in server.RviDevices)
						{
							rviStates.Add(new RviState(rviDevice, RviStatus.ConnectionLost));
							foreach (var camera in rviDevice.Cameras)
							{
								rviStates.Add(new RviState(camera, RviStatus.ConnectionLost, false, false, camera.RviStreams));
							}
						}
					}
					else
					{
						rviStates.Add(new RviState(server, RviStatus.Connected));
						foreach (var rviDevice in server.RviDevices)
						{
							rviStates.Add(new RviState(rviDevice, rviDevice.Status));
							rviDevice.Cameras.ForEach(camera => rviStates.Add(new RviState(camera, camera.Status, camera.IsOnGuard, camera.IsRecordOnline, camera.RviStreams)));
						}
					}
				}
			}
			return rviStates;
		}
		static void OnRun()
		{
			_autoResetEvent = new AutoResetEvent(false);
			while (true)
			{
				try
				{
					if (_autoResetEvent.WaitOne(TimeSpan.FromSeconds(1)))
					{
						return;
					}
					if (ConfigurationCashHelper.SystemConfiguration != null && ConfigurationCashHelper.SystemConfiguration.RviSettings != null && ConfigurationCashHelper.SystemConfiguration.RviServers != null
						&& ConfigurationCashHelper.SystemConfiguration.Cameras != null)
					{
						var rviStates = new List<RviState>();
						var journalItems = new List<JournalItem>();
						var rviSettings = ConfigurationCashHelper.SystemConfiguration.RviSettings;
						foreach (var server in ConfigurationCashHelper.SystemConfiguration.RviServers)
						{
							bool isNotConnected;
							var newDevices = RviClientHelper.GetRviDevices(server.Url, rviSettings.Login, rviSettings.Password, ConfigurationCashHelper.SystemConfiguration.Cameras, out isNotConnected);
							if (isNotConnected)
							{
								if (server.Status != RviStatus.ConnectionLost)
								{
									rviStates.Add(new RviState(server, RviStatus.ConnectionLost));
									server.Status = RviStatus.ConnectionLost;
									foreach (var rviDevice in server.RviDevices)
									{
										rviDevice.Status = RviStatus.ConnectionLost;
										rviStates.Add(new RviState(rviDevice, RviStatus.ConnectionLost));
										foreach (var camera in rviDevice.Cameras)
										{
											camera.Status = RviStatus.ConnectionLost;
											rviStates.Add(new RviState(camera, RviStatus.ConnectionLost, false, false, camera.RviStreams));
										}
									}
								}
							}
							else
							{
								if (server.Status != RviStatus.Connected)
								{
									rviStates.Add(new RviState(server, RviStatus.Connected));
									server.Status = RviStatus.Connected;
								}

								foreach (var oldDevice in server.RviDevices)
								{
									var newDevice = newDevices.FirstOrDefault(x => x.Uid == oldDevice.Uid);
									if (newDevice != null)
									{
										if (oldDevice.Status != newDevice.Status)
										{
											oldDevice.Status = newDevice.Status;
											rviStates.Add(new RviState(oldDevice, oldDevice.Status));
										}
										foreach (var oldCamera in oldDevice.Cameras)
										{
											var newCamera = newDevice.Cameras.FirstOrDefault(x => x.UID == oldCamera.UID);
											if (newCamera != null)
											{
												var isOnGuardChanged = oldCamera.IsOnGuard != newCamera.IsOnGuard;
												var isRecordOnlineChanged = oldCamera.IsRecordOnline != newCamera.IsRecordOnline;
												if (oldCamera.Status != newCamera.Status || isOnGuardChanged || isRecordOnlineChanged)
												{
													var rviState = new RviState(newCamera, newCamera.Status, newCamera.IsOnGuard, newCamera.IsRecordOnline, newCamera.RviStreams);
													rviStates.Add(rviState);
													oldCamera.Status = newCamera.Status;
													oldCamera.IsOnGuard = newCamera.IsOnGuard;
													oldCamera.IsRecordOnline = newCamera.IsRecordOnline;

													if (isOnGuardChanged)
														journalItems.Add(CreateOnGuardJournalItem(oldCamera.UID, oldCamera.IsOnGuard));
													if (isRecordOnlineChanged)
														journalItems.Add(CreateRecordOnlineJournalItem(oldCamera.UID, oldCamera.IsRecordOnline));
												}
												if (oldCamera.RviStreams.Count() != newCamera.RviStreams.Count()) // спросить у Ромы
												{
													oldCamera.RviStreams = newCamera.RviStreams;
													rviStates.Add(new RviState(oldCamera, oldCamera.Status, oldCamera.IsOnGuard, oldCamera.IsRecordOnline, oldCamera.RviStreams));
												}
											}
											else
											{
												if (oldCamera.Status != RviStatus.Error)
												{
													oldCamera.Status = RviStatus.Error;
													rviStates.Add(new RviState(oldCamera, oldCamera.Status, false, false, oldCamera.RviStreams));
												}
											}
										}
									}
									else
									{
										if (oldDevice.Status != RviStatus.Error)
										{
											oldDevice.Status = RviStatus.Error;
											rviStates.Add(new RviState(oldDevice, RviStatus.Error));
										}
										foreach (var oldCamera in oldDevice.Cameras)
										{
											if (oldCamera.Status != RviStatus.Error)
											{
												oldCamera.Status = RviStatus.Error;
												rviStates.Add(new RviState(oldCamera, oldCamera.Status, false, false, oldCamera.RviStreams));

											}
										}
									}
								}
							}
						}
						if (rviStates.Count != 0)
						{
							var rviCallbackResult = new RviCallbackResult();
							rviCallbackResult.RviStates.AddRange(rviStates);
							FiresecService.Service.FiresecService.NotifyRviObjectStateChanged(rviCallbackResult);
							FiresecService.Service.FiresecService.NotifyJournalItems(journalItems, true);
						}
					}
				}
				catch (Exception) { }
			}
		}
		static JournalItem CreateJournalItem(Guid objectUid, JournalObjectType journalObjectType, JournalEventNameType journalEventNameType, string desriptionText = null)
		{
			var journalItem = new JournalItem
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = null,
				JournalObjectType = journalObjectType,
				JournalEventNameType = journalEventNameType,
				DescriptionText = desriptionText,
				JournalEventDescriptionType = JournalEventDescriptionType.NULL,
				ObjectUID = objectUid,
				UserName = string.Empty,
			};
			return journalItem;
		}
		static JournalItem CreateOnGuardJournalItem(Guid cameraUid, bool isOnGuard)
		{
			JournalItem journalItem;
			if (isOnGuard)
				journalItem = CreateJournalItem(cameraUid, JournalObjectType.Camera, JournalEventNameType.Канал_Rvi_поставлен_на_охрану);
			else
				journalItem = CreateJournalItem(cameraUid, JournalObjectType.Camera, JournalEventNameType.Канал_Rvi_снят_с_охраны);
			return journalItem;
		}
		static JournalItem CreateRecordOnlineJournalItem(Guid cameraUid, bool isRecordOnline)
		{
			JournalItem journalItem;
			if (isRecordOnline)
				journalItem = CreateJournalItem(cameraUid, JournalObjectType.Camera, JournalEventNameType.Начата_запись_на_канале_Rvi);
			else
				journalItem = CreateJournalItem(cameraUid, JournalObjectType.Camera, JournalEventNameType.Прекращена_запись_на_канале_Rvi);
			return journalItem;
		}

	}
}