using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class GetJournalItemStep : ProcedureStep
	{
		public GetJournalItemStep()
		{
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument ResultArgument { get; set; }

		[DataMember]
		public JournalColumnType JournalColumnType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.GetJournalItem; } }
	}

	public enum JournalColumnType
	{
		[Description("Дата в приборе")]
		DeviceDateTime,

		[Description("Дата в системе")]
		SystemDateTime,

		[Description("Название")]
		JournalEventNameType,

		[Description("Уточнение")]
		JournalEventDescriptionType,

		[Description("Тип объекта")]
		JournalObjectType,

		[Description("Идентификатор объекта")]
		JournalObjectUid,
	}
}