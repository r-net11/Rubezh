using System.ComponentModel;

namespace StrazhAPI.SKD
{
	public enum PendingCardAction
	{
		[Description("Добавление")]
		Add,

		[Description("Редактирование")]
		Edit,

		[Description("Удаление")]
		Delete,

		[Description("Сброс антипессбэка")]
		ResetRepeatEnter,
	}
}