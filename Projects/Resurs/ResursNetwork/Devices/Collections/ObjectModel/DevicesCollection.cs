using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubezhResurs.Devices.Collections.ObjectModel
{
    public class DevicesCollection: KeyedCollection<UInt32, Device>
    {
        protected override uint GetKeyForItem(Device item)
        {
            return item.Address;
        }
    }
}
