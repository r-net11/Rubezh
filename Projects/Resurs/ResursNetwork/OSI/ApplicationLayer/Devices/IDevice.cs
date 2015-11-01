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
        DeviceModel DeviceModel { get; }
        
        /// <summary>
        /// Сетевой адрес устройства
        /// </summary>
		UInt32 Address { get; set; }
        
        /// <summary>
        /// Сетевой контроллер, владелец данного устройства
        /// </summary>
		INetwrokController Network { get; set; }
        
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

        bool RtcError { get; set; }

        /// <summary>
        /// Дата и время в удалённом устройтсве 
        /// </summary>
        DateTime Rtc { get; set; }

        #endregion

        #region Methods
		
		/// <summary>
		/// Выполняет комманду 
		/// </summary>
		/// <param name="commandName">Команда</param>
		void ExecuteCommand(string commandName);

		/// <summary>
		/// Читает параметр из удалённого устройства
		/// </summary>
		/// <param name="parameterName"></param>
		OperationResult ReadParameter(string parameterName);

		/// <summary>
		/// Записывает параметр в удалённом устройстве
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="value"></param>
		OperationResult WriteParameter(string parameterName, ValueType value);        
		
		#endregion

        #region Events

        /// <summary>
        /// Возникает при ошибках в устройтве
        /// </summary>
        event EventHandler<DeviceErrorOccuredEventArgs> ErrorOccurred;

        #endregion
    }
}
