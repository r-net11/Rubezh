using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using FiresecClient;
using FiresecAPI.Models;

namespace PlansModule.Kursk
{
	internal static class Helper
	{
		static Dictionary<Guid, XDevice> _xdeviceMap;
		public static void BuildMap()
		{
			_xdeviceMap = new Dictionary<Guid, XDevice>();
			XManager.DeviceConfiguration.Devices.ForEach(item => _xdeviceMap.Add(item.UID, item));
		}

		public static XDevice GetXDevice(ElementRectangleTank element)
		{
			return element.XDeviceUID != Guid.Empty && _xdeviceMap.ContainsKey(element.XDeviceUID) ? _xdeviceMap[element.XDeviceUID] : null;
		}
	}
}