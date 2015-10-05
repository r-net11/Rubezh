using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ResursNetwork.Management;
using ResursNetwork.Networks.Collections.ObjectModel;
using ResursNetwork.Devices;
using ResursNetwork.OSI.ApplicationLayer;

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

        public void EventHandler_Device_ErrorOccurred(object sender, ErrorOccuredEventArgs e)
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
        #endregion
    }
}
