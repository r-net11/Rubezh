using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

		//[Description("Патрульные карты")]
		//PatrolCards = 25,

		//[Description("Замок в состоянии 'Взлом'")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "DeviceIsUnderIntrusionAlam")]
        DeviceIsUnderIntrusionAlam = 26,

		//[Description("Нарушение графика доступа")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "PeriodError")]
        PeriodError = 32,

		//[Description("Ошибочный временной диапазон для нерабочего дня")]
		//TimeRangeErrorInHoliday = 33,

		//[Description("")]
		//NeedToVerifyCardWhichHasFirstCardPrivilege = 48,

		//[Description("Неверный пароль пропуска")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "CardCorrectInputPasswordError")]
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

		//[Description("Ожидание подтверждения прохода")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalErrorCode), "VerificationPassedControlNotAuthorized")]
        VerificationPassedControlNotAuthorized = 96
	}
}
