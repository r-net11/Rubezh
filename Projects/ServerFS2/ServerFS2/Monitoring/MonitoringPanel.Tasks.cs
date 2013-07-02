using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Journal;
using ServerFS2.Service;
using System.Diagnostics;
using ServerFS2.Operations;

namespace ServerFS2.Monitoring
{
	public partial class MonitoringPanel
	{
		public List<string> ResetStateIds { get; set; }
		public List<Device> DevicesToIgnore { get; set; }
		public List<Device> DevicesToResetIgnore { get; set; }
		public List<CommandItem> CommandItems { get; set; }
		public List<Zone> ZonesToSetGuard { get; set; }
		public List<Zone> ZonesToResetGuard { get; set; }
		int RealChildIndex;
		bool IsStateRefreshNeeded;

		void DoTasks()
		{
			if (ResetStateIds != null && ResetStateIds.Count > 0)
			{
				ServerHelper.ResetOnePanelStates(PanelDevice, ResetStateIds);
				ResetStateIds.Clear();
				DeviceStatesManager.UpdatePanelState(PanelDevice);
			}

			if (DevicesToIgnore != null && DevicesToIgnore.Count > 0)
			{
				foreach (var deviceToIgnore in DevicesToIgnore)
				{
					var response = USBManager.Send(PanelDevice, 0x02, 0x54, 0x0B, 0x01, 0x00, deviceToIgnore.AddressOnShleif, 0x00, 0x00, 0x00, deviceToIgnore.ShleifNo - 1);
				}
				DevicesToIgnore = new List<Device>();
			}
			if (DevicesToResetIgnore != null && DevicesToResetIgnore.Count > 0)
			{
				foreach (var deviceToIgnore in DevicesToResetIgnore)
				{
					USBManager.Send(PanelDevice, 0x02, 0x54, 0x0B, 0x00, 0x00, deviceToIgnore.AddressOnShleif, 0x00, 0x00, 0x00, deviceToIgnore.ShleifNo - 1);
				}
				DevicesToResetIgnore = new List<Device>();
			}
			if (ZonesToSetGuard != null && ZonesToSetGuard.Count > 0)
			{
				foreach (var zone in ZonesToSetGuard)
				{
					var deviceZoneNo = 0;
					if (zone != null)
					{
						deviceZoneNo = 0;
					}
					var response = USBManager.Send(PanelDevice, 0x02, 0x54, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);
				}
				ZonesToSetGuard = new List<Zone>();
			}
			if (ZonesToResetGuard != null && ZonesToResetGuard.Count > 0)
			{
				foreach (var zone in ZonesToResetGuard)
				{
					var deviceZoneNo = 0;
					if (zone != null)
					{
						deviceZoneNo = 0;
					}
					var response = USBManager.Send(PanelDevice, 0x02, 0x54, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);
				}
				ZonesToResetGuard = new List<Zone>();
			}
			if (CommandItems != null && CommandItems.Count > 0)
			{
				CommandItems.ForEach(x => x.Send());
				CommandItems = new List<CommandItem>();
			}
			if (IsStateRefreshNeeded)
			{
				DeviceStatesManager.UpdatePanelState(PanelDevice);
				foreach (var device in RealChildren)
				{
					DeviceStatesManager.UpdateDeviceStateAndParameters(device);
				}
				IsStateRefreshNeeded = false;
			}
			DeviceStatesManager.UpdatePanelExtraDevices(PanelDevice);

			DeviceStatesManager.UpdateDeviceStateAndParameters(RealChildren[RealChildIndex]);
			NextIndextoGetParams();
		}

		void NextIndextoGetParams()
		{
			RealChildIndex++;
			if (RealChildIndex + 1 >= RealChildren.Count)
				RealChildIndex = 0;
		}

		public void SynchronizeTime()
		{
			var setDateTimeProperty = PanelDevice.Properties.FirstOrDefault(x => x.Name == "SetDateTime");
			if (setDateTimeProperty != null && setDateTimeProperty.Value == "1")
			{
				ServerHelper.SynchronizeTime(PanelDevice);
			}
		}
	}
}