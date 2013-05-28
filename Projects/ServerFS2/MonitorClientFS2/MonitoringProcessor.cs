using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using ServerFS2;

namespace MonitorClientFS2
{
	public class MonitoringProcessor
	{
		List<MonitoringDevice> MonitoringDevices = new List<MonitoringDevice>();
		bool DoMonitoring;
		DateTime StartTime;
		
		public MonitoringProcessor()
		{
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices.Where(x => x.Driver.IsPanel))
			{
				if (device.Driver.DriverType == DriverType.Rubezh_2OP)
					MonitoringDevices.Add(new SecDeviceResponceRelation(device));
				else
					MonitoringDevices.Add(new MonitoringDevice(device));
			}
			DoMonitoring = false;
			ServerHelper.UsbRunner.NewResponse += new Action<Response>(UsbRunner_NewResponse);
			StartMonitoring();
		}

		public void StartMonitoring()
		{
			if (!DoMonitoring)
			{
				StartTime = DateTime.Now;
				DoMonitoring = true;
				var thread = new Thread(OnRun);
				thread.Start();
			}
		}

		public void StopMonitoring()
		{
			DoMonitoring = false;
		}

		void OnRun()
		{
			while (DoMonitoring)
			{
				//foreach (var monitoringDevice in MonitoringDevices.Where(x => x.IsMonitoringAllowed))
				//foreach (var monitoringDevice in MonitoringDevices)
				//{
				//    //if (deviceResponceRelation.GetType() == typeof(SecDeviceResponceRelation))
				//    //{
				//    //    //SecLastIndexRequest((deviceResponceRelation as SecDeviceResponceRelation));
				//    //}
				//    //else
				//    //{
				//    //    monitoringDevice.LastIndexRequest();
				//    //}
				//    monitoringDevice.RequestLastIndex();
				//}
				//MonitoringDevices.ForEach(x => x.CheckForBusy());

				MonitoringDevices.FirstOrDefault(x => x.Device.Driver.DriverType == DriverType.Rubezh_2AM).RequestLastIndex();

				Thread.Sleep(MonitoringDevice.betweenCyclesSpan);
			}
		}

		void UsbRunner_NewResponse(Response response)
		{
			MonitoringDevice monitoringDevice = null;
			Request request = null;
			lock (MonitoringDevice.Locker)
			{
				//var deviceResponceRelation = MonitoringDevices.FirstOrDefault(x => x.Requests.FirstOrDefault(y => y != null && y.Id == response.Id) != null);

				foreach (var monitoringDevicesItem in MonitoringDevices.ToList())
				{
					foreach (var requestsItem in monitoringDevicesItem.Requests.ToList())
					{
						if (requestsItem.Id == response.Id)
						{
							request = requestsItem;
							monitoringDevice = monitoringDevicesItem;
						}
					}
				}
			}
			if (monitoringDevice != null && request != null)
			{
				monitoringDevice.AnsweredCount++;
				if (request.RequestType == RequestTypes.ReadIndex)
				{
					monitoringDevice.LastIndexReceived(response);
				}
				else if (request.RequestType == RequestTypes.ReadItem)
				{
					monitoringDevice.NewItemReceived(response);
				}
				//else if (request.RequestType == RequestTypes.SecReadIndex)
				//{
				//    SecLastIndexReceived((deviceResponceRelation as SecDeviceResponceRelation), response);
				//}
				//else if (request.RequestType == RequestTypes.SecReadItem)
				//{
				//    SecNewItemReceived((deviceResponceRelation as SecDeviceResponceRelation), response);
				//}
				monitoringDevice.Requests.Remove(request);
			}
		}

		public void WriteStats()
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