using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using ServerFS2;

namespace ServerFS2.Monitor
{
	public static class MonitoringProcessor
	{
		static List<MonitoringDevice> MonitoringDevices = new List<MonitoringDevice>();
		public static bool DoMonitoring{get; set;}
		static DateTime StartTime;
		static object locker = new object();

		static MonitoringProcessor()
		{
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices.Where(x => DeviceStatesManager.IsMonitoringable(x)))
			{
				MonitoringDevices.Add(new MonitoringDevice(device));
				//if (device.Driver.DriverType == DriverType.Rubezh_2OP)
				//    MonitoringDevices.Add(new SecMonitoringDevice(device));
				//else
				//    MonitoringDevices.Add(new MonitoringDevice(device));
			}
			DoMonitoring = false;
			ServerHelper.UsbRunnerBase.NewResponse += new Action<Response>(UsbRunner_NewResponse);
		}

		public static void StartMonitoring()
		{
			if (!DoMonitoring)
			{
				StartTime = DateTime.Now;
				DoMonitoring = true;
				var thread = new Thread(OnRun);
				thread.Start();
			}
		}

		public static void StopMonitoring()
		{
			DoMonitoring = false;
		}

		static void OnRun()
		{
			foreach (var monitoringDevice in MonitoringDevices.Where(x => !x.IsInitialized))
			{
				monitoringDevice.Initialize();
			}
			//return;
			while (true)
			{
				if (DoMonitoring)
				{
					foreach (var monitoringDevice in MonitoringDevices.Where(x => x.IsReadingNeeded))
					{
						var journalItems = monitoringDevice.GetNewItems();
						DeviceStatesManager.UpdateDeviceStateJournal(journalItems);
						DeviceStatesManager.UpdateDeviceState(journalItems);
						DeviceStatesManager.UpdatePanelState(monitoringDevice.Panel);
					}
					foreach (var monitoringDevice in MonitoringDevices.Where(x => x.StatesToReset != null && x.StatesToReset.Count > 0))
					{
						foreach (var state in monitoringDevice.StatesToReset)
						{
							DeviceStatesManager.ResetState(state, monitoringDevice);
						}
						monitoringDevice.StatesToReset = new List<DriverState>();
						DeviceStatesManager.UpdatePanelState(monitoringDevice.Panel);
					}
					foreach (var monitoringDevice in MonitoringDevices.Where(x => x.DevicesToIgnore != null && x.DevicesToIgnore.Count > 0))
					{
						foreach (var deviceToIgnore in monitoringDevice.DevicesToIgnore)
						{
							ServerHelper.SendCodeToPanel(monitoringDevice.Panel, 0x02, 0x54, 0x0B, 0x01, 0x00, monitoringDevice.Panel.IntAddress, 0x00, 0x00, 0x00, deviceToIgnore.ShleifNo - 1);
						}
						monitoringDevice.DevicesToIgnore = new List<Device>();
						DeviceStatesManager.UpdatePanelState(monitoringDevice.Panel);
					}
					foreach (var monitoringDevice in MonitoringDevices.Where(x => x.DevicesToResetIgnore != null && x.DevicesToResetIgnore.Count > 0))
					{
						foreach (var deviceToIgnore in monitoringDevice.DevicesToResetIgnore)
						{
							ServerHelper.SendCodeToPanel(monitoringDevice.Panel, 0x02, 0x54, 0x0B, 0x00, 0x00, monitoringDevice.Panel.IntAddress, 0x00, 0x00, 0x00, deviceToIgnore.ShleifNo - 1);
						}
						monitoringDevice.DevicesToResetIgnore = new List<Device>();
						DeviceStatesManager.UpdatePanelState(monitoringDevice.Panel);
					}
					MonitoringDevices.Where(x => x.IsStateRefreshNeeded).ToList().ForEach(x => 
						{
							x.RefreshStates();
						});
					MonitoringDevices.ForEach(x => x.CheckForLostConnection());
					MonitoringDevices.ForEach(x => x.RequestLastIndex());
				}
			}
		}

		public static void AddStateToReset(Device device, DriverState state)
		{
			foreach (var monitoringDevice in MonitoringDevices)
			{
				if (monitoringDevice.Panel == device)
					monitoringDevice.StatesToReset.Add(state);
				break;
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

		static void UsbRunner_NewResponse(Response response)
		{
			MonitoringDevice monitoringDevice = null;
			Request request = null;
			lock (MonitoringDevice.Locker)
			{
				foreach (var monitoringDevicesItem in MonitoringDevices.ToList())
				{
					foreach (var requestsItem in monitoringDevicesItem.Requests.ToList())
					{
						if (requestsItem != null && requestsItem.Id == response.Id)
						{
							request = requestsItem;
							monitoringDevice = monitoringDevicesItem;
						}
					}
				}
			}
			if (request != null)
			{
				monitoringDevice.AnsweredCount++;
				if (request.RequestType == RequestTypes.ReadIndex)
				{
					monitoringDevice.LastIndexReceived(response);
				}
				//else if (request.RequestType == RequestTypes.SecReadIndex)
				//{
				//    SecLastIndexReceived((deviceResponceRelation as SecDeviceResponceRelation), response);
				//}
				//else if (request.RequestType == RequestTypes.SecReadItem)
				//{
				//    SecNewItemReceived((deviceResponceRelation as SecDeviceResponceRelation), response);
				//}
				lock(MonitoringDevice.Locker)
					monitoringDevice.Requests.RemoveAll(x => x != null && x.Id == request.Id);
			}
		}

		public static void WriteStats()
		{
			var timeSpan = DateTime.Now - StartTime;
			Trace.WriteLine("betweenCyclesSpan "+MonitoringDevice.betweenCyclesSpan);
			Trace.WriteLine("betweenDevicesSpan " + MonitoringDevice.betweenDevicesSpan);
			Trace.WriteLine("testTime " + timeSpan);
			foreach (var monitoringDevice in MonitoringDevices)
			{
				Trace.WriteLine(monitoringDevice.Panel.PresentationAddress + " " + (monitoringDevice.AnsweredCount / timeSpan.TotalSeconds).ToString() + " " + monitoringDevice.AnsweredCount + "/" + monitoringDevice.UnAnsweredCount);
			}
		}
	}
}