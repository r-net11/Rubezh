using GKWebService.Models;
using GKWebService.Utils;
using RubezhAPI;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GKWebService.DataProviders.Devices
{
	public class DevicesDataProvider
	{
		/// <summary>
		/// Инстанс провайдера данных
		/// </summary>
		public static DevicesDataProvider Instance
		{
			get
			{
				if (_instance != null)
				{
					return _instance;
				}
				return _instance = new DevicesDataProvider();
			}
		}

		/// <summary>
		/// private instance of DataProvider
		/// </summary>
		private static DevicesDataProvider _instance;

		public IEnumerable<Device> GetDevices()
		{
			var root = GKManager.DeviceConfiguration.RootDevice;
			return BuildTreeList(root);
		}

		private List<Device> BuildTreeList(GKDevice device, int level = 0)
		{
			List<Device> list = new List<Device>();

			list.Add(new Device(device)
			{
				Level = level,
			});

			foreach (var child in device.Children)
			{
				list.AddRange(BuildTreeList(child, level + 1));
			}

			return list;
		}
	}
}