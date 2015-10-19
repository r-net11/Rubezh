using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ResursNetwork.Management;
using ResursNetwork.Networks.Collections.ObjectModel;
using ResursNetwork.OSI.ApplicationLayer.Devices;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.OSI.DataLinkLayer;
using ResursNetwork.Incotex.Models;
using ResursNetwork.Incotex.NetworkControllers.ApplicationLayer;
using ResursNetwork.Incotex.NetworkControllers.DataLinkLayer;
using ResursAPI;
using ResursAPI.ParameterNames;

namespace ResursNetwork.Networks
{
    /// <summary>
    /// Объет упрвления сетями системы учёта русурсов
    /// </summary>
    public class NetworksManager
    {
        #region Fields And Properties

        private NetworksCollection _NetworkControllers;
        private static Object _SyncRoot = new Object();
        private static NetworksManager _Instance;

        /// <summary>
        /// Возвращает менеджер сетей
        /// </summary>
        public static NetworksManager Instance
        {
            get 
            {
                if (_Instance == null)
                {
                    lock(_SyncRoot)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new NetworksManager();
                        }
                    }
                }
                return _Instance;
            }
        }

        /// <summary>
        /// Список сетевых контроллеров (интерфейсов)
        /// </summary>
        public NetworksCollection Networks
        {
            get { return _NetworkControllers; }
        }
        #endregion

        #region Constructors

        public NetworksManager()
        {
            _NetworkControllers = new NetworksCollection();
            _NetworkControllers.CollectionChanged += 
                EventHandler_NetworkControllers_CollectionChanged;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Создаёт сеть на основе типа и добавляет в коллекцию
        /// </summary>
        /// <param name="network">Объект сети из БД</param>
        public void AddNetwork(ResursAPI.Device network)
        {
            switch (network.DeviceType)
            {
                case ResursAPI.DeviceType.Network:
                    {
                        switch(network.DriverType)
                        {
                            case ResursAPI.DriverType.IncotextNetwork:
                                {
                                    var incotexPort = new Incotex.NetworkControllers.DataLinkLayer.ComPort
                                    {
                                        BaudRate = (int)network.GetParameter(
                                            ParameterNamesIncotexNetwork.BautRate),
                                        PortName = ((ParameterStringContainer)network.GetParameter(
                                            ParameterNamesIncotexNetwork.PortName)).GetValue()
                                    };

                                    var incotexController = new IncotexNetworkController
                                    {
                                        Id = (Guid)network.GetParameter(ParameterNamesIncotexNetwork.Id),
                                        Connection = (IDataLinkPort)incotexPort,
                                        BroadcastRequestDelay = (int)network.GetParameter(
                                            ParameterNamesIncotexNetwork.BroadcastDelay),
                                        RequestTimeout = (int)network.GetParameter(
                                            ParameterNamesIncotexNetwork.Timeout)
                                    };

                                    _NetworkControllers.Add((INetwrokController)incotexController);

                                    if (network.IsActive)
                                    { 
                                        incotexController.Start(); 
                                    }
                                    else
                                    {
                                        incotexController.Stop(); 
                                    }

                                    break; 
                                }
                            default:
                                {
                                    throw new NotSupportedException(String.Format(
                                        "Устройтсво с DriverType={0} не поддерживается", 
                                        network.DriverType.ToString()));
                                }
                        }
                        break; 
                    }
                case ResursAPI.DeviceType.Counter:
                    { 
                        throw new InvalidCastException(
                            "Попытка приветсти счётчик эл энергии к контроллеру сети"); 
                    }
                default:
                    {
                        throw new InvalidCastException(String.Format(
                          "Неудалось привести типы. Устройтсво с DeviceType={0} не поддерживается",
                          network.DeviceType.ToString()));
                    }
            }
        }

		/// <summary>
		/// Удаляет сеть
		/// </summary>
		/// <param name="id">Идентификатор удаляемой сети</param>
		public void RemoveNetwork(Guid id)
		{
			_NetworkControllers.Remove(id);
		}

        /// <summary>
        /// Создаёт устройтсво на основе типа и добавляет в коллекцию
        /// </summary>
        public void AddDevice(ResursAPI.Device device)
        {
            switch (device.DeviceType)
            {
                case ResursAPI.DeviceType.Network:
                    {
                        throw new InvalidCastException(
                            "Попытка приветсти к контроллер сети к устройтсву");
                    }
                case ResursAPI.DeviceType.Counter:
                    {
                        switch (device.DriverType)
                        {
                            case ResursAPI.DriverType.Mercury203Counter:
                                {
                                    var mercury203 = new Mercury203
                                    {
                                        Id = (Guid)device.GetParameter(ParameterNamesMercury203.Id),
                                        Address = (UInt32)device.GetParameter(ParameterNamesMercury203.Address),
                                        Status = device.IsActive ? Status.Running : Status.Stopped
                                    };

                                    mercury203.Parameters[ParameterNamesMercury203.GADDR].Value =
                                        device.GetParameter(ParameterNamesMercury203.GADDR);
                                    mercury203.Parameters[ParameterNamesMercury203.PowerLimit].Value =
                                        device.GetParameter(ParameterNamesMercury203.PowerLimit);
                                    mercury203.Parameters[ParameterNamesMercury203.PowerLimitPerMonth].Value =
                                        device.GetParameter(ParameterNamesMercury203.PowerLimitPerMonth);
                                    //TODO: Сделать таблицу параметров доконца

                                    var owner = device.Parent;
                                    
                                    if ((owner == null) || 
                                        (owner.DriverType != DriverType.IncotextNetwork))
                                    {
                                        throw new InvalidOperationException(
                                            "Невозможно добавить устройтсво. Владельцем устройства не является IncotextNetwork");
                                    }

                                    _NetworkControllers[(Guid)owner.GetParameter(ParameterNamesBase.Id)]
                                        .Devices.Add(mercury203);
                                    break;
                                }
                            default:
                                {
                                    throw new NotSupportedException(String.Format(
                                        "Устройтсво с DriverType={0} не поддерживается",
                                        device.DriverType.ToString()));
                                }
                        }
                        break;
                    }
                default:
                    {
                        throw new InvalidCastException(String.Format(
                          "Неудалось привести типы. Устройтсво с DeviceType={0} не поддерживается",
                          device.DeviceType.ToString()));
                    }
            }
        }

        /// <summary>
        /// Удаляет указанное устройтсво из сети 
        /// </summary>
        /// <param name="id">Идентификатор удаляемого устройтсва</param>
        public void RemoveDevice(Guid id)
        {
            foreach(var network in Networks)
            {
                var device = network.Devices.FirstOrDefault(p => p.Id == id);

                if (device != null)
                {
                    network.Devices.Remove(device);
                }
            }
        }
        
        /// <summary>
        /// Устанавливает новое состояние сети/устройтсву
        /// </summary>
        /// <param name="id">Идентификатор сетевого контроллера или устройства</param>
        /// <param name="status">Новое состояние: false - выкл. true-вкл.</param>
        public void SetSatus(Guid id, bool status)
        {
            var network = Networks.FirstOrDefault(p => p.Id == id);
            
            if (network != null)
            {
                network.Status = status ? Status.Running : Status.Stopped;
                return;
            }

            var list = from n in Networks
                    from d in n.Devices
                    where d.Id == id
                    select d;
            var device = list.FirstOrDefault();
            
            if (device != null)
            {
                device.Status = status ? Status.Running : Status.Stopped;
            }

        }

        /// <summary>
        /// Синхронизирует дату и время устройтсв в указанной сети
        /// </summary>
        /// <param name="networkId"></param>
        public void SyncDateTime(Guid networkId)
        {
            _NetworkControllers[networkId].SyncDateTime();
        }

        /// <summary>
        /// Обработчик события изменения списка сетей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_NetworkControllers_CollectionChanged(
            object sender, NetworksCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        e.Network.StatusChanged += EventHandler_Network_StatusChanged;
						e.Network.ParameterChanged += EventHandler_Network_ParameterChanged;
                        
                        foreach(var device in e.Network.Devices)
                        {
                            device.StatusChanged += EventHandler_Device_StatusChanged;
                            device.PropertyChanged += EventHandler_Device_PropertyChanged;
                            device.ErrorOccurred += EventHandler_Device_ErrorOccurred;
                        }
                        break; 
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        e.Network.StatusChanged -= EventHandler_Network_StatusChanged;
						e.Network.ParameterChanged -= EventHandler_Network_ParameterChanged;

                        foreach (var device in e.Network.Devices)
                        {
                            device.StatusChanged -= EventHandler_Device_StatusChanged;
                            device.PropertyChanged -= EventHandler_Device_PropertyChanged;
                            device.ErrorOccurred -= EventHandler_Device_ErrorOccurred;
                        }
                        break; 
                    }
                default: { throw new NotSupportedException(); }
            }
        }

		private void EventHandler_Network_ParameterChanged(object sender, ParameterChangedArgs e)
		{
			// Прокидываем событие дальше
			OnParameterChanged(e);
		}

        private void EventHandler_Device_ErrorOccurred(object sender, ErrorOccuredEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EventHandler_Device_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var device = (IDevice)sender;
            //device.Parameters[ e.PropertyName
        }

        private void EventHandler_Device_StatusChanged(object sender, EventArgs e)
        {
            var device = (IDevice)sender;
            //OnDeviceChangedStatus(device.Id, device.Status);
            OnStatusChanged(device.Id, device.Status);
        }

        private void EventHandler_Network_StatusChanged(object sender, EventArgs e)
        {
            var network = (INetwrokController)sender;
            //OnNetwokChangedStatus(network.Id, network.Status);
            OnStatusChanged(network.Id, network.Status);
        }

        //private void OnNetwokChangedStatus(Guid id, Status newStatus)
        //{
        //    var handler = this.NetworkChangedStatus;

        //    if (handler != null)
        //    {
        //        var args = new StatusChangedEventArgs { Id = id, Status = newStatus };
        //        handler(this, args);
        //    }
        //}

        //private void OnDeviceChangedStatus(Guid id, Status newStatus)
        //{
        //    var handler = this.DeviceChangedStatus;

        //    if (handler != null)
        //    {
        //        var args = new StatusChangedEventArgs { Id = id, Status = newStatus };
        //        handler(this, args);
        //    }
        //}

        private void OnStatusChanged(Guid id, Status newStatus)
        {
            var handler = this.StatusChanged;

            if (handler != null)
            {
                var args = new StatusChangedEventArgs { Id = id, Status = newStatus };
                handler(this, args);
            }
        }

		private void OnParameterChanged(ParameterChangedArgs args)
		{
 			if (ParameterChanged != null)
			{
				ParameterChanged(this, args);
			}
		}

        #endregion

        #region Events
        /// <summary>
        /// Происходит при изменении статуса состояния сети
        /// </summary>
        //public event EventHandler<StatusChangedEventArgs> NetworkChangedStatus;
        /// <summary>
        /// Происходит при изменении статуса состояния устройтва
        /// </summary>
        //public event EventHandler<StatusChangedEventArgs> DeviceChangedStatus;
        /// <summary>
        /// Происходит при изменении статуса состояния устройтва или сети
        /// (объединяет-дублирует события NetworkChangedStatus и DeviceChangedStatus)
        /// </summary>
        public event EventHandler<StatusChangedEventArgs> StatusChanged;
		public event EventHandler<ParameterChangedArgs> ParameterChanged;
        #endregion
    }
}
