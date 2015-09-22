using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubezhResurs.OSI.ApplicationLayer;

namespace RubezhResurs.Networks.Collections.ObjectModel
{
    public class NetworksCollection: KeyedCollection<UInt32, INetwrokController>
    {
        protected override uint GetKeyForItem(INetwrokController item)
        {
            return item.ControllerId;
        }
    }
}
