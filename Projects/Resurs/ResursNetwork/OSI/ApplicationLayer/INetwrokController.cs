using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.Management;
using ResursNetwork.OSI.ApplicationLayer.Devices;
using ResursNetwork.OSI.ApplicationLayer.Devices.Collections.ObjectModel;
using ResursNetwork.OSI.DataLinkLayer;
using ResursNetwork.OSI.Messages;
using ResursNetwork.OSI.Messages.Transactions;
using ResursAPI.Models;

namespace ResursNetwork.OSI.ApplicationLayer
{
    public interface INetwrokController: IManageable, IDisposable
    {
        #region Fields And Properties

        /// <summary>
        /// Уникальный идентификатор контроллера
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Список поддерживаемых данным контроллером типов устройств 
        /// </summary>
        IEnumerable<DeviceModel> SuppotedDevices { get; }

        /// <summary>
        /// Список устройств в сети.
        /// </summary>
        DevicesCollection Devices { get; }

		/// <summary>
        /// Период (мсек) опроса и получения данных от удалённых устройств
        /// </summary>
		int PollingPeriod { get; set; }

        /// <summary>
        /// Возвращает объет для работы с физическим интерфейсом
        /// </summary>
        IDataLinkPort Connection { get; set; }

		NetworkControllerErrors Errors { get; }

        #endregion

        #region Methods

		/// <summary>
		/// Читает параметр из сетевого контроллера
		/// </summary>
		/// <param name="parameterName"></param>
		OperationResult ReadParameter(string parameterName);

		/// <summary>
		/// Записывает параметр сетевого контроллера (настройки и т.п)
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="value"></param>
		void WriteParameter(string parameterName, ValueType value);  

        /// <summary>
        /// Записывает транзакцию в буфер исходящих сообщений
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isExternalCall">
        /// Признак, что вызов записи в буфер контроллера проиходит
        /// из внешнего источника (например GUI)
        /// </param>
        IAsyncRequestResult Write(NetworkRequest request, bool isExternalCall);

        /// <summary>
        /// Отсылает в сеть широковещательную команду 
        /// синхронизации времени
        /// </summary>
        void SyncDateTime();

		/// <summary>
		/// Выполняет команду 
		/// </summary>
		/// <param name="commandName"></param>
		void ExecuteCommand(string commandName);

        #endregion

        #region

		event EventHandler<NetworkRequestCompletedArgs> NetwrokRequestCompleted;
		event EventHandler<ParameterChangedEventArgs> ParameterChanged;
		event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;
		event EventHandler<NetworkControllerErrorOccuredEventArgs> ErrorOccurred;
        
        #endregion
    }
}
