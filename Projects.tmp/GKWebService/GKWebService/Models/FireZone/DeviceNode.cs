using System.Collections.Generic;

namespace GKWebService.Models
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
