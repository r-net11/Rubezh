using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ChangeListStep : ProcedureStep
	{
		public ChangeListStep()
		{
			ListArgument = new Argument();
			ItemArgument = new Argument();
		}

		[DataMember]
		public Argument ListArgument { get; set; }

		[DataMember]
		public Argument ItemArgument { get; set; }

		[DataMember]
		public ChangeType ChangeType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ChangeList; } }
	}

	public enum ChangeType
	{
		[Description("Добавить в конец")]
		AddLast,

		[Description("Удалить первый")]
		RemoveFirst,

		[Description("Удалить все")]
		RemoveAll
	}
}