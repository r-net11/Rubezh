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
                new CollectionChangedEventArgs 
                {
                    Networks = new INetwrokController[] { item }, 
                    Action = NotifyCollectionChangedAction.Add 
                });
        }

        protected override void SetItem(int index, INetwrokController item)
        {
            base.SetItem(index, item);
            OnCollectionChanged(
                new CollectionChangedEventArgs
                {
                    Networks = new INetwrokController[] { item },
                    Action = NotifyCollectionChangedAction.Replace
                });
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            base.RemoveItem(index);
            OnCollectionChanged(
                new CollectionChangedEventArgs
                    {
                        Networks = new INetwrokController[] { item },
                        Action = NotifyCollectionChangedAction.Remove
                    });
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            OnCollectionChanged(
                new CollectionChangedEventArgs
                {
                    Networks = Items.ToArray(),
                    Action = NotifyCollectionChangedAction.Reset
                });
        }

        private void OnCollectionChanged(CollectionChangedEventArgs e) 
        {
            var handler = this.CollectionChanged;
            
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Events

        public event EventHandler<CollectionChangedEventArgs> CollectionChanged;

        #endregion
    }
}
