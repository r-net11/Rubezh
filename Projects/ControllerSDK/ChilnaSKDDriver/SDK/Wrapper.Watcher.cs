using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using System.Threading;
using System.Diagnostics;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		Thread WatcherThread;
		bool IsStopping;

		public void StartWatcher()
		{
			NativeWrapper.WRAP_StartListen(LoginID);

			IsStopping = false;
			WatcherThread = new Thread(RunMonitoring);
			WatcherThread.Start();
		}

		public void StopWatcher()
		{
			IsStopping = true;
			if (WatcherThread != null)
			{
				WatcherThread.Join(TimeSpan.FromSeconds(2));
			}
		}

		void RunMonitoring()
		{
			var lastIndex = -1;
			while (true)
			{
				if (IsStopping)
					return;
				Thread.Sleep(100);

				var index = NativeWrapper.WRAP_GetLastIndex();
				if (index > lastIndex)
				{
					for (int i = lastIndex + 1; i <= index; i++)
					{
						var wrapJournalItem = new NativeWrapper.WRAP_JournalItem();
						NativeWrapper.WRAP_GetJournalItem(i, out wrapJournalItem);
						var journalItem = ParceJournal(wrapJournalItem);

						if (NewJournalItem != null)
							NewJournalItem(journalItem);
					}
					lastIndex = index;
				}
			}
		}

		const int DH_ALARM_VEHICLE_CONFIRM = 0x2169;
		const int DH_ALARM_VEHICLE_LARGE_ANGLE = 0x2170;
		const int DH_ALARM_TALKING_INVITE = 0x2171;
		const int DH_ALARM_ALARM_EX2 = 0x2175;
		const int DH_ALARM_ALARMEXTENDED = 0x3174;
		const int DH_URGENCY_ALARM_EX = 0x210B;
		const int DH_URGENCY_ALARM_EX2 = 0x3182;
		const int DH_ALARM_BATTERYLOWPOWER = 0x2134;
		const int DH_ALARM_TEMPERATURE = 0x2135;
		const int DH_ALARM_POWERFAULT = 0x3172;
		const int DH_ALARM_CHASSISINTRUDED = 0x3173;
		const int DH_ALARM_INPUT_SOURCE_SIGNAL = 0x3183;
		const int DH_ALARM_ACCESS_CTL_EVENT = 0x3181;
		const int DH_ALARM_ACCESS_CTL_NOT_CLOSE = 0x3177;
		const int DH_ALARM_ACCESS_CTL_BREAK_IN = 0x3178;
		const int DH_ALARM_ACCESS_CTL_REPEAT_ENTER = 0x3179;
		const int DH_ALARM_ACCESS_CTL_DURESS = 0x3180;

		SKDJournalItem ParceJournal(NativeWrapper.WRAP_JournalItem wrapJournalItem)
		{
			var journalItem = new SKDJournalItem();
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.DeviceDateTime = Wrapper.NET_TIMEToDateTime(wrapJournalItem.DeviceDateTime);

			var name = "";
			var description = "";
			switch(wrapJournalItem.EventType)
			{
				case DH_ALARM_VEHICLE_CONFIRM:
					name = "DH_ALARM_VEHICLE_CONFIRM";
					var stGPSStatusInfo = wrapJournalItem.stGPSStatusInfo;
					var bEventAction = wrapJournalItem.bEventAction;
					var szInfo = CharArrayToString(wrapJournalItem.szInfo);
						break;

				case DH_ALARM_VEHICLE_LARGE_ANGLE:
						name = "DH_ALARM_VEHICLE_LARGE_ANGLE";
						stGPSStatusInfo = wrapJournalItem.stGPSStatusInfo;
						bEventAction = wrapJournalItem.bEventAction;
					break;
			

				case DH_ALARM_TALKING_INVITE:
					name = "TalkingInvite";
					var emCaller = wrapJournalItem.emCaller;
					break;

				case DH_ALARM_ALARM_EX2:
						name = "LocalAlarm";
						var nChannelID = wrapJournalItem.nChannelID;
						var nAction = wrapJournalItem.nAction;
						var emSenseType = wrapJournalItem.emSenseType;
						break;

				case DH_ALARM_ALARMEXTENDED:
						name = "AlarmExtend";
						nChannelID = wrapJournalItem.nChannelID;
						nAction = wrapJournalItem.nAction;
						break;

				case DH_URGENCY_ALARM_EX:
						name = "Urgency";
						break;

				case DH_URGENCY_ALARM_EX2:
						name = "UrgencyEx2";
						break;

				case DH_ALARM_BATTERYLOWPOWER:
						name = "BatteryLowPower";
						nAction = wrapJournalItem.nAction;
						var nBatteryLeft = wrapJournalItem.nBatteryLeft;
						break;

				case DH_ALARM_TEMPERATURE:
						name = "Temperature";
						var szSensorName = CharArrayToString(wrapJournalItem.szSensorName);
						nChannelID = wrapJournalItem.nChannelID;
						nAction = wrapJournalItem.nAction;
						var fTemperature = wrapJournalItem.fTemperature;
						break;

				case DH_ALARM_POWERFAULT:
						name = "PowerFault";
						var emPowerType = wrapJournalItem.emPowerType;
						var emPowerFaultEvent = wrapJournalItem.emPowerFaultEvent;
						nAction = wrapJournalItem.nAction;
						break;

				case DH_ALARM_CHASSISINTRUDED:
						name = "ChassisIntruded";
						nAction = wrapJournalItem.nAction;
						nChannelID = wrapJournalItem.nChannelID;
						break;

				case DH_ALARM_INPUT_SOURCE_SIGNAL:
						name = "AlarmInputSourceSignal";
						nAction = wrapJournalItem.nAction;
						nChannelID = wrapJournalItem.nChannelID;
						break;

				case DH_ALARM_ACCESS_CTL_EVENT:
						name = "Проход";
						var nDoor = wrapJournalItem.nDoor;
						var emEventType = wrapJournalItem.emEventType;
						var szDoorName = CharArrayToString(wrapJournalItem.szDoorName);
						var bStatus = wrapJournalItem.bStatus;
						var emCardType = wrapJournalItem.emCardType;
						var emOpenMethod = wrapJournalItem.emOpenMethod;
						var szCardNo = CharArrayToString(wrapJournalItem.szCardNo);
						var szPwd = CharArrayToString(wrapJournalItem.szPwd);
						break;

				case DH_ALARM_ACCESS_CTL_NOT_CLOSE:
						name = "door not close";
						nDoor = wrapJournalItem.nDoor;
						nAction = wrapJournalItem.nAction;
						szDoorName = CharArrayToString(wrapJournalItem.szDoorName);
						break;

				case DH_ALARM_ACCESS_CTL_BREAK_IN:
						name = "break in";
						nDoor = wrapJournalItem.nDoor;
						szDoorName = CharArrayToString(wrapJournalItem.szDoorName);
						break;

				case DH_ALARM_ACCESS_CTL_REPEAT_ENTER:
						name = "repeat enter";
						nDoor = wrapJournalItem.nDoor;
						szDoorName = CharArrayToString(wrapJournalItem.szDoorName);
						break;

				case DH_ALARM_ACCESS_CTL_DURESS:
						name = "duress";
						nDoor = wrapJournalItem.nDoor;
						szDoorName = CharArrayToString(wrapJournalItem.szDoorName);
						szCardNo = CharArrayToString(wrapJournalItem.szCardNo);
						break;

				default:
						name = "Неизвестное событие";
						break;
			}
			journalItem.Name = name;
			journalItem.Description = description;

			return journalItem;
		}
	}
}