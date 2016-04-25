using System;
using System.Runtime.Serialization;
using FiresecAPI.Automation;

namespace FiresecAPI.Models.Automation
{
	[DataContract]
	public sealed class LocalVariable : IVariable
	{
		public LocalVariable()
		{
			UID = Guid.NewGuid();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public VariableValue VariableValue { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public bool IsReference { get; set; }
	}
}
