using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum EnumType
	{
		[DescriptionAttribute("Состояние")]
		StateType,

		[DescriptionAttribute("Тип устройства")]
		DriverType,

		[DescriptionAttribute("Права пользователя")]
		PermissionType,

		[DescriptionAttribute("Название события")]
		JournalEventNameType,
		
		[DescriptionAttribute("Уточнение события")]
		JournalEventDescriptionType,

		[DescriptionAttribute("Тип объекта")]
		JournalObjectType
	}
}
