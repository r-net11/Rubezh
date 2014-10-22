using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
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
		[Description("Дата в приборе")]
		DeviceDateTime,

		[Description("Дата в системе")]
		SystemDateTime,

		[Description("Название")]
		JournalEventNameType,

		[Description("Уточнение")]
		JournalEventDescriptionType,

		[Description("Тип объекта")]
		JournalObjectType
	}
}