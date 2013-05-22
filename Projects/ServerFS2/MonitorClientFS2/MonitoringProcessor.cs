using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using ServerFS2;

namespace MonitorClientFS2
{
	public class MonitoringProcessor
	{
		List<DeviceResponceRelation> DeviceResponceRelations = new List<DeviceResponceRelation>();
		bool DoMonitoring;
		RequestManager RequestManager;

		public MonitoringProcessor()
		{
			RequestManager = new RequestManager();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices.Where(x => x.Driver.IsPanel))
			{
				if (device.Driver.DriverType == DriverType.Rubezh_2OP)
					DeviceResponceRelations.Add(new SecDeviceResponceRelation(device));
				else
					DeviceResponceRelations.Add(new DeviceResponceRelation(device));
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
				//{
				//    //if (deviceResponceRelation.GetType() == typeof(SecDeviceResponceRelation))
				//    //{
				//    //    //SecLastIndexRequest((deviceResponceRelation as SecDeviceResponceRelation));
				//    //}
				//    //else
				//    RequestManager.LastIndexRequest(deviceResponceRelation);
				//}
				RequestManager.LastIndexRequest(DeviceResponceRelations.FirstOrDefault());
				//Trace.WriteLine("");
				//Thread.Sleep(1000);
			}
		}

		void UsbRunner_NewResponse(Response response)
		{
			var deviceResponceRelation = DeviceResponceRelations.FirstOrDefault(x => x.Requests.FirstOrDefault(y => y != null && y.Id == response.Id) != null);
			if (deviceResponceRelation != null)
			{
				var request = deviceResponceRelation.Requests.FirstOrDefault(y => y.Id == response.Id);
				if (request.RequestType == RequestTypes.ReadIndex)
				{
					RequestManager.LastIndexReceived(deviceResponceRelation, response);
				}
				else if (request.RequestType == RequestTypes.ReadItem)
				{
					RequestManager.NewItemReceived(deviceResponceRelation, response);
				}
				//else if (request.RequestType == RequestTypes.SecReadIndex)
				//{
				//    SecLastIndexReceived((deviceResponceRelation as SecDeviceResponceRelation), response);
				//}
				//else if (request.RequestType == RequestTypes.SecReadItem)
				//{
				//    SecNewItemReceived((deviceResponceRelation as SecDeviceResponceRelation), response);
				//}
				//deviceResponceRelation.Requests.Remove(request);
			}
		}
	}
}