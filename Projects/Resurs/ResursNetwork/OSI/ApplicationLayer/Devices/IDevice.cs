using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ResursNetwork.OSI.ApplicationLayer.Devices.Collections.ObjectModel;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.Management;
using ResursAPI.Models;

namespace ResursNetwork.OSI.ApplicationLayer.Devices
{
    /// <summary>
    /// Реализует базовые компонеты счётчика электроэнергии
    /// </summary>
    public interface IDevice: IManageable, INotifyPropertyChanged
    {
        #region Properties
        
        Guid Id { get; set; }

        /// <summary>
        /// Возвращает тип устройства (счётчика электро)
        /// </summary>
        DeviceType DeviceType { get; }
        
        /// <summary>
        /// Сетевой адрес устройства
        /// </summary>
        UInt32 Address { get; }
        
        /// <summary>
        /// Сетевой контроллер, владелец данного устройства
        /// </summary>
        INetwrokController Network { get; }
        
        /// <summary>
        /// Возвращает коллекцию описания параметров устройства
        /// </summary>
        ParatemersCollection Parameters { get; }

        /// <summary>
        /// Регистр состояния (ошибок) устройства
        /// </summary>
        DeviceErrors Errors { get; }

        bool CommunicationError { get; set; }

        bool ConfigurationError { get; set; }

        bool RTCError { get; set; }
                    
        #endregion

        #region Methods
        #endregion

        #region Events

        /// <summary>
        /// Возникает при ошибках в устройтве
        /// </summary>
        event EventHandler<ErrorOccuredEventArgs> ErrorOccurred;

        #endregion
    }
}
