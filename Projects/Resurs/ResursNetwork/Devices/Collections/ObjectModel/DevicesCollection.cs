using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.OSI.ApplicationLayer;

namespace ResursNetwork.Devices.Collections.ObjectModel
{
    public class DevicesCollection: KeyedCollection<UInt32, DeviceBase>
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
        protected override uint GetKeyForItem(DeviceBase item)
        {
            return item.Address;
        }
        protected override void ClearItems()
        {
            foreach(DeviceBase device in this.Items)
            {
                device.Network = null;
            }
            base.ClearItems();
        }
        protected override void RemoveItem(int index)
        {
            this[index].Network = null;
            base.RemoveItem(index);
        }
        protected override void InsertItem(int index, DeviceBase item)
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
        }
        protected override void SetItem(int index, DeviceBase item)
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
            base.SetItem(index, item);
        }
        #endregion
    }
}
