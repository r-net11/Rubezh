using System.ComponentModel;
using LocalizationConveters;

namespace StrazhAPI.Journal
{
	public enum JournalErrorCode
	{
		//[Decription("Нет ошибки")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "None")]
		None = 0,

		//[Decription("Неверный идентификатор")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "Unauthorized")]
        Unauthorized = 16,

		//[Decription("Пропуск заблокирован")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "CardLostOrCancelled")]
        CardLostOrCancelled = 17,

		//[Decription("Нет прав доступа")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "NoRight")]
        NoRight = 18,

		//[Decription("Неверный метод открытия замка")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "UnlockModeError")]
        UnlockModeError = 19,

		//[Decription("Срок действия пропуска истек или не наступил")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "ValidityError")]
        ValidityError = 20,

		//[Decription("Повторный проход в зону")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "AntipassBack")]
        AntipassBack = 21,

		//[Decription("Настройки замка не поддерживает пропуск 'Принуждение'")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "IntimidationAlarmNotOn")]
        IntimidationAlarmNotOn = 22,

		//[Decription("Замок в режиме 'Закрыто'")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "DoorNcStatus")]
        DoorNcStatus = 23,

		//[Decription("Открыта другая дверь шлюза")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "AbInterlockStatus")]
        AbInterlockStatus = 24,


		//[Description("Замок в состоянии 'Взлом'")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "DeviceIsUnderIntrusionAlam")]
        DeviceIsUnderIntrusionAlam = 26,

		//[Description("Нарушение графика доступа")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "PeriodError")]
        PeriodError = 32,


		//[Description("Неверный пароль пропуска")]
		[LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "CardCorrectInputPasswordError")]
		CardCorrectInputPasswordError = 64,


		//[Description("Ожидание подтверждения прохода")]
		[LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "VerificationPassedControlNotAuthorized")]
		VerificationPassedControlNotAuthorized = 96
	}
}
