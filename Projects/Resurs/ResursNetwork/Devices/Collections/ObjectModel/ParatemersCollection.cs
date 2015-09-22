using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubezhResurs.Devices;

namespace RubezhResurs.Devices.Collections.ObjectModel
{
    /// <summary>
    /// Коллекция для хранения описания праметров устройства
    /// </summary>
    public class ParatemersCollection: KeyedCollection<UInt32, Parameter>
    {
        protected override uint GetKeyForItem(Parameter item)
        {
            return item.Index;
        }
    }
}
