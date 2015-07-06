using System.ComponentModel;

namespace ChinaSKDDriverAPI
{
	public enum ErrorCode
	{
		[Description("Нет ошибки")]
		NoError = 0,

		[Description("Нет авторизации")]
		NotAuthorized = 16,

		[Description("Потеря карты или отмена")]
		CardLogOffOrReportLoss = 17,

		[Description("Нет разрешения на эту дверь")]
		NoRightToThisDoor = 18,

		[Description("Ошибка режима открытия двери")]
		DoorOpeningModeError = 19,

		[Description("Ошибка даты валидации")]
		ValidDateError = 20,

		[Description("")]
		AntiPassbackMode = 21,

		[Description("")]
		DuressAlarmIsNotTurnedOn = 22,

		[Description("Двери обычно закрыты")]
		DoorsNormallyClosed = 23,

		[Description("")]
		AbInterlockStatus = 24,

		[Description("Патрульные карты")]
		PatrolCards = 25,

		[Description("Состояние тревоги из-за взлома")]
		AlarmStateBecauseOfBreakIn = 26,

		[Description("Ошибочный временной диапазон")]
		TimeError = 32,

		[Description("Ошибочный временной диапазон для нерабочего дня")]
		TimeRangeErrorInHoliday = 33,

		[Description("")]
		NeedToVerifyCardWhichHasFirstCardPrivilege = 48,

		[Description("Карта верная, ошибка в пароле")]
		CardRightPasswordError = 64,

		[Description("Карта верная, вышло время ввода пароля")]
		CardRightPasswordInputTimeout = 65,

		[Description("Карта верная, ошибка отпечатка пальца")]
		CardRightFingerprintError = 66,

		[Description("Карта верная, вышло время ввода отпечатка пальца")]
		CardRightFingerprintInputTimeout = 67,

		[Description("Отпечаток пальца верный, ошибка в пароле")]
		FingerprintRightPasswordError = 68,

		[Description("Отпечаток пальца верный, вышло время ввода пароля")]
		FingerprintRightPasswordInputTimeout = 69,

		[Description("Ошибка при совместной попытке открыть дверь")]
		CombinedOrderToOpenTheDoorError = 80,

		[Description("Требуется проверка при совместной попытке открыть дверь")]
		CombinedOrderToOpenTheDoorNeedVerified = 81,

		[Description("Проверено. Требуется аутентификация на удаленной консоли")]
		VerifiedRemoteConsoleIsNotAuthorized = 96
	}
}