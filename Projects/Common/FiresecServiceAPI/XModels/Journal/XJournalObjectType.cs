using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public enum XJournalObjectType
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