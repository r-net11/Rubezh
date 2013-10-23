using System.Collections.Generic;
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
			if (device.IsNotUsed)
				return;

			if (device.IsRealDevice)
				AllDevices.Add(device);

			foreach (var child in device.Children)
			{
				AddChild(child);
			}
		}
	}
}