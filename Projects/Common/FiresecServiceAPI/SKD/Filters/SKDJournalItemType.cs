using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public enum SKDJournalItemType
	{
		[EnumMember]
		[DescriptionAttribute("Система")]
		System,

		[EnumMember]
		[DescriptionAttribute("Устройство ГК")]
		GKDevice,

		[EnumMember]
		[DescriptionAttribute("Зона ГК")]
		GKZone,

		[EnumMember]
		[DescriptionAttribute("Контроллер")]
		Controller,

		[EnumMember]
		[DescriptionAttribute("Считыватель")]
		Reader,
	}
}