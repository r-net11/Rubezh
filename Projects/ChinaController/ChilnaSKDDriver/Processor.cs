using System;
using System.Collections.Generic;
using System.Linq;
using ChinaSKDDriverAPI;
using FiresecAPI.GK;
using FiresecAPI.SKD;

namespace ChinaSKDDriver
{
	public static class Processor
	{
		public static SKDConfiguration SKDConfiguration { get; private set; }
		public static List<DeviceProcessor> DeviceProcessors { get; private set; }
		public static event Action<SKDCallbackResult> SKDCallbackResultEvent;

		static Processor()
		{
#if DEBUG
			System.IO.File.Copy(@"D:\Projects\Projects\ChinaController\CPPWrapper\Bin\CPPWrapper.dll", @"D:\Projects\Projects\FiresecService\bin\Debug\CPPWrapper.dll", true);
#endif
		}

		public static void DoCallback(SKDCallbackResult callbackResult)
		{
			if (Processor.SKDCallbackResultEvent != null)
				Processor.SKDCallbackResultEvent(callbackResult);
		}

		public static void Run(SKDConfiguration skdConfiguration)
		{
			DeviceProcessors = new List<DeviceProcessor>();
			SKDConfiguration = skdConfiguration;

			ChinaSKDDriverNativeApi.NativeWrapper.WRAP_Initialize();


			foreach (var device in skdConfiguration.Devices)
			{
				device.State = new SKDDeviceState();
				device.State.UID = device.UID;
				device.State.StateClass = XStateClass.Unknown;
			}
			skdConfiguration.RootDevice.State.StateClass = XStateClass.Norm;

			foreach (var device in skdConfiguration.RootDevice.Children)
			{
				var deviceProcessor = new DeviceProcessor(device);
				DeviceProcessors.Add(deviceProcessor);
				deviceProcessor.Run();
			}
		}

		public static void Stop()
		{
			foreach (var deviceProcessor in DeviceProcessors)
			{
				deviceProcessor.Wrapper.StopWatcher();
			}
		}

		public static SKDStates SKDGetStates()
		{
			var skdStates = new SKDStates();
			foreach (var device in SKDManager.Devices)
			{
				skdStates.DeviceStates.Add(device.State);
			}
			foreach (var zone in SKDManager.Zones)
			{
				skdStates.ZoneStates.Add(zone.State);
			}
			return skdStates;
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

		public static bool ResetController(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				return deviceProcessor.Wrapper.Reset();
			}
			return false;
		}

		public static bool RebootController(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				var result = deviceProcessor.Wrapper.Reboot();
				deviceProcessor.Reconnect();
				return result;
			}
			return false;
		}

		public static bool SKDWriteTimeSheduleConfiguration(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				for (int i = 0; i < SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals.Count; i++)
				{
					var weeklyInterval = SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals[i];
					var timeShedules = new List<TimeShedule>();
					foreach (var weeklyIntervalPart in weeklyInterval.WeeklyIntervalParts)
					{
						if (!weeklyIntervalPart.IsHolliday)
						{
							var timeShedule = new TimeShedule();
							var timeInterval = SKDConfiguration.TimeIntervalsConfiguration.TimeIntervals.FirstOrDefault(x => x.UID == weeklyIntervalPart.TimeIntervalUID);
							if (timeInterval != null)
							{
								foreach (var timeIntervalPart in timeInterval.TimeIntervalParts)
								{
									var timeSheduleInterval = new TimeSheduleInterval();
									timeSheduleInterval.BeginHours = timeIntervalPart.StartTime.Hour;
									timeSheduleInterval.BeginMinutes = timeIntervalPart.StartTime.Minute;
									timeSheduleInterval.BeginSeconds = timeIntervalPart.StartTime.Second;
									timeSheduleInterval.EndHours = timeIntervalPart.EndTime.Hour;
									timeSheduleInterval.EndMinutes = timeIntervalPart.EndTime.Minute;
									timeSheduleInterval.EndSeconds = timeIntervalPart.EndTime.Second;
									timeShedule.TimeSheduleIntervals.Add(timeSheduleInterval);
								}
								for (int j = timeShedule.TimeSheduleIntervals.Count; j < 4; j++)
								{
									var timeSheduleInterval = new TimeSheduleInterval();
									timeShedule.TimeSheduleIntervals.Add(timeSheduleInterval);
								}
							}
							timeShedules.Add(timeShedule);
						}
					}
					var result = deviceProcessor.Wrapper.SetTimeShedules(i, timeShedules);
					if (!result)
						return false;
				}
				return true;
			}
			return false;
		}

		public static SKDDoorConfiguration GetDoorConfiguration(Guid deviceUID)
		{
			var readerDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (readerDevice == null)
				return null;
			var controllerDevice = readerDevice.Parent;
			if (controllerDevice == null)
				return null;

			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == controllerDevice.UID);
			if (deviceProcessor != null)
			{
				var nativeDoorConfiguration = deviceProcessor.Wrapper.GetDoorConfiguration(readerDevice.IntAddress);

				var doorConfiguration = new SKDDoorConfiguration();
				doorConfiguration.UnlockHoldInterval = nativeDoorConfiguration.UnlockHoldInterval;
				doorConfiguration.CloseTimeout = nativeDoorConfiguration.CloseTimeout;
				doorConfiguration.OpenAlwaysTimeIndex = nativeDoorConfiguration.OpenAlwaysTimeIndex;
				doorConfiguration.HolidayTimeRecoNo = nativeDoorConfiguration.HolidayTimeRecoNo;
				doorConfiguration.IsBreakInAlarmEnable = nativeDoorConfiguration.IsBreakInAlarmEnable;
				doorConfiguration.IsRepeatEnterAlarmEnable = nativeDoorConfiguration.IsRepeatEnterAlarmEnable;
				doorConfiguration.IsDoorNotClosedAlarmEnable = nativeDoorConfiguration.IsDoorNotClosedAlarmEnable;
				doorConfiguration.IsDuressAlarmEnable = nativeDoorConfiguration.IsDuressAlarmEnable;
				doorConfiguration.IsSensorEnable = nativeDoorConfiguration.IsSensorEnable;
				return doorConfiguration;
			}
			return null;
		}

		public static bool SetDoorConfiguration(Guid deviceUID, SKDDoorConfiguration doorConfiguration)
		{
			var readerDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (readerDevice == null)
				return false;
			var controllerDevice = readerDevice.Parent;
			if (controllerDevice == null)
				return false;

			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == controllerDevice.UID);
			if (deviceProcessor != null)
			{
				var nativeDoorConfiguration = new DoorConfiguration();
				nativeDoorConfiguration.UnlockHoldInterval = doorConfiguration.UnlockHoldInterval;
				nativeDoorConfiguration.CloseTimeout = doorConfiguration.CloseTimeout;
				nativeDoorConfiguration.OpenAlwaysTimeIndex = doorConfiguration.OpenAlwaysTimeIndex;
				nativeDoorConfiguration.HolidayTimeRecoNo = doorConfiguration.HolidayTimeRecoNo;
				nativeDoorConfiguration.IsBreakInAlarmEnable = doorConfiguration.IsBreakInAlarmEnable;
				nativeDoorConfiguration.IsRepeatEnterAlarmEnable = doorConfiguration.IsRepeatEnterAlarmEnable;
				nativeDoorConfiguration.IsDoorNotClosedAlarmEnable = doorConfiguration.IsDoorNotClosedAlarmEnable;
				nativeDoorConfiguration.IsDuressAlarmEnable = doorConfiguration.IsDuressAlarmEnable;
				nativeDoorConfiguration.IsSensorEnable = doorConfiguration.IsSensorEnable;

				nativeDoorConfiguration.UseDoorOpenMethod = true;
				nativeDoorConfiguration.UseUnlockHoldInterval = true;
				nativeDoorConfiguration.UseCloseTimeout = true;
				nativeDoorConfiguration.UseOpenAlwaysTimeIndex = true;
				nativeDoorConfiguration.UseHolidayTimeIndex = true;
				nativeDoorConfiguration.UseBreakInAlarmEnable = true;
				nativeDoorConfiguration.UseRepeatEnterAlarmEnable = true;
				nativeDoorConfiguration.UseDoorNotClosedAlarmEnable = true;
				nativeDoorConfiguration.UseDuressAlarmEnable = true;
				nativeDoorConfiguration.UseDoorTimeSection = false;
				nativeDoorConfiguration.UseSensorEnable = true;

				var result = deviceProcessor.Wrapper.SetDoorConfiguration(nativeDoorConfiguration, readerDevice.IntAddress);
				return result;
			}
			return false;
		}

		public static bool OpenDoor(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				var result = deviceProcessor.Wrapper.OpenDoor(deviceProcessor.Device.IntAddress);
				return result;
			}
			return false;
		}

		public static bool CloseDoor(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				var result = deviceProcessor.Wrapper.CloseDoor(deviceProcessor.Device.IntAddress);
				return result;
			}
			return false;
		}

		public static CardWriter AddCard(SKDCard skdCard)
		{
			var cardWriter = new CardWriter();
			var result = cardWriter.AddCard(skdCard);
			return cardWriter;
		}

		public static CardWriter EditCard(SKDCard skdCard)
		{
			var cardWriter = new CardWriter();
			//var result = cardWriter.AddCard(skdCard);
			return cardWriter;
		}

		public static CardWriter DeleteCard(SKDCard skdCard)
		{
			var cardWriter = new CardWriter();
			//var result = cardWriter.AddCard(skdCard);
			return cardWriter;
		}
	}
}