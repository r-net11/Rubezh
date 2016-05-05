using System.ComponentModel;
using System.Runtime.Serialization;

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
		[Description("Дата на устройстве")]
		DeviceDateTime = 0,

		[Description("Дата в системе")]
		SystemDateTime = 1,

		[Description("Название события")]
		JournalEventNameType = 2,

		[Description("Уточнение")]
		JournalEventDescriptionType = 3,

		[Description("Тип объекта")]
		JournalObjectType = 4,

		[Description("Идентификатор объекта")]
		JournalObjectUid = 5,

		[Description("Пользователь")]
		UserUid = 6,

		[Description("Сотрудник")]
		EmployeeUid = 7,

		[Description("Посетитель")]
		VisitorUid = 8,

		[Description("Номер пропуска")]
		CardNo = 9,

		[Description("Тип пропуска")]
		CardType = 10,
	}
}