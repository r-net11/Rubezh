using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
	[DataContract]
	public class SkudPerson
	{
		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public string LastNmae { get; set; }
		[DataMember]
		public string FirstName { get; set; }
		[DataMember]
		public string SecondName { get; set; }
		[DataMember]
		public DateTime? Birthday { get; set; }
		[DataMember]
		public DateTime? Sex { get; set; }
		[DataMember]
		public string Comment { get; set; }
	}
}
