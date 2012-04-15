using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
	[DataContract]
	public class EmployeeCard
	{
		[DataMember]
		public int Id { get; set; }
		//public int PersonId { get; set; }
		[DataMember]
		public string Staff { get; set; }
		[DataMember]
		public string Position { get; set; }
		[DataMember]
		public string Comment { get; set; }
		//[DataMember]
		//public SkudPerson Person { get; set; }
		[DataMember]
		public string LastName { get; set; }
		[DataMember]
		public string FirstName { get; set; }
		[DataMember]
		public string SecondName { get; set; }
		[DataMember]
		public DateTime? Birthday { get; set; }
		[DataMember]
		public DateTime? Sex { get; set; }
		//[DataMember]
		//public string Comment { get; set; }
	}
}
