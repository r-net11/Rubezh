using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class Month
	{
		[DataMember]
		public MonthType MonthType { get; set; }

		[DataMember]
		public List<int> SelectedDays { get; set; }
	}
}
