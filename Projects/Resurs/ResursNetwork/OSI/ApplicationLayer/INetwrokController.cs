using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.Management;
using ResursNetwork.Devices;
using ResursNetwork.Devices.Collections.ObjectModel;
using ResursNetwork.OSI.DataLinkLayer;
using ResursNetwork.OSI.Messages.Transaction;

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
        /// <param name="transaction"></param>
        void Write(Transaction transaction);
        #endregion

    }
}
