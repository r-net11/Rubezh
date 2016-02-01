using System.Collections.Generic;

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
