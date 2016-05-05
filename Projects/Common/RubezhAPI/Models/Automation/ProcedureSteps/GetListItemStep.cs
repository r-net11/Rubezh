using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class GetListItemStep : ProcedureStep
	{
		public GetListItemStep()
		{
			ListArgument = new Argument();
			ItemArgument = new Argument();
			IndexArgument = new Argument();
		}

		[DataMember]
		public Argument ListArgument { get; set; }

		[DataMember]
		public Argument ItemArgument { get; set; }

		[DataMember]
		public Argument IndexArgument { get; set; }

		[DataMember]
		public PositionType PositionType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.GetListItem; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { ListArgument, ItemArgument, IndexArgument }; }
		}
	}

	public enum PositionType
	{
		[Description("Первый")]
		First,

		[Description("Последний")]
		Last,

		[Description("По индексу")]
		ByIndex
	}
}