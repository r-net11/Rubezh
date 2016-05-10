using System.ComponentModel;
using LocalizationConveters;

namespace StrazhDeviceSDK.API
{
    public enum ErrorCode
    {
		//[Description("Нет ошибки")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.ErrorCode), "None")]
		None = 0,
        //[Description("Неверный идентификатор")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.ErrorCode), "Unauthorized")]
		Unauthorized = 16,
        //[Description("Пропуск заблокирован")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.ErrorCode), "CardLostOrCancelled")]
		CardLostOrCancelled = 17,
        //[Description("Нет прав доступа")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.ErrorCode), "NoRight")]
		NoRight = 18,
        //[Description("Неверный метод открытия замка")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.ErrorCode), "UnlockModeError")]
		UnlockModeError = 19,
        //[Description("Срок действия пропуска истек или не наступил")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.ErrorCode), "ValidityError")]
		ValidityError = 20,
        //[Description("Повторный проход в зону")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.ErrorCode), "AntipassBack")]
		AntipassBack = 21,
        //[Description("Настройки замка не поддерживает пропуск 'Принуждение'")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.ErrorCode), "IntimidationAlarmNotOn")]
		IntimidationAlarmNotOn = 22,
        //[Description("Замок в режиме 'Закрыто'")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.ErrorCode), "DoorNcStatus")]
		DoorNcStatus = 23,
        //[Description("Открыта другая дверь шлюза")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.ErrorCode), "AbInterlockStatus")]
		AbInterlockStatus = 24,
		//[Description("Патрульные карты")]
		//PatrolCards = 25,
        //[Description("Замок в состоянии 'Взлом'")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.ErrorCode), "DeviceIsUnderIntrusionAlam")]
		DeviceIsUnderIntrusionAlam = 26,
        //[Description("Нарушение графика доступа")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.ErrorCode), "PeriodError")]
		PeriodError = 32,
		//[Description("Ошибочный временной диапазон для нерабочего дня")]
		//TimeRangeErrorInHoliday = 33,
		//[Description("")]
		//NeedToVerifyCardWhichHasFirstCardPrivilege = 48,
        //[Description("Неверный пароль пропуска")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.ErrorCode), "CardCorrectInputPasswordError")]
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
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.ErrorCode), "VerificationPassedControlNotAuthorized")]
		VerificationPassedControlNotAuthorized = 96
	}
}
