using System.ComponentModel;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public enum JournalItemType
	{
		[EnumMember]
		[DescriptionAttribute("Система")]
		System,

		[EnumMember]
		[DescriptionAttribute("ГК")]
		GK,

		[EnumMember]
		[DescriptionAttribute("Устройство")]
		Device,

		[EnumMember]
		[DescriptionAttribute("Зона")]
		Zone,

		[EnumMember]
		[DescriptionAttribute("Направление")]
		Direction,

		[EnumMember]
		[DescriptionAttribute("Задержка")]
		Delay,

		[EnumMember]
		[DescriptionAttribute("НС")]
		PumpStation,

		[EnumMember]
		[DescriptionAttribute("МПТ")]
		MPT,

		[EnumMember]
		[DescriptionAttribute("ПИМ")]
		Pim,

		[EnumMember]
		[DescriptionAttribute("Пользователь прибора")]
		GkUser
	}
}