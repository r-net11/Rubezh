using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Events;

namespace FiresecAPI.Models
{
	[DataContract]
	public class JournalFilter
	{
		public JournalFilter()
		{
			Uid = Guid.NewGuid();
			Name = "Новый фильтр";
			EventNames = new List<GlobalEventNameEnum>();
			SubsystemTypes = new List<GlobalSubsystemType>();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<GlobalEventNameEnum> EventNames { get; set; }

		[DataMember]
		public List<GlobalSubsystemType> SubsystemTypes { get; set; }
	}
}