using System.ComponentModel;
using LocalizationConveters;

namespace StrazhAPI.SKD
{
	public enum DocumentType
	{
		//[Description("Переработка")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.DocumentType), "Overtime")]
		Overtime = 0,

        //[Description("Присутствие")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.DocumentType), "Presence")]
		Presence = 1,

        //[Description("Отсутствие по неуважительной причине")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.DocumentType), "Absence")]
		Absence = 2,

        //[Description("Отсутствие по уважительной причине")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.DocumentType), "AbsenceReasonable")]
		AbsenceReasonable = 3,
	}
}