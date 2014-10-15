using System;
using FiresecAPI.Journal;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriverAPI
{
	public class SKDJournalItem
	{
		public int LoginID { get; set; }
		public DateTime SystemDateTime { get; set;}
		public DateTime DeviceDateTime { get; set; }
		public JournalEventNameType JournalEventNameType { get; set; }
		public string Description { get; set; }
		public int CardNo { get; set; }
		public int DoorNo { get; set; }

		public NativeWrapper.NET_ACCESS_CTL_EVENT_TYPE emEventType;
		public bool bStatus;
		public NativeWrapper.NET_ACCESSCTLCARD_TYPE emCardType;
		public NativeWrapper.NET_ACCESS_DOOROPEN_METHOD emOpenMethod;
		public string szPwd;
		public int nAction;
		public NativeWrapper.NET_ACCESS_CTL_STATUS_TYPE emStatus;
		public string szReaderID;
		public string szDoorName;
	}
}