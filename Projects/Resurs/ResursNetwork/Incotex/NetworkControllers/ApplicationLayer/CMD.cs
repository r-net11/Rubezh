using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursNetwork.Incotex.NetworkControllers.ApplicationLayer
{
    /// <summary>
    /// Коды команд Mercury 203
    /// </summary>
    public enum Mercury203CmdCode: ushort
    {
        /// <summary>
        /// Установка нового сетевого адреса счетчика 
        /// </summary>
        WriteAddress = 0,
        /// <summary>
        /// Установка нового группового адреса счетчика 
        /// </summary>
        WriteGroupAddress = 1,
		/// <summary>
		/// Установка внутренних часов и календаря счетчика
		/// </summary>
		WriteDateTime = 0x02,
        /// <summary>
        /// Установка лимита мощности
        /// </summary>
        WriteLimitPower = 3,
        /// <summary>
        /// Установка лимита энергии за месяц
        /// </summary>
        WriteLimitPowerPerMonth = 4,
        /// <summary>
        /// Установка флага сезонного времени 
        /// </summary>
        SetPeriodTimeFlag = 5,
        /// <summary>
        /// Установка величины коррекции времени
        /// </summary>
        SetTimeCorrection = 6,
        /// <summary>
        /// Установка функции выходного оптрона
        /// </summary>
        SetOptronFunction = 7,
        /// <summary>
        /// Установка скорости обмена 
        /// </summary>
        SetBaudrate = 8,
        /// <summary>
        /// Установка режима индикации
        /// </summary>
        SetIndicationMode = 9,
        /// <summary>
        /// Установка числа действующих тарифов
        /// </summary>
        WriteAmountOfActiveTariffs = 0x0A,
		/// <summary>
		/// Установка действующего тарифа
		/// </summary>
		WriteActiveTariff = 0x0B,
        /// <summary>
        /// Чтение группового адреса счетчика
        /// </summary>
        ReadGroupAddress = 0x20,
        /// <summary>
        /// Чтение внутренних часов и календаря счетчика
        /// </summary>
        ReadDateTime = 0x21,
        /// <summary>
        /// Чтение лимита мощности
        /// </summary>
        ReadPowerLimit = 0x22,
        /// <summary>
        /// Чтение лимита энергии за месяц
        /// </summary>
        ReadPowerLimitPerMonth = 0x23,
        /// <summary>
        /// Чтение флага сезонного времени 
        /// </summary>
        ReadPeriodTimeFlag = 0x24,
        /// <summary>
        /// Чтение величины коррекции времени 
        /// </summary>
        ReadCorrectionTimeValue = 0x25,
        /// <summary>
        /// Чтение текущей мощности в нагрузке
        /// </summary>
        ReadCurrentPowerLoad = 0x26,
        /// <summary>
        /// Чтение содержимого тарифных аккумуляторов
        /// </summary>
        ReadTariffAccumulators = 0x27,
        /// <summary>
        /// Чтение идентификационных данных счетчика
        /// </summary>
        ReadDeviceId = 0x28,
        /// <summary>
        /// Чтение напряжения на литиевой батарее
        /// </summary>
        ReadBattaryVoltage = 0x29,
        /// <summary>
        /// Чтение режима индикации
        /// </summary>
        ReadIndicationMode = 0x2A,
        /// <summary>
        /// Чтение времени последнего отключения напряжения
        /// </summary>
        ReadLastPowerFailure = 0x21,
    }
}
