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
		//RequestManager RequestManager;

		public MonitoringProcessor()
		{
			//RequestManager = new RequestManager();
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
				//foreach (var deviceResponceRelation in DeviceResponceRelations.Where(x => !x.UnAnswered))
				foreach (var monitoringDevice in MonitoringDevices)
				{
					//if (deviceResponceRelation.GetType() == typeof(SecDeviceResponceRelation))
					//{
					//    //SecLastIndexRequest((deviceResponceRelation as SecDeviceResponceRelation));
					//}
					//else
					//lock (Locker)
					//{
					//    monitoringDevice.LastIndexRequest();
					//}
					monitoringDevice.LastIndexRequest();
				}

				//MonitoringDevices.FirstOrDefault().LastIndexRequest();

				//Trace.WriteLine("");
				Thread.Sleep(10);
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
				monitoringDevice.Answered++;
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
			foreach (var monitoringDevice in MonitoringDevices)
			{
				Trace.WriteLine(monitoringDevice.Device.PresentationAddress + " " + monitoringDevice.Answered + "/" + monitoringDevice.UnAnswered);
			}
		}
	}
}