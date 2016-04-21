using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FiresecAPI.Automation;

namespace FiresecAPI.Models.Automation
{
	[DataContract]
	public sealed class GlobalVariable : IVariable
	{
		public GlobalVariable()
		{
			UID = Guid.NewGuid();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public VariableValue VariableValue { get; set; }

		[DataMember]
		public bool IsReference { get; set; }

		[DataMember]
		public bool IsSaveWhenRestart { get; set; }

		[DataMember]
		public string Name { get; set; }
	}
}
