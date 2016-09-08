using System.ComponentModel;
using Localization.Converters;
using Localization.StrazhDeviceSDK.Common;

namespace StrazhDeviceSDK.API
{
    public enum ErrorCode
    {
		//[Description("Нет ошибки")]
		[LocalizedDescription(typeof(CommonResources), "NoError")]
		None = 0,
		//[Description("Неверный идентификатор")]
		[LocalizedDescription(typeof(CommonResources), "WrongId")]
		Unauthorized = 16,
		//[Description("Пропуск заблокирован")]
		[LocalizedDescription(typeof(CommonResources), "PasscardBlocked")]
		CardLostOrCancelled = 17,
		//[Description("Нет прав доступа")]
		[LocalizedDescription(typeof(CommonResources), "NoAccessPermissions")]
		NoRight = 18,
		//[Description("Неверный метод открытия замка")]
		[LocalizedDescription(typeof(CommonResources), "IncorectLockOpenMode")]
		UnlockModeError = 19,
		//[Description("Срок действия пропуска истек или не наступил")]
		[LocalizedDescription(typeof(CommonResources), "PasscardValidityExpired")]
		ValidityError = 20,
		//[Description("Повторный проход в зону")]
		[LocalizedDescription(typeof(CommonResources), "PassbackToZone")]
		AntipassBack = 21,
		//[Description("Настройки замка не поддерживает пропуск 'Принуждение'")]
		[LocalizedDescription(typeof(CommonResources), "LockSettingNotSupportForcing")]
		IntimidationAlarmNotOn = 22,
		//[Description("Замок в режиме 'Закрыто'")]
		[LocalizedDescription(typeof(CommonResources), "LockClosed")]
		DoorNcStatus = 23,
		//[Description("Открыта другая дверь шлюза")]
		[LocalizedDescription(typeof(CommonResources), "AnotherDoorOpen")]
		AbInterlockStatus = 24,
		//[Description("Патрульные карты")]
		//PatrolCards = 25,
		//[Description("Замок в состоянии 'Взлом'")]
		[LocalizedDescription(typeof(CommonResources), "LockBreaking")]
		DeviceIsUnderIntrusionAlam = 26,
		//[Description("Нарушение графика доступа")]
		[LocalizedDescription(typeof(CommonResources), "AccessScheduleOffense")]
		PeriodError = 32,
		//[Description("Ошибочный временной диапазон для нерабочего дня")]
		//TimeRangeErrorInHoliday = 33,
		//[Description("")]
		//NeedToVerifyCardWhichHasFirstCardPrivilege = 48,
		//[Description("Неверный пароль пропуска")]
		[LocalizedDescription(typeof(CommonResources), "IncorrectPasscardPassword")]
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
		[LocalizedDescription(typeof(CommonResources), "WaitPassConfirm")]
		VerificationPassedControlNotAuthorized = 96
	}
}
