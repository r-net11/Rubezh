using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using FS2Api;
using Common;
using System.Windows.Forms;

namespace ServerFS2.Monitoring
{
	public static partial class MonitoringProcessor
	{
		static List<MonitoringDevice> MonitoringDevices;
		static DateTime StartTime;
		static object locker = new object();

		public static void Initialize()
		{
			MonitoringDevices = new List<MonitoringDevice>();
			foreach (var device in ConfigurationManager.Devices.Where(x => DeviceStatesManager.IsMonitoringable(x)))
			{
				MonitoringDevices.Add(new MonitoringDevice(device));
			}
			foreach (var usbProcessorInfo in USBManager.UsbProcessorInfos)
			{
				//usbProcessorInfo.UsbProcessor.NewResponse -= new Action<Response>(UsbRunner_NewResponse);
				usbProcessorInfo.UsbProcessor.NewResponse += new Action<Response>(UsbRunner_NewResponse);
			}
		}

		static void OnRun()
		{
			try
			{
				if (USBManager.UsbProcessorInfos.Count == 0)
				{
					MessageBox.Show("USBManager.UsbProcessorInfos.Count == 0");
				}
				DeviceStatesManager.SetInitializingStateToAll();
				DeviceStatesManager.SetMonitoringDisabled();
				MonitoringDevices.Where(x => !x.IsInitialized).ToList().ForEach(x => x.Initialize());
				DeviceStatesManager.RemoveInitializingFromAll();
			}
			catch (FS2StopMonitoringException)
			{
				return;
			}
			catch (Exception e)
			{
				Logger.Error(e, "MonitoringProcessor.OnRun");
			}

			while (true)
			{
				try
				{
					if (CheckSuspending(false))
						return;

					foreach (var monitoringDevice in MonitoringDevices)
					{
						if (CheckSuspending(false))
							return;
						monitoringDevice.CheckTasks();
					}

					if (CheckSuspending(false))
						return;

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
			//Trace.WriteLine("response.Id=" + response.Id);
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