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
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices.Where(x => x.Driver.IsPanel && x.IntAddress == 15))
			{
				if (device.Driver.DriverType == DriverType.Rubezh_2OP)
					MonitoringDevices.Add(new SecMonitoringDevice(device));
				else
					MonitoringDevices.Add(new MonitoringDevice(device));
			}
			DoMonitoring = false;
			ServerHelper.UsbRunner.NewResponse += new Action<Response>(UsbRunner_NewResponse);
		}

		public static void Initialize()
		{
			;
		}

		public static void StartMonitoring()
		{
			foreach (var monitoringDevice in MonitoringDevices)
			{
				monitoringDevice.Initialize();
			}
			//return;

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
			while (true)
			{
				if (DoMonitoring)
				{
					if (MonitoringDevices.Any(x => x.IsReadingNeeded))
					{
						Thread.Sleep(MonitoringDevice.betweenCyclesSpan);
						foreach (var monitoringDevice in MonitoringDevices.Where(x => x.IsReadingNeeded))
						{
							var journalItems = monitoringDevice.GetNewItems();
							DeviceStatesManager.UpdateDeviceState(journalItems);
							DeviceStatesManager.UpdateDeviceStateJournal(journalItems);
							DeviceStatesManager.UpdatePanelState(monitoringDevice.Device);
						}
					}
					foreach (var monitoringDevice in MonitoringDevices)
					{
						if (monitoringDevice.CanLastIndexBeRequested())
						{
							monitoringDevice.RequestLastIndex();
						}
					}
					
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
				Trace.WriteLine(monitoringDevice.Device.PresentationAddress + " " + (monitoringDevice.AnsweredCount / timeSpan.TotalSeconds).ToString() + " " + monitoringDevice.AnsweredCount + "/" + monitoringDevice.UnAnsweredCount);
			}
		}
	}
}