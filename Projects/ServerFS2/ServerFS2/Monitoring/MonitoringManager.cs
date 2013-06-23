using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FS2Api;

namespace ServerFS2.Monitoring
{
	public static class MonitoringManager
	{
		public static List<MonitoringProcessor> MonitoringProcessors { get; private set; }

		static MonitoringManager()
		{
			MonitoringProcessors = new List<MonitoringProcessor>();
		}

		public static MonitoringProcessor Find(Device device)
		{
			return MonitoringProcessors.FirstOrDefault(x => x.USBDevice.UID == device.ParentUSB.UID);
		}

		public static void StartMonitoring(Device device = null)
		{
			if (device == null)
			{
				MonitoringProcessors = new List<MonitoringProcessor>();
				foreach (var usbDevice in ConfigurationManager.DeviceConfiguration.RootDevice.Children)
				{
					var monitoringProcessor = new MonitoringProcessor(usbDevice);
					if (monitoringProcessor.MonitoringPanelDevices.Count > 0 || monitoringProcessor.MonitoringNonPanelDevices.Count > 0)
					{
						MonitoringProcessors.Add(monitoringProcessor);
					}
				}
			}
			foreach (var monitoringProcessor in MonitoringProcessors)
			{
				if (device == null || monitoringProcessor.USBDevice.UID == device.ParentUSB.UID)
				{
					monitoringProcessor.StartMonitoring();
				}
			}
		}

		public static void StopMonitoring(Device device = null)
		{
			foreach (var monitoringProcessor in MonitoringProcessors)
			{
				if (device == null || monitoringProcessor.USBDevice.UID == device.ParentUSB.UID)
				{
					monitoringProcessor.StopMonitoring();
				}
			}
		}

		public static void SuspendMonitoring(Device device = null)
		{
			foreach (var monitoringProcessor in MonitoringProcessors)
			{
				if (device == null || monitoringProcessor.USBDevice.UID == device.ParentUSB.UID)
				{
					monitoringProcessor.SuspendMonitoring();
				}
			}
		}

		public static void ResumeMonitoring(Device device = null)
		{
			foreach (var monitoringProcessor in MonitoringProcessors)
			{
				if (device == null || monitoringProcessor.USBDevice.UID == device.ParentUSB.UID)
				{
					monitoringProcessor.ResumeMonitoring();
				}
			}
		}

		public static void AddPanelResetItems(List<PanelResetItem> panelResetItems)
		{
			foreach (var panelResetItem in panelResetItems)
			{
				var panelDevice = ConfigurationManager.Devices.FirstOrDefault(x => x.UID == panelResetItem.PanelUID);
				var monitoringProcessor = MonitoringProcessors.FirstOrDefault(x => x.USBDevice.UID == panelResetItem.PanelUID);
				if (monitoringProcessor != null)
				{
					foreach (var monitoringDevice in monitoringProcessor.MonitoringPanelDevices)
					{
						if (monitoringDevice.Panel == panelDevice)
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
					foreach (var monitoringDevice in monitoringProcessor.MonitoringPanelDevices)
					{
						if (monitoringDevice.Panel == device.ParentPanel)
						{
							monitoringDevice.DevicesToIgnore = new List<Device>() { device };
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
					foreach (var monitoringDevice in monitoringProcessor.MonitoringPanelDevices)
					{
						if (monitoringDevice.Panel == device.ParentPanel)
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
				foreach (var monitoringDevice in monitoringProcessor.MonitoringPanelDevices)
				{
					if (monitoringDevice.Panel == device.ParentPanel)
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