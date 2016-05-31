using System.ComponentModel;

namespace StrazhAPI.Journal
{
	public enum JournalErrorCode
	{
		[Description("Нет ошибки")]
		None = 0,
		[Description("Неверный идентификатор")]
		Unauthorized = 16,
		[Description("Пропуск заблокирован")]
		CardLostOrCancelled = 17,
		[Description("Нет прав доступа")]
		NoRight = 18,
		[Description("Неверный метод открытия замка")]
		UnlockModeError = 19,
		[Description("Срок действия пропуска истек или не наступил")]
		ValidityError = 20,
		[Description("Повторный проход в зону")]
		AntipassBack = 21,
		[Description("Настройки замка не поддерживает пропуск 'Принуждение'")]
		IntimidationAlarmNotOn = 22,
		[Description("Замок в режиме 'Закрыто'")]
		DoorNcStatus = 23,
		[Description("Открыта другая дверь шлюза")]
		AbInterlockStatus = 24,
		[Description("Замок в состоянии 'Взлом'")]
		DeviceIsUnderIntrusionAlam = 26,
		[Description("Нарушение графика доступа")]
		PeriodError = 32,
		[Description("Неверный пароль пропуска")]
		CardCorrectInputPasswordError = 64,
		[Description("Ожидание подтверждения прохода")]
		VerificationPassedControlNotAuthorized = 96
	}
}
