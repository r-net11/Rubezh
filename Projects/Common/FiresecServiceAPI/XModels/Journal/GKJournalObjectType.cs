using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Тип объекта ГК
	/// </summary>
	[DataContract]
	public enum GKJournalObjectType
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
		[DescriptionAttribute("Охранная Зона")]
		GuardZone,

		[EnumMember]
		[DescriptionAttribute("Точка доступа")]
		Door,

		[EnumMember]
		[DescriptionAttribute("Пользователь прибора")]
		GkUser
	}
}