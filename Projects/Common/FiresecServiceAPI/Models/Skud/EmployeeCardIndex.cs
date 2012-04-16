using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
	[DataContract]
	public class EmployeeCardIndex
	{
		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public int PersonId { get; set; }
		[DataMember]
		public string LastName { get; set; }
		[DataMember]
		public string FirstName { get; set; }
		[DataMember]
		public string SecondName { get; set; }
		[DataMember]
		public int? Age { get; set; }
		[DataMember]
		public string Department { get; set; }
		[DataMember]
		public string Position { get; set; }
		[DataMember]
		public string Comment { get; set; }
	}
}
