using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI.Models
{
	public interface INetworksManager
	{
		#region Propeties

        /// <summary>
        /// Создаёт сеть на основе типа и добавляет в коллекцию
        /// </summary>
        /// <param name="network">Объект сети из БД</param>
		void AddNetwork(Device network);

		/// <summary>
		/// Удаляет сеть
		/// </summary>
		/// <param name="id">Идентификатор удаляемой сети</param>
		void RemoveNetwork(Guid id);

		/// <summary>
        /// Создаёт устройтсво на основе типа и добавляет в коллекцию
        /// </summary>
		void AddDevice(Device device);

		/// <summary>
        /// Удаляет указанное устройтсво из сети 
        /// </summary>
        /// <param name="id">Идентификатор удаляемого устройтсва</param>
		void RemoveDevice(Guid id);

		/// <summary>
        /// Устанавливает новое состояние сети/устройтсву
        /// </summary>
        /// <param name="id">Идентификатор сетевого контроллера или устройства</param>
        /// <param name="status">Новое состояние: false - выкл. true-вкл.</param>
		void SetSatus(Guid id, bool status);
		
		/// <summary>
        /// Синхронизирует дату и время устройтсв в указанной сети
        /// </summary>
        /// <param name="networkId"></param>
		/// <param name="broadcastAddress">Широковещательный адрес</param>
		/// <remarks>broadcastAddress: в Incotex-протоколе это это групповой адрес устройств </remarks>
		void SyncDateTime(Guid networkId, ValueType broadcastAddress);

		void SyncDateTime(Guid networkId);

		void SendCommand(Guid id, string commandName);

		/// <summary>
		/// Читает значение параметра из удалённого устройство
		/// асинхронно, обновление параметра по событию
		/// </summary>
		/// <param name="deviceId"></param>
		/// <param name="parameterName"></param>
		IOperationResult ReadParameter(Guid deviceId, string parameterName);

		/// <summary>
		/// Записывает новое значение параметра у удалённое устройство
		/// Блокирует вызывающий поток до окончания сетевой транзакции
		/// </summary>
		/// <param name="deviceId"></param>
		/// <param name="parameterName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		void WriteParameter(Guid deviceId, string parameterName, ValueType value);

		#endregion

		#region Events
		//event EventHandler<StatusChangedEventArgs> StatusChanged;
		//event EventHandler<ParameterChangedEventArgs> ParameterChanged;
		//event EventHandler<DeviceErrorOccuredEventArgs> DeviceHasError;
		//event EventHandler<NetworkControllerErrorOccuredEventArgs> NetworkControllerHasError;
		#endregion
	}
}
