using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class Email
	{
		public Email()
		{
			States = new List<StateType>();
			Zones = new List<Guid>();
		}

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public List<StateType> States { get; set; }

		[DataMember]
		public List<Guid> Zones { get; set; }

		[DataMember]
		public string MessageTitle { get; set; }
	}
}