using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubezhResurs.OSI.ApplicationLayer;
using RubezhResurs.OSI.DataLinkLayer;
using RubezhResurs.OSI.Messages.Transaction;
using RubezhResurs.Devices;
using RubezhResurs.Devices.Collections.ObjectModel;
using RubezhResurs.Management;

namespace RubezhResurs.Incotex.NetworkControllers
{
    /// <summary>
    /// Сетевой контроллер для работы со устройствами производства Incotex
    /// </summary>
    public class IncotexNetworkController: NetworkControllerBase
    {
        #region Fields And Properties
        private static DeviceType[] _SupportedDevices = new DeviceType[] { DeviceType.Mercury203 };

        private Transaction _CurrentTransaction;
        /// <summary>
        /// Возвращает состояние текущей сетевой транзакции
        /// </summary>
        public Transaction CurrentTransaction
        {
            get { return _CurrentTransaction; }
        }
        public override IEnumerable<DeviceType> SuppotedDevices
        {
            get { return _SupportedDevices; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        public IncotexNetworkController()
        {
            _Devices = new DevicesCollection();
        }
        #endregion

        #region Methods
        #endregion

        #region Network API

        #endregion

        #region Events
        #endregion
    }
}
