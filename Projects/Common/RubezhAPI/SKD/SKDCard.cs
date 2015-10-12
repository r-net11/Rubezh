using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDCard
	{
		[DataMember]
		public int IDFamily { get; set; }

		[DataMember]
		public int IDNo { get; set; }

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public DateTime EndDate { get; set; }
	}
}