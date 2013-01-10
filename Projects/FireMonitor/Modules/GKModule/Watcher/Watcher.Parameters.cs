using System.Collections.Generic;
using System.Linq;
using Common.GK;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKModule
{
	public partial class Watcher
	{
		static void GetDeviceParameters(XDevice device)
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
	}
}