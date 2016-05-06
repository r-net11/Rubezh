using System.ComponentModel;

namespace StrazhAPI.SKD
{
	public enum DocumentType
	{
		[Description("Переработка")]
		Overtime = 0,

		[Description("Присутствие")]
		Presence = 1,

		[Description("Отсутствие по неуважительной причине")]
		Absence = 2,

		[Description("Отсутствие по уважительной причине")]
		AbsenceReasonable = 3,
	}
}