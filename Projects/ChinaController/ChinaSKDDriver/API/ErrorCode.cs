using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ChinaSKDDriverAPI
{
    public enum ErrorCode
    {
        [Description("Нет ошибки")]
        NoError = 0,
        [Description("Несанкционированный")]
        Unauthorized = 16,
        [Description("Потеря карты или отмена")]
        CardLossOrCancellation = 17,
        [Description("Без разрешения ворота")]
        WithoutTheGatePermission = 18,
        [Description("Ошибка режима открытия")]
        OpenModeError = 19,
        [Description("Срок действия ошибки")]
        ValidityOfTheError = 20,
        [Description("")]
        AntiSubmarineMode = 21,
        [Description("")]
        DuressAlarmIsNotTurnedOn = 22,
        [Description("Двери обычно закрыты")]
        DoorsNormallyClosed = 23,
        [Description("")]
        AbInterlockStatus = 24,
        [Description("Патрульные карты")]
        PatrolCards = 25,
        [Description("Оборудование в состоянии тревоги вторжения")]
        EquipmentInIntrusionAlarmState = 26,
        [Description("Ошибка времени")]
        TimeError = 32,
        [Description("")]
        HolidaysMistakesWithinOpeningHours = 33,
        [Description("")]
        WeNeedToVerifyThatYouHaveTheFirstCardPrivilegesCard = 48,
        [Description("Метод открытия \"Сначала карта, потом пароль\". Ошибка в пароле")]
        CardFirstOpenMethodPasswordError = 64,
        [Description("Метод открытия \"Сначала карта, потом пароль\". Вышло время ввода пароля")]
        CardFirstOpenMethodPasswordTimeout = 65,
        [Description("Метод открытия \"Сначала карта, потом отпечаток пальца\". Ошибка отпечатка пальца")]
        CardFirstOpenMethodFingerprintError = 66,
        [Description("Метод открытия \"Сначала карта, потом отпечаток пальца\". Вышло время ввода отпечатка пальца")]
        CardFirstOpenMethodFingerprintTimeout = 67,
        [Description("Метод открытия \"Сначала отпечаток пальца, потом пароль\". Ошибка в пароле")]
        FingerprintFirstOpenMethodPasswordError = 68,
        [Description("Метод открытия \"Сначала отпечаток пальца, потом пароль\". Вышло время ввода пароля")]
        FingerprintFirstOpenMethodPasswordTimeout = 69,
        [Description("Неправильный порядок в комбинации открытия двери")]
        OpenDoorCombinationWrongOrder = 80,
        [Description("Требуется проверка комбинации открытия двери")]
        OpenDoorCombinationNeedsVerification = 81,
        [Description("Проверено. Требуется аутентификация на удаленной консоли")]
        VerifiedRemoteConsoleIsNotAuthorized = 96
    }
}
