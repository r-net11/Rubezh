using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.GK;
using FiresecAPI.XModels;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using XFiresecAPI;
using System.Threading;
using System.Windows.Threading;
using GKModule.ViewModels;

namespace GKModule
{
	public partial class Watcher
	{
		List<DeviceParameeterRequest> DeviceParameeterRequests;
		int deviceParameeterRequestIndex = 0;

		void GetNextParameter()
		{
			if (DeviceParameeterRequests == null)
			{
				DeviceParameeterRequests = new List<DeviceParameeterRequest>();
				foreach (var binaryObject in GkDatabase.BinaryObjects)
				{
					var device = binaryObject.BinaryBase as XDevice;
					if (device != null)
					{
						//foreach (var auParameter in device.Driver.AUParameters)
						//{
						//    var deviceParameeterRequest = new DeviceParameeterRequest()
						//    {
						//        Device = device,
						//        AUParameter = auParameter
						//    };
						//    DeviceParameeterRequests.Add(deviceParameeterRequest);
						//}
						var deviceParameeterRequest = new DeviceParameeterRequest()
						{
							Device = device,
						};
						DeviceParameeterRequests.Add(deviceParameeterRequest);
					}
				}
			}

			if (deviceParameeterRequestIndex < DeviceParameeterRequests.Count)
			{
				var deviceParameeterRequests = DeviceParameeterRequests[deviceParameeterRequestIndex];
				GetParameters(deviceParameeterRequests.Device);
				deviceParameeterRequestIndex++;
			}
		}

		static void GetParameters(XDevice device)
		{
			var AUParameterValues = new List<AUParameterValue>();
			foreach (var auParameter in device.Driver.AUParameters)
			{
				var bytes = new List<byte>();
				var databaseNo = device.GetDatabaseNo(DatabaseType.Kau);
				bytes.Add((byte)device.Driver.DriverTypeNo);
				bytes.Add(device.IntAddress);
				bytes.Add((byte)(device.ShleifNo - 1));
				bytes.Add(auParameter.No);
				var result = SendManager.Send(device.KauDatabaseParent, 4, 128, 2, bytes);
				if (!result.HasError)
				{
					if (result.Bytes.Count > 0)
					{
						var parameterValue = BytesHelper.SubstructShort(result.Bytes, 0);
						var auParameterValue = new AUParameterValue()
						{
							Name = auParameter.Name,
							Value = parameterValue
						};
						AUParameterValues.Add(auParameterValue);
					}
				}
			}

			var currentDustinessParameter = AUParameterValues.FirstOrDefault(x => x.Name == "Текущая запыленность");
			var criticalDustinessParameter = AUParameterValues.FirstOrDefault(x => x.Name == "Порог запыленности предварительный");
			if (currentDustinessParameter != null && criticalDustinessParameter != null)
			{
				if (currentDustinessParameter.Value > 0 && currentDustinessParameter.Value > 0)
				{
					if (currentDustinessParameter.Value - criticalDustinessParameter.Value > 0)
					{
						ApplicationService.Invoke(() => { device.DeviceState.IsService = true; });
					}
				}
			}
		}

		class DeviceParameeterRequest
		{
			public XDevice Device { get; set; }
			//public XAUParameter AUParameter { get; set; }
		}
	}
}