using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDDoorConfiguration
	{
		[DataMember]
		public int UnlockHoldInterval { get; set; }

		[DataMember]
		public int CloseTimeout { get; set; }

		[DataMember]
		public int OpenAlwaysTimeIndex { get; set; }

		[DataMember]
		public int HolidayTimeRecoNo { get; set; }

		[DataMember]
		public bool IsBreakInAlarmEnable { get; set; }

		[DataMember]
		public bool IsRepeatEnterAlarmEnable { get; set; }

		[DataMember]
		public bool IsDoorNotClosedAlarmEnable { get; set; }

		[DataMember]
		public bool IsDuressAlarmEnable { get; set; }

		[DataMember]
		public bool IsSensorEnable { get; set; }
	}
}