﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Journal;

namespace ServerFS2.Monitoring
{
	public static class MonitoringManager
	{
		public static List<MonitoringUSB> MonitoringUSBs { get; private set; }

		static MonitoringManager()
		{
			MonitoringUSBs = new List<MonitoringUSB>();
		}

		public static MonitoringUSB Find(Device device)
		{
			return MonitoringUSBs.FirstOrDefault(x => x.USBDevice.UID == device.ParentUSB.UID);
		}

		public static void StartMonitoring(Device device = null)
		{
			if (device == null)
			{
				var deviceStatesManager = new DeviceStatesManager();
				deviceStatesManager.CanNotifyClients = false;
				foreach (var childDevice in ConfigurationManager.Devices)
				{
					if (childDevice.IsMonitoringDisabled)
					{
						deviceStatesManager.ForseUpdateDeviceStates(childDevice);
					}
				}

				USBManager.UsbRemoved += new Action(USBManager_UsbRemoved);

				MonitoringUSBs = new List<MonitoringUSB>();
				foreach (var usbDevice in ConfigurationManager.DeviceConfiguration.RootDevice.Children)
				{
					var monitoringUSB = new MonitoringUSB(usbDevice);
					if (monitoringUSB.MonitoringPanels.Count > 0 || monitoringUSB.MonitoringNonPanels.Count > 0)
					{
						MonitoringUSBs.Add(monitoringUSB);
					}
				}
			}
			foreach (var monitoringUSB in MonitoringUSBs)
			{
				if (device == null || monitoringUSB.USBDevice.UID == device.ParentUSB.UID)
				{
					monitoringUSB.StartMonitoring();
				}
			}
		}

		static void USBManager_UsbRemoved()
		{
		}

		public static void StopMonitoring(Device device = null)
		{
			foreach (var monitoringUSB in MonitoringUSBs)
			{
				if (device == null || monitoringUSB.USBDevice.UID == device.ParentUSB.UID)
				{
					monitoringUSB.StopMonitoring();
				}
			}
		}

		public static void SuspendMonitoring(Device device = null)
		{
			foreach (var monitoringUSB in MonitoringUSBs)
			{
				if (device == null || monitoringUSB.USBDevice.UID == device.ParentUSB.UID)
				{
					monitoringUSB.SuspendMonitoring();
				}
			}
		}

		public static void ResumeMonitoring(Device device = null)
		{
			foreach (var monitoringUSB in MonitoringUSBs)
			{
				if (device == null || monitoringUSB.USBDevice.UID == device.ParentUSB.UID)
				{
					monitoringUSB.ResumeMonitoring();
				}
			}
		}

		public static void AddPanelResetItems(List<PanelResetItem> panelResetItems, string userName)
		{
			foreach (var panelResetItem in panelResetItems)
			{
				var panelDevice = ConfigurationManager.Devices.FirstOrDefault(x => x.UID == panelResetItem.PanelUID);
				var monitoringUSB = MonitoringUSBs.FirstOrDefault(x => x.USBDevice.UID == panelDevice.ParentUSB.UID);
				if (monitoringUSB != null)
				{
					foreach (var monitoringDevice in monitoringUSB.MonitoringPanels)
					{
						foreach (var stateId in panelResetItem.Ids)
						{
							if (monitoringDevice.PanelDevice == panelDevice)
							{
								var metadataPanelState = MetadataHelper.Metadata.panelStates.FirstOrDefault(x => x.ID == stateId);
								if (metadataPanelState != null)
								{
									CustomMessageJournalHelper.Add("Состояние '" + metadataPanelState.value + "' сброшено оператором", userName, panelDevice);
								}
							}
						}

						monitoringDevice.ResetStateIds = panelResetItem.Ids.ToList();
					}
				}
			}
		}

		public static void AddTaskIgnore(List<Device> devices)
		{
			foreach (var device in devices)
			{
				var monitoringProcessor = Find(device);
				if (monitoringProcessor != null)
				{
					foreach (var monitoringPanel in monitoringProcessor.MonitoringPanels)
					{
						if (monitoringPanel.PanelDevice == device.ParentPanel)
						{
							monitoringPanel.DevicesToIgnore = new List<Device>() { device };
						}
					}
				}
			}
		}

		public static void AddTaskResetIgnore(List<Device> devices)
		{
			foreach (var device in devices)
			{
				var monitoringProcessor = Find(device);
				if (monitoringProcessor != null)
				{
					foreach (var monitoringDevice in monitoringProcessor.MonitoringPanels)
					{
						if (monitoringDevice.PanelDevice == device.ParentPanel)
						{
							monitoringDevice.DevicesToResetIgnore = new List<Device>() { device };
						}
					}
				}
			}
		}

		public static void AddTaskSetGuard(Zone zone, string userName)
		{
			var deviceInZone = zone.DevicesInZone.FirstOrDefault();
			if (deviceInZone != null)
			{
				AddTaskSetGuard(deviceInZone.ParentPanel, userName, zone);
			}
		}

		public static void AddTaskResetGuard(Zone zone, string userName)
		{
			var deviceInZone = zone.DevicesInZone.FirstOrDefault();
			if (deviceInZone != null)
			{
				AddTaskResetGuard(deviceInZone.ParentPanel, userName, zone);
			}
		}

		public static void AddTaskSetGuard(Device panelDevice, string userName, Zone zone = null)
		{
			var monitoringProcessor = Find(panelDevice);
			if (monitoringProcessor != null)
			{
				foreach (var monitoringDevice in monitoringProcessor.MonitoringPanels)
				{
					if (monitoringDevice.PanelDevice == panelDevice)
					{
						if (zone != null)
						{
							CustomMessageJournalHelper.Add("Постановка зоны на охрану", userName, panelDevice, null, zone);
						}
						else
						{
							CustomMessageJournalHelper.Add("Постановка прибора на охрану", userName, panelDevice);
						}
						monitoringDevice.ZonesToSetGuard.Add(zone);
						break;
					}
				}
			}
		}

		public static void AddTaskResetGuard(Device panelDevice, string userName, Zone zone = null)
		{
			var monitoringProcessor = Find(panelDevice);
			if (monitoringProcessor != null)
			{
				foreach (var monitoringDevice in monitoringProcessor.MonitoringPanels)
				{
					if (monitoringDevice.PanelDevice == panelDevice)
					{
						if (zone != null)
						{
							CustomMessageJournalHelper.Add("Снятие зоны с охраны", userName, panelDevice, null, zone);
						}
						else
						{
							CustomMessageJournalHelper.Add("Снятие прибора с охраны", userName, panelDevice);
						}
						monitoringDevice.ZonesToResetGuard.Add(zone);
						break;
					}
				}
			}
		}

		public static void AddCommand(Device device, string commandName)
		{
			var monitoringProcessor = Find(device);
			if (monitoringProcessor != null)
			{
				foreach (var monitoringDevice in monitoringProcessor.MonitoringPanels)
				{
					if (monitoringDevice.PanelDevice == device.ParentPanel)
					{
						monitoringDevice.CommandItems.Add(new CommandItem(device, commandName));
						break;
					}
				}
			}
		}
	}
}