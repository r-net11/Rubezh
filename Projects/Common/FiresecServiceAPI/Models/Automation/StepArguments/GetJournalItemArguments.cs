using System.ComponentModel;
using System.Runtime.Serialization;
using LocalizationConveters;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class GetJournalItemArguments
	{
		public GetJournalItemArguments()
		{
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument ResultArgument { get; set; }

		[DataMember]
		public JournalColumnType JournalColumnType { get; set; }
	}

	public enum JournalColumnType
	{
		//[Description("Дата на устройстве")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.GetJournalItemArguments), "DeviceDateTime")]
		DeviceDateTime = 0,

		//[Description("Дата в системе")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.GetJournalItemArguments), "SystemDateTime")]
        SystemDateTime = 1,

		//[Description("Название события")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.GetJournalItemArguments), "JournalEventNameType")]
        JournalEventNameType = 2,

		//[Description("Уточнение")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.GetJournalItemArguments), "JournalEventDescriptionType")]
        JournalEventDescriptionType = 3,

		//[Description("Тип объекта")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.GetJournalItemArguments), "JournalObjectType")]
        JournalObjectType = 4,

		//[Description("Идентификатор объекта")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.GetJournalItemArguments), "JournalObjectUid")]
        JournalObjectUid = 5,

		//[Description("Пользователь")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.GetJournalItemArguments), "UserUid")]
        UserUid = 6,

		//[Description("Сотрудник")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.GetJournalItemArguments), "EmployeeUid")]
        EmployeeUid = 7,

		//[Description("Посетитель")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.GetJournalItemArguments), "VisitorUid")]
        VisitorUid = 8,

		//[Description("Номер пропуска")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.GetJournalItemArguments), "CardNo")]
        CardNo = 9,

		//[Description("Тип пропуска")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.GetJournalItemArguments), "CardType")]
        CardType = 10,
	}
}