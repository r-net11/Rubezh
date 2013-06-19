using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using FS2Api;
using Common;

namespace ServerFS2.Monitoring
{
	public static partial class MonitoringProcessor
	{
		static List<MonitoringDevice> MonitoringDevices = new List<MonitoringDevice>();
		public static bool DoMonitoring { get; set; }
		static bool LoopMonitoring = true;
		static bool IsInitialized = false;
		static DateTime StartTime;
		static object locker = new object();

		static MonitoringProcessor()
		{
			foreach (var device in ConfigurationManager.Devices.Where(x => DeviceStatesManager.IsMonitoringable(x)))
			{
				MonitoringDevices.Add(new MonitoringDevice(device));
			}
			DoMonitoring = false;
			foreach (var usbProcessorInfo in USBManager.UsbProcessorInfos)
			{
				usbProcessorInfo.UsbProcessor.NewResponse += new Action<Response>(UsbRunner_NewResponse);
			}
		}

		static void OnRun()
		{
			try
			{
				if (!IsInitialized)
				{
					DeviceStatesManager.SetInitializingStateToAll();
					DeviceStatesManager.SetMonitoringDisabled();
					MonitoringDevices.Where(x => !x.IsInitialized).ToList().ForEach(x => x.Initialize());
					DeviceStatesManager.RemoveInitializingFromAll();
					IsInitialized = true;

				//OneDetectorAnalysis();
				//AllDevicesAnalysis();
				}
			}
			catch (FS2StopMonitoringException)
			{
				return;
			}

			while (LoopMonitoring)
			{
				try
				{
					foreach (var monitoringDevice in MonitoringDevices)
					{
						CheckSuspending();
						monitoringDevice.CheckTasks();
					}

					if (IsTimeSynchronizationNeeded)
					{
						MonitoringDevices.ForEach(x => x.SynchronizeTime());
						IsTimeSynchronizationNeeded = false;
					}
				}
				catch (FS2StopMonitoringException)
				{
					return;
				}
				catch (Exception e)
				{
					Logger.Error(e, "MonitoringProcessor.LoopMonitoring");
				}
			}
		}

		static void UsbRunner_NewResponse(Response response)
		{
			Trace.WriteLine("response.Id=" + response.Id);
			lock (MonitoringDevice.Locker)
			{
				foreach (var monitoringDevice in MonitoringDevices.ToList())
				{
					foreach (var request in monitoringDevice.Requests.ToList())
					{
						if (request != null && request.Id == response.Id)
						{
							monitoringDevice.OnResponceRecieved(request, response);
						}
					}
				}
			}
		}

		#region Комманды для MainManager
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
		#endregion
	}
}