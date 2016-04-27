using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Localization;

namespace FiresecAPI.SKD
{
	[DataContract]
	[Flags]
	public enum TimeTrackActions : byte
	{
		None = 0,

		//[Description("Редактирование границ")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackActions), "EditBorders")]
		[EnumMember]
		EditBorders = 0x1,

        //[Description("Добавление")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackActions), "Adding")]
		[EnumMember]
		Adding = 0x2,

        //[Description("Удаление")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackActions), "Remove")]
		[EnumMember]
		Remove = 0x4,

        //[Description("Принудительное закрытие")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackActions), "ForceClose")]
		[EnumMember]
		ForceClose = 0x8,

        //[Description("Отключение флага Не учитывать в расчетах'")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackActions), "TurnOffCalculation")]
		[EnumMember]
		TurnOffCalculation = 0x10,

        //[Description("Включение флага 'Не учитывать в расчетах'")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackActions), "TurnOnCalculation")]
		[EnumMember]
		TurnOnCalculation = 0x20
	}
}
