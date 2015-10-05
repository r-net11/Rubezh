using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.Management;
using ResursNetwork.Networks.Collections.ObjectModel;
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
        private void EventHandler_NetworkControllers_CollectionChanged(
            object sender, CollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    { break; }
            }
            foreach(var network in _NetworkControllers)
            {
                network.StatusChanged += network_StatusChanged;
            }
        }

        private void network_StatusChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

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
        /// Происходит при изменении статуса состояния устройтва
        /// </summary>
        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        #endregion
    }
}
