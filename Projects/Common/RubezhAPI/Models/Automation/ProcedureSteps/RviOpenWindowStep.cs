using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{

	public class RviOpenWindowStep : ProcedureStep
	{
		public RviOpenWindowStep()
		{
			NameArgument = new Argument();
			NameArgument.StringValue = "";
			XArgument = new Argument();
			XArgument.IntValue = 0;
			YArgument = new Argument();
			YArgument.IntValue = 0;
			MonitorNumberArgument = new Argument();
			MonitorNumberArgument.IntValue = 1;
			LoginArgument = new Argument();
			LoginArgument.StringValue = "";
			IpArgument = new Argument();
			IpArgument.StringValue = "";
		}
		[DataMember]
		public Argument NameArgument { get; set; }
		[DataMember]
		public Argument XArgument { get; set; }
		[DataMember]
		public Argument YArgument { get; set; }
		[DataMember]
		public Argument MonitorNumberArgument { get; set; }
		[DataMember]
		public Argument LoginArgument { get; set; }
		[DataMember]
		public Argument IpArgument { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.RviOpenWindow; } }
	}
}