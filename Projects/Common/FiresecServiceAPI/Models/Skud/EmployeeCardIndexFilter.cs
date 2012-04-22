using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
	[DataContract]
	public class EmployeeCardIndexFilter
	{
		[DataMember]
		public string ClockNumber { get; set; }
		[DataMember]
		public string LastName { get; set; }
		[DataMember]
		public string FirstName { get; set; }
		[DataMember]
		public string SecondName { get; set; }
		[DataMember]
		public string Department { get; set; }
		[DataMember]
		public string Position { get; set; }
		[DataMember]
		public string Group { get; set; }
	}
}
