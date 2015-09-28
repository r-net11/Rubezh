using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursNetwork.Devices.Collections.ObjectModel
{
    public class DevicesCollection: KeyedCollection<UInt32, DeviceBase>
    {
        protected override uint GetKeyForItem(DeviceBase item)
        {
            return item.Address;
        }
    }
}
