using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.OSI.ApplicationLayer;

namespace ResursNetwork.Networks.Collections.ObjectModel
{
    public class NetworksCollection : KeyedCollection<Guid, INetwrokController>
    {
        #region Methods

        protected override Guid GetKeyForItem(INetwrokController item)
        {
            return item.Id;
        }

        protected override void InsertItem(int index, INetwrokController item)
        {
            base.InsertItem(index, item);
            OnCollectionChanged(
                new NetworksCollectionChangedEventArgs 
                {
                    Action = NotifyCollectionChangedAction.Add,
                    Network = item
                });
        }

        protected override void SetItem(int index, INetwrokController item)
        {
            var removedItem = this[index];
            base.SetItem(index, item);
            OnCollectionChanged(
                new NetworksCollectionChangedEventArgs
                {
                    Action = NotifyCollectionChangedAction.Remove,
                    Network = removedItem,
                });
            OnCollectionChanged(
                new NetworksCollectionChangedEventArgs
                {
                    Action = NotifyCollectionChangedAction.Add,
                    Network = item,
                });
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            base.RemoveItem(index);
            OnCollectionChanged(
                new NetworksCollectionChangedEventArgs
                    {
                        Network = item,
                        Action = NotifyCollectionChangedAction.Remove
                    });
        }

        protected override void ClearItems()
        {
            var items = Items;
            base.ClearItems();
            foreach (var item in items)
            {
                OnCollectionChanged(
                    new NetworksCollectionChangedEventArgs
                    {
                        Network = item,
                        Action = NotifyCollectionChangedAction.Remove
                    });
            }
        }

        private void OnCollectionChanged(NetworksCollectionChangedEventArgs e) 
        {
            var handler = this.CollectionChanged;
            
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Events

        public event EventHandler<NetworksCollectionChangedEventArgs> CollectionChanged;

        #endregion
    }
}
