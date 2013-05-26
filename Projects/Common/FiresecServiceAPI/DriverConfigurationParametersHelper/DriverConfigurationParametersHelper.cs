using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace FiresecAPI.Models
{
	public static class DriverConfigurationParametersHelper
	{
		public static void CreateKnownProperties(List<Driver> drivers)
		{
			try
			{
				RMHelper.Create(drivers);
				MROHelper.Create(drivers);
				AMP4Helper.Create(drivers);
				MDUHelper.Create(drivers);
				BUZHelper.Create(drivers);
				foreach (var driverType in new List<DriverType>() { DriverType.Pump, DriverType.JokeyPump, DriverType.Compressor, DriverType.DrenazhPump, DriverType.CompensationPump })
				{
					var driver = drivers.FirstOrDefault(x => x.DriverType == driverType);
					BUNHelper.Create(driver);
				}
				MPTHelper.Create(drivers);
				DetectorsHelper.Create(drivers);

				AM_1_Helper.Create(drivers);
				AM1_T_Helper.Create(drivers);
				AM1_O_Helper.Create(drivers);

				ControlCabinetHelper.Create(drivers);
				FanCabinetHelper.Create(drivers);
				MRO2Helper.Create(drivers);
			}
			catch (Exception e)
			{
				Logger.Error(e, "DriverConfigurationParametersHelper.CreateKnownProperties");
			}
		}
	}
}