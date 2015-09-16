using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubezhResurs.Devices.Collections.ObjectModel;

namespace RubezhResurs.Devices
{
    /// <summary>
    /// Реализует базовые компонеты счётчика электроэнергии
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Возвращает тип устройства (счётчика электро)
        /// </summary>
        DeviceType DeviceType { get; }
        /// <summary>
        /// Сетевой адрес устройства
        /// </summary>
        UInt32 Address { get; }
        /// <summary>
        /// Возвращает коллекцию описания параметров устройства
        /// </summary>
        ParatemersCollection Parameters { get; }
    }
}
