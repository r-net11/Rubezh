using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public abstract class ControlOpcDaTagStep : ProcedureStep
	{
		public ControlOpcDaTagStep()
		{
			ValueArgument = new Argument();
		}

		[DataMember]
		public Guid OpcDaServerUID { get; set; }

		[DataMember]
		public Guid OpcDaTagUID { get; set; }

		[DataMember]
		public Argument ValueArgument { get; set; }

		public override Argument[] Arguments
		{
			get { return new Argument[] { ValueArgument }; }
		}
	}

	[DataContract]
	public class ControlOpcDaTagGetStep : ControlOpcDaTagStep { public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ControlOpcDaTagGet; } } }
	[DataContract]
	public class ControlOpcDaTagSetStep : ControlOpcDaTagStep { public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ControlOpcDaTagSet; } } }

}