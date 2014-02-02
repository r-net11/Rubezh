using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDWeeklyIntervalPart
	{
		public SKDWeeklyIntervalPart()
		{

		}

		[DataMember]
		public int No { get; set; }

		[DataMember]
		public bool IsHolliday { get; set; }

		[DataMember]
		public Guid TimeIntervalUID { get; set; }
	}
}