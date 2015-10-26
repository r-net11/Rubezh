using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.OSI.ApplicationLayer.Devices;

namespace ResursNetwork.OSI.ApplicationLayer.Devices.Collections.ObjectModel
{
    /// <summary>
    /// Коллекция для хранения описания праметров устройства
    /// </summary>
    public class ParatemersCollection: KeyedCollection<string, Parameter>
    {
        protected override string GetKeyForItem(Parameter item)
        {
            return item.Name;
        }
    }
}
