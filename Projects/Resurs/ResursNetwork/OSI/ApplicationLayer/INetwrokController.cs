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

namespace ResursNetwork.OSI.ApplicationLayer
{
    public class NetworkRequestCompletedArgs : EventArgs
    {
        /// <summary>
        /// Завершённый сетевой запрос
        /// </summary>
        public NetworkRequest NetworkRequest;
    }

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

        #region Methods

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

        #endregion

        #region
        event EventHandler<NetworkRequestCompletedArgs> NetwrokRequestCompleted;
        #endregion
    }
}
