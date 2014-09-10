using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace FiresecAPI.Automation
{
	public class ArithmeticParameter
	{
		public ArithmeticParameter()
		{
			VariableUid = new Guid();
			VariableItem = new VariableItem();
		}

		[DataMember]
		public VariableScope VariableScope { get; set; }

		[DataMember]
		public VariableItem VariableItem { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }
	
		[DataMember]
		public Guid VariableUid { get; set; }
	}
}
