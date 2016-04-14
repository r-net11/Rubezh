using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class Argument : ExplicitValue
	{
		[DataMember]
		public VariableScope VariableScope { get; set; }

		[DataMember]
		public Guid VariableUid { get; set; }
	}
}