using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using FS2Api;

namespace ServerFS2.Monitoring
{
	public static partial class MonitoringProcessor
	{
		static List<MonitoringDevice> MonitoringDevices = new List<MonitoringDevice>();
		public static bool DoMonitoring { get; set; }
		static bool LoopMonitoring = true;
		static bool IsInitialized = false;
		static Thread MonitoringThread;
		static DateTime StartTime;
		static object locker = new object();

		static MonitoringProcessor()
		{
			foreach (var device in ConfigurationManager.Devices.Where(x => DeviceStatesManager.IsMonitoringable(x)))
			{
				if (!device.IsMonitoringDisabled)
				{
					MonitoringDevices.Add(new MonitoringDevice(device));
				}
			}
			DoMonitoring = false;
			foreach (var usbProcessorInfo in USBManager.UsbProcessorInfos)
			{
				usbProcessorInfo.UsbProcessor.NewResponse += new Action<Response>(UsbRunner_NewResponse);
			}
		}

		public static void StartMonitoring()
		{
			if (!DoMonitoring)
			{
				StartTime = DateTime.Now;
				DoMonitoring = true;
				MonitoringThread = new Thread(OnRun);
				MonitoringThread.Start();
			}
			StartTimeSynchronization();
		}

		public static void StopMonitoring()
		{
			DoMonitoring = false;
			LoopMonitoring = false;

			if (MonitoringThread != null)
			{
				MonitoringThread.Join(TimeSpan.FromSeconds(2));
			}
			MonitoringThread = null;

			StopTimeSynchronization();
		}

		public static void SuspendMonitoring()
		{
			DoMonitoring = false;
		}

		public static void ResumeMonitoring()
		{
			DoMonitoring = true;
		}

		static void OnRun()
		{
			if (!IsInitialized)
			{
				DeviceStatesManager.AllToInitializing();
				MonitoringDevices.Where(x => !x.IsInitialized).ToList().ForEach(x => x.Initialize());
				DeviceStatesManager.AllFromInitializing();
				IsInitialized = true;
			}
			//MonitoringDevices.ForEach(x => x.FromInitializingState());
			while (LoopMonitoring)
			{
				if (DoMonitoring)
				{
					MonitoringDevices.ForEach(x => x.CheckTasks());

					if(IsTimeSynchronizationNeeded)
					{
						MonitoringDevices.ForEach(x => x.SynchronizeTime());
						IsTimeSynchronizationNeeded = false;
					}
				}
			}
		}

		static void UsbRunner_NewResponse(Response response)
		{
			lock (MonitoringDevice.Locker)
			{
				foreach (var monitoringDevice in MonitoringDevices.ToList())
				{
					foreach (var request in monitoringDevice.Requests.ToList())
					{
						if (request != null && request.Id == response.Id)
						{
							monitoringDevice.ReqestResponsed(request, response);
						}
					}
				}
			}
		}

		public static void WriteStats()
		{
			var timeSpan = DateTime.Now - StartTime;
			Trace.WriteLine("betweenCyclesSpan " + MonitoringDevice.betweenCyclesSpan);
			Trace.WriteLine("betweenDevicesSpan " + MonitoringDevice.betweenDevicesSpan);
			Trace.WriteLine("testTime " + timeSpan);
			foreach (var monitoringDevice in MonitoringDevices)
			{
				Trace.WriteLine(monitoringDevice.Panel.PresentationAddress + " " + (monitoringDevice.AnsweredCount / timeSpan.TotalSeconds).ToString() + " " + monitoringDevice.AnsweredCount + "/" + monitoringDevice.UnAnsweredCount);
			}
		}

		public static void AddPanelResetItems(List<PanelResetItem> panelResetItems)
		{
			foreach (var panelResetItem in panelResetItems)
			{
				var parentPanel = ConfigurationManager.Devices.FirstOrDefault(x => x.UID == panelResetItem.PanelUID);
				if (parentPanel != null)
				{
					foreach (var monitoringDevice in MonitoringDevices)
					{
						if (monitoringDevice.Panel == parentPanel)
							monitoringDevice.ResetStateIds = panelResetItem.Ids.ToList();
					}
				}
			}
		}

		public static void AddTaskIgnore(Device device)
		{
			foreach (var monitoringDevice in MonitoringDevices)
			{
				if (monitoringDevice.Panel == device.ParentPanel)
				{
					monitoringDevice.DevicesToIgnore = new List<Device>() { device };
				}
			}
		}

		public static void AddTaskResetIgnore(Device device)
		{
			foreach (var monitoringDevice in MonitoringDevices)
			{
				if (monitoringDevice.Panel == device.ParentPanel)
				{
					monitoringDevice.DevicesToResetIgnore = new List<Device>() { device };
				}
			}
		}

		public static void AddCommand(Device device, string commandName)
		{
			foreach (var monitoringDevice in MonitoringDevices)
			{
				if (monitoringDevice.Panel == device.ParentPanel)
				{
					monitoringDevice.CommandItems.Add(new CommandItem(device, commandName));
					break;
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