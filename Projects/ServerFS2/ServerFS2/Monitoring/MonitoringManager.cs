using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
						if (monitoringDevice.PanelDevice == panelDevice)
						{
							CustomMessageJournalHelper.Add("Команда оператора. Сброс", userName, panelDevice);
						}
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

		public static void ExecuteCommand(Device device, string commandName)
		{
			CommandExecutor commandExecutor = new CommandExecutor(device, commandName);
			//commandExecutor.CheckForExpired();
		}
	}
}