using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubezhResurs.Management;
using RubezhResurs.Devices;
using RubezhResurs.Devices.Collections.ObjectModel;
using RubezhResurs.OSI.DataLinkLayer;

namespace RubezhResurs.OSI.ApplicationLayer
{
    public interface INetwrokController: IManageable
    {
        #region Fields And Properties
        /// <summary>
        /// Уникальный идентификатор контроллера
        /// </summary>
        UInt32 ControllerId { get; set; }
        /// <summary>
        /// Список поддерживаемых данным контроллером типов устройств 
        /// </summary>
        IEnumerable<DeviceType> SuppotedDevices { get; }
        /// <summary>
        /// Список устройств в сети.
        /// </summary>
        DevicesCollection Devices { get; }
        /// <summary>
        /// Возвращает объет для работы с физическим интерфейсом
        /// </summary>
        IDataLinkPort Connection { get; }
        #endregion


    }
}
