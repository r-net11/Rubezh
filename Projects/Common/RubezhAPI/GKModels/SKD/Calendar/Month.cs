using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.GK
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
