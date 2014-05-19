using System;
using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;

namespace PlansModule.Kursk.Designer
{
	internal static class Helper
	{
		static Dictionary<Guid, XDevice> _xdeviceMap;
		public static void BuildMap()
		{
			_xdeviceMap = new Dictionary<Guid, XDevice>();
			XManager.Devices.ForEach(item => _xdeviceMap.Add(item.BaseUID, item));
		}

		public static XDevice GetXDevice(ElementRectangleTank element)
		{
			//return XManager.Devices.FirstOrDefault(x => x.UID == element.XDeviceUID);
			return element.XDeviceUID != Guid.Empty && _xdeviceMap != null && _xdeviceMap.ContainsKey(element.XDeviceUID) ? _xdeviceMap[element.XDeviceUID] : null;
		}
	}
}