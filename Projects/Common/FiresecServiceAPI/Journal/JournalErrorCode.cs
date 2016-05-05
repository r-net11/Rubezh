using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

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
		//[Description("Патрульные карты")]
		//PatrolCards = 25,
		[Description("Замок в состоянии 'Взлом'")]
		DeviceIsUnderIntrusionAlam = 26,
		[Description("Нарушение графика доступа")]
		PeriodError = 32,
		//[Description("Ошибочный временной диапазон для нерабочего дня")]
		//TimeRangeErrorInHoliday = 33,
		//[Description("")]
		//NeedToVerifyCardWhichHasFirstCardPrivilege = 48,
		[Description("Неверный пароль пропуска")]
		CardCorrectInputPasswordError = 64,
		//[Description("Карта верная, вышло время ввода пароля")]
		//CardCorrectPasswordInputTimeout = 65,
		//[Description("Карта верная, ошибка отпечатка пальца")]
		//CardCorrectFingerprintError = 66,
		//[Description("Карта верная, вышло время ввода отпечатка пальца")]
		//CardCorrectFingerprintInputTimeout = 67,
		//[Description("Отпечаток пальца верный, ошибка в пароле")]
		//FingerprintCorrectPasswordError = 68,
		//[Description("Отпечаток пальца верный, вышло время ввода пароля")]
		//FingerprintCorrectPasswordInputTimeout = 69,
		//[Description("Ошибка при совместной попытке открыть дверь")]
		//CombinedOrderToOpenTheDoorError = 80,
		//[Description("Требуется проверка при совместной попытке открыть дверь")]
		//CombinedOrderToOpenTheDoorNeedVerified = 81,
		[Description("Ожидание подтверждения прохода")]
		VerificationPassedControlNotAuthorized = 96
	}
}
