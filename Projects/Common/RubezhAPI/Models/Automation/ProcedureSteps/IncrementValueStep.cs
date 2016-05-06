using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class IncrementValueStep : ProcedureStep
	{
		public IncrementValueStep()
		{
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument ResultArgument { get; set; }

		[DataMember]
		public IncrementType IncrementType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.IncrementValue; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { ResultArgument }; }
		}
	}

	public enum IncrementType
	{
		[Description("Инкремент")]
		Inc,

		[Description("Декремент")]
		Dec,
	}
}