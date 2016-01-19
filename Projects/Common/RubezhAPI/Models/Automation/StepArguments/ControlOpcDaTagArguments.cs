using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ControlOpcDaTagArguments
	{
		public ControlOpcDaTagArguments()
		{
			ValueArgument = new Argument();
		}

		[DataMember]
		public Guid OpcDaServerUID { get; set; }

		[DataMember]
		public Guid OpcDaTagUID { get; set; }

		[DataMember]
		public Argument ValueArgument { get; set; }
	}
}