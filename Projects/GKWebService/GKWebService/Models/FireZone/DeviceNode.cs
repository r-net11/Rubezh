using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKWebService.Models.FireZone
{
    public class DeviceNode
    {
        public DeviceNode()
        {
            DeviceList = new List<Device>();
        }

        public List<Device> DeviceList { get; set; }
    }
}
