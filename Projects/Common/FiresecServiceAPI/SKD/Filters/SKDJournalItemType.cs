using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public enum SKDJournalItemType
	{
		[EnumMember]
		[DescriptionAttribute("Устройства ГК")]
		GKDevice,

		[EnumMember]
		[DescriptionAttribute("Зоны ГК")]
		GKZone,

		[EnumMember]
		[DescriptionAttribute("Направления ГК")]
		GKDirection,

		[EnumMember]
		[DescriptionAttribute("МПТ ГК")]
		GKMPT,

		[EnumMember]
		[DescriptionAttribute("Насосные станции ГК")]
		GKPumpStation,

		[EnumMember]
		[DescriptionAttribute("Задержки ГК")]
		GKDelay,

		[EnumMember]
		[DescriptionAttribute("Устройство СКД")]
		SKDDevice,

		[EnumMember]
		[DescriptionAttribute("Зона СКД")]
		SKDZone,

		[EnumMember]
		[DescriptionAttribute("Видеоустройство")]
		VideoDevice,
	}
}