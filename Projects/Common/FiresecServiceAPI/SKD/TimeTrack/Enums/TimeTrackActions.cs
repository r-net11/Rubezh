using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	[Flags]
	public enum TimeTrackActions : byte
	{
		None = 0,

		[Description("Редактирование границ")]
		[EnumMember]
		EditBorders = 0x1,

		[Description("Добавление")]
		[EnumMember]
		Adding = 0x2,

		[Description("Удаление")]
		[EnumMember]
		Remove = 0x4,

		[Description("Принудительное закрытие")]
		[EnumMember]
		ForceClose = 0x8,

		[Description("Отключение флага Не учитывать в расчетах'")]
		[EnumMember]
		TurnOffCalculation = 0x10,

		[Description("Включение флага 'Не учитывать в расчетах'")]
		[EnumMember]
		TurnOnCalculation = 0x20
	}
}
