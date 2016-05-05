using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class RandomStep : ProcedureStep
	{
		public RandomStep()
		{
			MaxValueArgument = new Argument();
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument MaxValueArgument { get; set; }

		[DataMember]
		public Argument ResultArgument { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.Random; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { MaxValueArgument, ResultArgument }; }
		}
	}
}