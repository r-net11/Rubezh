using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;

namespace FiresecClient
{
	public partial class GKManager
	{
		#region RebuildRSR2Addresses
		public static void RebuildRSR2Addresses(GKDevice parentDevice)
		{
			foreach (var shliefDevice in parentDevice.Children)
			{
				RebuildRSR2Addresses_Children = new List<GKDevice>();
				RebuildRSR2Addresses_AddChild(shliefDevice);

				byte currentAddress = 1;
				foreach (var device in RebuildRSR2Addresses_Children)
				{
					device.IntAddress = currentAddress;
					device.OnChanged();
				}
			}
		}

		static List<GKDevice> RebuildRSR2Addresses_Children;
		static void RebuildRSR2Addresses_AddChild(GKDevice device)
		{
			foreach (var child in device.Children)
			{
				RebuildRSR2Addresses_AddChild(child);
			}
		}
		#endregion

		public static void ChangeLogic(GKDevice device, GKLogic logic)
		{
			device.Logic = logic;
			device.OnChanged();
		}
	}
}