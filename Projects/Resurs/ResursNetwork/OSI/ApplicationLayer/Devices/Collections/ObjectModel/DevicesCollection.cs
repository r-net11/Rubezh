using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.OSI.ApplicationLayer;

namespace ResursNetwork.OSI.ApplicationLayer.Devices.Collections.ObjectModel
{
    public class DevicesCollection: KeyedCollection<UInt32, IDevice>
    {
        #region Fields And Properties

        public readonly INetwrokController _NetworkController;
        
        #endregion

        #region Constructors

        private DevicesCollection() { throw new NotSupportedException(); }
        
        public DevicesCollection(INetwrokController network)
        {
            if (network == null) 
            { throw new ArgumentNullException("network"); }
            else
            { _NetworkController = network; }
        }
        #endregion

        #region Methods

        protected override uint GetKeyForItem(IDevice item)
        {
            return item.Address;
        }

        protected override void ClearItems()
        {
            var list = this.Items;
            
            foreach(DeviceBase device in list)
            {
                device.Network = null;
            }

            base.ClearItems();

            foreach(var device in list)
            {
                OnCollectionChanged(new DevicesCollectionChangedEventArgs
                {
                    Action = NotifyCollectionChangedAction.Remove,
                    Device = (IDevice)device
                });
            }
        }

        protected override void RemoveItem(int index)
        {
            var removedItem = this[index];
            this[index].Network = null;
            base.RemoveItem(index);

            OnCollectionChanged(
                new DevicesCollectionChangedEventArgs
                {
                    Action = NotifyCollectionChangedAction.Remove,
                    Device = (IDevice)removedItem
                });
        }

        protected override void InsertItem(int index, IDevice item)
        {
            if (item.Network != null)
            {
                throw new ArgumentException(
                    "Попытка добавить устройтство, которое принадлежит другому контроллеру");
            }
            else
            {
                item.Network = _NetworkController;
            }
            base.InsertItem(index, item);

            OnCollectionChanged(
                new DevicesCollectionChangedEventArgs
                {
                    Action = NotifyCollectionChangedAction.Add,
                    Device = (IDevice)item
                });
        }

        protected override void SetItem(int index, IDevice item)
        {
            var removedItem = this[index];

            if (item.Network != null)
            {
                throw new ArgumentException(
                    "Попытка добавить устройтство, которое принадлежит другому контроллеру");
            }
            else
            {
                item.Network = _NetworkController;
            }

            base.SetItem(index, item);

            OnCollectionChanged(
                new DevicesCollectionChangedEventArgs
                {
                    Action = NotifyCollectionChangedAction.Remove,
                    Device = (IDevice)removedItem
                });
            OnCollectionChanged(
                new DevicesCollectionChangedEventArgs
                {
                    Action = NotifyCollectionChangedAction.Add,
                    Device = (IDevice)item
                });
        }

        private void OnCollectionChanged(DevicesCollectionChangedEventArgs args)
        {
            var handler = this.CollectionChanged;

			if (args == null)
			{
				throw new ArgumentNullException();
			}

            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion

        #region Events

        public event EventHandler<DevicesCollectionChangedEventArgs> CollectionChanged;

        #endregion
    }
}
