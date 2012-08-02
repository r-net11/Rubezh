using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace Common.GK
{
	public static class KauChildrenHelper
	{
		static List<XDevice> AllDevices;
		public static List<XDevice> GetRealChildren(XDevice device)
		{
			AllDevices = new List<XDevice>();
			AddChild(device);
			return AllDevices;
		}

		static void AddChild(XDevice device)
		{
			if (!device.Driver.IsGroupDevice)
				AllDevices.Add(device);
			foreach (var child in device.Children)
			{
				AddChild(child);
			}
		}
	}
}