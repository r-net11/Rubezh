using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class CardFilter : IsDeletedFilter
	{
		public CardFilter()
			: base()
		{
			DeactivationType = LogicalDeletationType.All;
		}

		[DataMember]
		public LogicalDeletationType DeactivationType { get; set; }

		[DataMember]
		public int FirstSeries { get; set; }

		[DataMember]
		public int LastSeries { get; set; }

		[DataMember]
		public int FirstNos { get; set; }

		[DataMember]
		public int LastNos { get; set; }
	}
}