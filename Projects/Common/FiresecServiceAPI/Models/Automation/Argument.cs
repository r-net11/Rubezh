using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class Argument
	{
		public Argument()
		{
			ExplicitValue = new ExplicitValue();
			ExplicitValues = new List<ExplicitValue>();
		}

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }

		[DataMember]
		public VariableScope VariableScope { get; set; }

		[DataMember]
		public Guid VariableUid { get; set; }

		[DataMember]
		public ExplicitValue ExplicitValue { get; set; }

		[DataMember]
		public List<ExplicitValue> ExplicitValues { get; set; }
	}
}