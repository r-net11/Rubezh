using Localization;
using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum PendingCardAction
	{
        //[Description("Добавление")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Card.PendingCardAction), "Add")]
		Add,

        //[Description("Редактирование")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Card.PendingCardAction), "Edit")]
		Edit,

        //[Description("Удаление")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Card.PendingCardAction), "Delete")]
		Delete,

        //[Description("Сброс антипессбэка")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Card.PendingCardAction), "ResetRepeatEnter")]
		ResetRepeatEnter,
	}
}