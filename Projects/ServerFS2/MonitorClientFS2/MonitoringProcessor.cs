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
		int UsbRequestNo = 1;
		static readonly object Locker = new object();
		List<DeviceResponceRelation> DeviceResponceRelations = new List<DeviceResponceRelation>();

		public MonitoringProcessor()
		{
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices.Where(x => x.Driver.IsPanel && x.Driver.DriverType == DriverType.Rubezh_2OP))
			{
				DeviceResponceRelations.Add(new DeviceResponceRelation(device));
			}
			ServerHelper.UsbRunner.NewResponse += new Action<Response>(UsbRunner_NewResponse);
			var thread = new Thread(OnRun);
			thread.Start();
		}

		void OnRun()
		{
			while (true)
			{
				foreach (var deviceResponceRelation in DeviceResponceRelations)
				{
					LastIndexRequest(deviceResponceRelation); //GetLastId
					Thread.Sleep(1000);
				}
				Thread.Sleep(1000);
			}
		}

		void SendByteCommand(List<byte> commandBytes, Device device, int requestId)
		{
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(requestId).Reverse());
			bytes.Add(GetSheifByte(device));
			bytes.Add(Convert.ToByte(device.AddressOnShleif));
			bytes.Add(0x01);
			bytes.AddRange(commandBytes);
			ServerHelper.SendCodeAsync(bytes);
		}

		byte GetSheifByte(Device device)
		{
			return 0x03;
		}

		void UsbRunner_NewResponse(Response response)
		{
			var deviceResponceRelation = DeviceResponceRelations.FirstOrDefault(x => x.Requests.FirstOrDefault(y => y.Id == response.Id) != null);
			if (deviceResponceRelation != null)
			{
				var request = deviceResponceRelation.Requests.FirstOrDefault(y => y.Id == response.Id);
				if (request.RequestType == RequestTypes.ReadIndex)
				{
					LastIndexReceived(deviceResponceRelation, response);
				}
				else if (request.RequestType == RequestTypes.ReadItem)
				{
					NewItemReceived(deviceResponceRelation, response);
				}
				else
				{
					;
				}
			}
		}

		void NewItemRequest(DeviceResponceRelation deviceResponceRelation, int ItemIndex)
		{
			++UsbRequestNo;
			deviceResponceRelation.Requests.Add(new Request { Id = UsbRequestNo, RequestType = RequestTypes.ReadItem });
			var bytes = new List<byte> { 0x20, 0x00 };
			bytes.AddRange(BitConverter.GetBytes(ItemIndex).Reverse());
			SendByteCommand(bytes, deviceResponceRelation.Device, UsbRequestNo);
			Thread.Sleep(2000);
		}

		void NewItemReceived(DeviceResponceRelation deviceResponceRelation, Response response)
		{
			var journalItem = JournalParser.FSParce(response.Data);
			//journalItem.Display
			Trace.WriteLine("ReadItem Responce " + journalItem.Description);
		}

		void LastIndexRequest(DeviceResponceRelation deviceResponceRelation)
		{
			++UsbRequestNo;
			var request = new Request { Id = UsbRequestNo, RequestType = RequestTypes.ReadIndex };
			deviceResponceRelation.Requests.Add(request);
			SendByteCommand(new List<byte> { 0x21, 0x00 }, deviceResponceRelation.Device, UsbRequestNo);
		}

		void LastIndexReceived(DeviceResponceRelation deviceResponceRelation, Response response)
		{
			var lastDeviceRecord = 256 * response.Data[9] + response.Data[10];
			Trace.WriteLine("ReadIndex Response " + lastDeviceRecord);
			var lastDisplayedRecord = deviceResponceRelation.LastDisplayedRecord;
			if (lastDisplayedRecord == -1)
			{
				deviceResponceRelation.LastDisplayedRecord = lastDeviceRecord;
			}
			else if (lastDeviceRecord > lastDisplayedRecord)
			{
				Trace.WriteLine("Дочитываю записи с " + (lastDisplayedRecord + 1).ToString() + " до " + lastDeviceRecord.ToString());
				//for (int i = lastDisplayedRecord + 1; i <= lastDeviceRecord; i++)
				for (int i = lastDeviceRecord - 10; i <= lastDeviceRecord; i++)
				{
					NewItemRequest(deviceResponceRelation, i);
				}
				deviceResponceRelation.LastDisplayedRecord = lastDeviceRecord;
			}
		}
	}

	public class DeviceResponceRelation
	{
		public DeviceResponceRelation(Device device)
		{
			Device = device;
			Requests = new List<Request>();
			LastDisplayedRecord = XmlJournalHelper.GetLastId(device);
		}
		public Device Device { get; set; }
		public List<Request> Requests { get; set; }
		int lastDisplayedRecord;
		public int LastDisplayedRecord
		{
			get { return lastDisplayedRecord; }
			set
			{
				lastDisplayedRecord = value;
				XmlJournalHelper.SetLastId(Device, value);
			}
		}
	}

	public class Request
	{
		public int Id { get; set; }
		public RequestTypes RequestType { get; set; }
	}

	public enum RequestTypes
	{
		ReadIndex,
		ReadItem,
		Other
	}
}