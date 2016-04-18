using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class Variable
	{
		public Variable()
		{
			Uid = Guid.NewGuid();
			ExplicitValue = new ExplicitValue();
			ExplicitValues = new List<ExplicitValue>();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public bool IsGlobal { get; set; }

		[DataMember]
		public ExplicitValue ExplicitValue { get; set; }

		[DataMember]
		public List<ExplicitValue> ExplicitValues { get; set; }

		[DataMember]
		public bool IsReference { get; set; }
	}
}