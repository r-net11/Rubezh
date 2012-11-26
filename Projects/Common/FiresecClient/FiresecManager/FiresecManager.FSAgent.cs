using System;
using System.Linq;
using Common;
using Firesec;
using FiresecAPI;
using Infrastructure.Common;
using FSAgentClient;
using System.Collections.Generic;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static void AddToIgnoreList(List<Device> devices)
		{
			var devicePaths = new List<string>();
			foreach (var device in devices)
			{
				devicePaths.Add(device.PlaceInTree);
			}

			FSAgent.AddToIgnoreList(devicePaths);
		}

		public static void RemoveFromIgnoreList(List<Device> devices)
		{
			var devicePaths = new List<string>();
			foreach (var device in devices)
			{
				devicePaths.Add(device.PlaceInTree);
			}

			FSAgent.RemoveFromIgnoreList(devicePaths);
		}

		public static void SetZoneGuard(Guid secPanelUID, int localZoneNo)
		{
			var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == secPanelUID);
			if (device != null)
			{
				FSAgent.SetZoneGuard(device.PlaceInTree, localZoneNo.ToString());
			}
		}

		public static void UnSetZoneGuard(Guid secPanelUID, int localZoneNo)
		{
			var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == secPanelUID);
			if (device != null)
			{
				FSAgent.UnSetZoneGuard(device.PlaceInTree, localZoneNo.ToString());
			}
		}

		public static void AddUserMessage(string message)
		{
			FSAgent.AddUserMessage(message);
		}
	}
}