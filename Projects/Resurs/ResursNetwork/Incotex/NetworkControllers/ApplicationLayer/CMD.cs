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
        SetNetworkAddress = 0,
        /// <summary>
        /// Установка нового группового адреса счетчика 
        /// </summary>
        SetGroupNetworkAddress = 1,
        /// <summary>
        /// Установка внутрен-них часов и кален-даря счетчика
        /// </summary>
        SetDateTime = 2,
        /// <summary>
        /// Установка лимита мощности
        /// </summary>
        SetLimitPower = 3,
        /// <summary>
        /// Установка лимита энергии за месяц
        /// </summary>
        SetLimitPowerPerMonth = 4,
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
        /// Установка числа действующих тари-фов
        /// </summary>
        SetActiveTariffs
    }
}
