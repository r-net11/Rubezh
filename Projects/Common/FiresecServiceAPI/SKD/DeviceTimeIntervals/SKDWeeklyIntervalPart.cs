using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDWeeklyIntervalPart
	{
		[DataMember]
		public int No { get; set; }

		[DataMember]
		public int DayIntervalID { get; set; }
	}
}