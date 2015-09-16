using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubezhResurs.Management;
using RubezhResurs.Devices;
using RubezhResurs.Devices.Collections.ObjectModel;

namespace RubezhResurs.OSI.ApplicationLayer
{
    public interface INetwrokController: IManageable
    {
        #region Fields And Properties
        /// <summary>
        /// Уникальный идентификатор контроллера
        /// </summary>
        UInt32 ControllerId { get; }
        /// <summary>
        /// Список поддерживаемых данным контроллером типов устройств 
        /// </summary>
        IEnumerable<DeviceType> DevicesSuppoted { get; }
        /// <summary>
        /// Список устройств в сети.
        /// </summary>
        DevicesCollection Devices { get; }
        #endregion


    }
}
