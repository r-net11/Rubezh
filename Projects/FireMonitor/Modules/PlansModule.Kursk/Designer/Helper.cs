using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using FiresecClient;
using FiresecAPI.Models;

namespace PlansModule.Kursk.Designer
{
	internal static class Helper
	{
		static Dictionary<Guid, XDevice> _xdeviceMap;
		public static void BuildMap()
		{
			_xdeviceMap = new Dictionary<Guid, XDevice>();
			XManager.Devices.ForEach(item => _xdeviceMap.Add(item.UID, item));
		}

		public static XDevice GetXDevice(ElementRectangleTank element)
		{
			return element.XDeviceUID != Guid.Empty && _xdeviceMap != null && _xdeviceMap.ContainsKey(element.XDeviceUID) ? _xdeviceMap[element.XDeviceUID] : null;
		}
	}
}