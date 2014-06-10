using System;
using System.Collections.Generic;

namespace ControllerSDK.API
{
	public class DeviceSoftwareInfo
	{
		public DateTime SoftwareBuildDate { get; set; }
		public string DeviceType { get; set; }
		public string SoftwareVersion { get; set; }
	}

	public class DeviceNetInfo
	{
		public string IP { get; set; }
		public string SubnetMask { get; set; }
		public string DefaultGateway { get; set; }
		public Int32 MTU;
	}

	public class DeviceJournalItem
	{
		public DateTime DateTime { get; set; }
		public string OperatorName { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}

	public class DeviceGeneralInfo
	{
		public string OpenDoorAudioPath { get; set; }
		public string CloseDoorAudioPath { get; set; }
		public string InUsedAuidoPath { get; set; }
		public string PauseUsedAudioPath { get; set; }
		public string NotClosedAudioPath { get; set; }
		public string WaitingAudioPath { get; set; }
		public Int32 UnlockReloadTime { get; set; }
		public Int32 UnlockHoldTime { get; set; }
		public bool IsProjectPassword { get; set; }
		public string ProjectPassword { get; set; }
	}

	public class TimeInterval
	{
		public TimeSpan StartDateTime { get; set; }
		public TimeSpan EndDateTime { get; set; }
	}

	public class NamedTimeInterval
	{
		public NamedTimeInterval()
		{
			Intervals = new List<TimeInterval>();
		}

		public List<TimeInterval> Intervals;
	}

	public class ControllerConfig
	{
		public ControllerConfig()
		{
			NamedTimeIntervals = new List<NamedTimeInterval>();
		}

		public List<NamedTimeInterval> NamedTimeIntervals;
	}

	public class Card
	{
		public int RecordNo { get; set; }
		public DateTime CreationDateTime { get; set; }
		public string CardNo { get; set; }
		public string UserID { get; set; }
		public ControllerSDK.SDK.SDKImport.NET_ACCESSCTLCARD_STATE CardStatus { get; set; }
		public ControllerSDK.SDK.SDKImport.NET_ACCESSCTLCARD_TYPE CardType { get; set; }
		public string Password { get; set; }
		public int DoorsCount { get; set; }
		public int[] Doors { get; set; }
		public int TimeSectionsCount { get; set; }
		public int[] TimeSections { get; set; }
		public int UserTime { get; set; }
		public DateTime ValidStartDateTime { get; set; }
		public DateTime ValidEndDateTime { get; set; }
		public bool IsValid { get; set; }
	}

	public class Password
	{
		public int RecordNo { get; set; }
		public DateTime CreationDateTime { get; set; }
		public string UserID { get; set; }
		public string DoorOpenPassword { get; set; }
		public string AlarmPassword { get; set; }
		public int DoorsCount { get; set; }
		public int[] Doors { get; set; }
	}
}