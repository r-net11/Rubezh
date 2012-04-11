using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
	[DataContract]
	public class SkudEmployee
	{
		[DataMember]
		public int Id { get; set; }
		//public int PersonId { get; set; }
		[DataMember]
		public string Staff { get; set; }
		[DataMember]
		public string Comment { get; set; }
		[DataMember]
		public SkudPerson Person { get; set; }
	}
}
