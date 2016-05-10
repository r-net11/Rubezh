using System;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDHoliday
	{
		public SKDHoliday()
		{
			UID = Guid.NewGuid();
			TypeNo = 1;
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public DateTime DateTime { get; set; }

		[DataMember]
		public int TypeNo { get; set; }
	}
}