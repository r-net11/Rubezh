using System.ComponentModel;
using LocalizationConveters;

namespace FiresecAPI.SKD
{
	public enum TimeTrackType
	{
		//[Description("Нет данных")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackType), "None")]
		None,

        //[Description("Баланс")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackType), "Balance")]
		Balance,

        //[Description("Явка")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackType), "Presence")]
		Presence,

        //[Description("Отсутствие")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackType), "Absence")]
		Absence,

        //[Description("Опоздание")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackType), "Late")]
		Late,

        //[Description("Уход раньше")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackType), "EarlyLeave")]
		EarlyLeave,

        //[Description("Сверхурочно")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackType), "Overtime")]
		Overtime,

        //[Description("Работа ночью")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackType), "Night")]
		Night,

        //[Description("Выходной")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackType), "DayOff")]
		DayOff,

        //[Description("Праздничный день")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackType), "Holiday")]
		Holiday,

        //[Description("Переработка по документу")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackType), "DocumentOvertime")]
		DocumentOvertime,

        //[Description("Присутствие по документу")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackType), "DocumentPresence")]
		DocumentPresence,

        //[Description("Отсутствие по неуважительной причине")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackType), "DocumentAbsence")]
		DocumentAbsence,

        //[Description("Отсутствие по уважительной причине")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.TimeTrackType), "DocumentAbsenceReasonable")]
		DocumentAbsenceReasonable,
	}
}