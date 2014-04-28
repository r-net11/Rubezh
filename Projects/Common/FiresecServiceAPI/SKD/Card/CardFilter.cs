using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class CardFilter : IsDeletedFilter
	{
		public CardFilter()
			: base()
		{
			WithDeleted = DeletedType.All;
		}

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