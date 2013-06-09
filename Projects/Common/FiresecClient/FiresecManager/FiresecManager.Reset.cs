using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static void ResetAllStates()
		{
			try
			{
				var resetItems = new List<ResetItem>();
				foreach (var device in Devices)
				{
					foreach (var deviceDriverState in device.DeviceState.ThreadSafeStates)
					{
						if (deviceDriverState.DriverState.IsManualReset)
						{
							var resetItem = new ResetItem()
							{
								DeviceState = device.DeviceState
							};
							var existringResetItem = resetItems.FirstOrDefault(x => x.DeviceState == resetItem.DeviceState);
							if (existringResetItem != null)
							{
								foreach (var driverState in resetItem.States)
								{
									if (existringResetItem.States.Any(x => x.DriverState.Code == driverState.DriverState.Code) == false)
										existringResetItem.States.Add(driverState);
								}
							}
							else
							{
								resetItems.Add(resetItem);
							}
						}
					}
				}

				FiresecManager.ResetStates(resetItems);
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecManager.ResetAllStates");
			}
		}
	}
}