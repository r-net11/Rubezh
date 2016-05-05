using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using StrazhAPI.GK;

namespace StrazhAPI.Models
{
	[DataContract]
	public class Email
	{
		public Email()
		{
			States = new List<XStateClass>();
			Zones = new List<Guid>();
			IsActivated = false;
		}

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public List<XStateClass> States { get; set; }

		[DataMember]
		public List<Guid> Zones { get; set; }

		[DataMember]
		public string MessageTitle { get; set; }

		[XmlIgnore]
		public bool IsActivated { get; set; }
	}
}