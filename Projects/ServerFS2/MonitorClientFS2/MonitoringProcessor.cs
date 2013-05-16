using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using ServerFS2;
using ServerFS2.DataBase;

namespace MonitorClientFS2
{
	public class MonitoringProcessor
	{
		static readonly int maxMessages = 1024;
		static readonly int maxSecMessages = 1024;
		int UsbRequestNo = 1;
		static readonly object Locker = new object();
		List<DeviceResponceRelation> DeviceResponceRelations = new List<DeviceResponceRelation>();
		bool DoMonitoring;

		public event Action<FSJournalItem> OnNewItems;

		public MonitoringProcessor()
		{
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
				foreach (var deviceResponceRelation in DeviceResponceRelations.Where(x => !x.UnAnswered))
				{
					if (deviceResponceRelation.GetType() == typeof(SecDeviceResponceRelation))
					{
						//SecLastIndexRequest((deviceResponceRelation as SecDeviceResponceRelation));
					}
					else
						LastIndexRequest(deviceResponceRelation);
					Thread.Sleep(100);
				}
				Trace.WriteLine("");
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
			var deviceResponceRelation = DeviceResponceRelations.FirstOrDefault(x => x.Requests.FirstOrDefault(y => y != null && y.Id == response.Id) != null);
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
				else if (request.RequestType == RequestTypes.SecReadIndex)
				{
					SecLastIndexReceived((deviceResponceRelation as SecDeviceResponceRelation), response);
				}
				else if (request.RequestType == RequestTypes.SecReadItem)
				{
					SecNewItemReceived((deviceResponceRelation as SecDeviceResponceRelation), response);
				}
				deviceResponceRelation.Requests.Remove(request);
			}
		}

		void NewItemRequest(DeviceResponceRelation deviceResponceRelation, int ItemIndex)
		{
			++UsbRequestNo;
			deviceResponceRelation.Requests.Add(new Request { Id = UsbRequestNo, RequestType = RequestTypes.ReadItem });
			var bytes = new List<byte> { 0x20, 0x00 };
			bytes.AddRange(BitConverter.GetBytes(ItemIndex).Reverse());
			SendByteCommand(bytes, deviceResponceRelation.Device, UsbRequestNo);
		}

		void NewItemReceived(DeviceResponceRelation deviceResponceRelation, Response response)
		{
			if (!ValidReadItemResponse(response))
				return;
			var journalItem = JournalParser.FSParce(response.Data);
			Trace.WriteLine("ReadItem Responce " + deviceResponceRelation.Device.PresentationAddressAndName);
			DBJournalHelper.AddJournalItem(journalItem);
			OnNewItems(journalItem);
		}

		private static bool ValidReadItemResponse(Response response)
		{
			return response.Data.Count >= 24;
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
			if (!ValidReadIndexResponse(response))
				return;
			var lastDeviceRecord = 256 * response.Data[9] + response.Data[10];
			if (deviceResponceRelation.FirstDisplayedRecord == -1)
				deviceResponceRelation.FirstDisplayedRecord = lastDeviceRecord;
			if (deviceResponceRelation.LastDisplayedRecord == -1)
				deviceResponceRelation.LastDisplayedRecord = lastDeviceRecord;
			Trace.WriteLine(deviceResponceRelation.Device.PresentationAddressAndName + " ReadIndex Response " + (lastDeviceRecord-deviceResponceRelation.FirstDisplayedRecord));
			if (lastDeviceRecord - deviceResponceRelation.LastDisplayedRecord > maxMessages)
			{
				deviceResponceRelation.LastDisplayedRecord = lastDeviceRecord - maxMessages;
			}
			if (lastDeviceRecord > deviceResponceRelation.LastDisplayedRecord)
			{
				Trace.WriteLine("Дочитываю записи с " +
					(deviceResponceRelation.LastDisplayedRecord + 1).ToString() +
					" до " +
					lastDeviceRecord.ToString());
				var thread = new Thread(() =>
				{
					NewItemRequests(deviceResponceRelation, lastDeviceRecord, deviceResponceRelation.LastDisplayedRecord);
				});
				thread.Start();
			}
		}

		private static bool ValidReadIndexResponse(Response response)
		{
			return response.Data.Count() >= 10;
		}

		void NewItemRequests(DeviceResponceRelation deviceResponceRelation, int lastDeviceRecord, int lastDisplayedRecord)
		{
			for (int i = lastDisplayedRecord + 1; i <= lastDeviceRecord; i++)
			{
				NewItemRequest(deviceResponceRelation, i);
				while (deviceResponceRelation.UnAnswered)
				{
					;
				}
			}
			deviceResponceRelation.LastDisplayedRecord = lastDeviceRecord;
		}

		#region Дубликаты для охранных записей

		void SecNewItemRequest(SecDeviceResponceRelation deviceResponceRelation, int ItemIndex)
		{
			++UsbRequestNo;
			deviceResponceRelation.Requests.Add(new Request { Id = UsbRequestNo, RequestType = RequestTypes.SecReadItem });
			var bytes = new List<byte> { 0x20, 0x02 };
			bytes.AddRange(BitConverter.GetBytes(ItemIndex).Reverse());
			SendByteCommand(bytes, deviceResponceRelation.Device, UsbRequestNo);
		}

		void SecNewItemReceived(SecDeviceResponceRelation deviceResponceRelation, Response response)
		{
			Trace.WriteLine("SecReadItem Responce " + deviceResponceRelation.Device.PresentationAddressAndName);
			var journalItem = JournalParser.FSParce(response.Data);
			DBJournalHelper.AddJournalItem(journalItem);
			OnNewItems(journalItem);
		}

		void SecLastIndexRequest(SecDeviceResponceRelation deviceResponceRelation)
		{
			++UsbRequestNo;
			var request = new Request { Id = UsbRequestNo, RequestType = RequestTypes.SecReadIndex };
			deviceResponceRelation.Requests.Add(request);
			SendByteCommand(new List<byte> { 0x21, 0x02 }, deviceResponceRelation.Device, UsbRequestNo);
		}

		void SecLastIndexReceived(SecDeviceResponceRelation deviceResponceRelation, Response response)
		{
			var lastDeviceRecord = 256 * response.Data[9] + response.Data[10];
			Trace.WriteLine("SecReadIndex Response " + lastDeviceRecord);
			if (lastDeviceRecord - deviceResponceRelation.LastDisplayedSecRecord > maxSecMessages)
			{
				deviceResponceRelation.LastDisplayedSecRecord = lastDeviceRecord - maxSecMessages;
			}
			if (lastDeviceRecord > deviceResponceRelation.LastDisplayedSecRecord)
			{
				Trace.WriteLine("Дочитываю записи с " +
					(deviceResponceRelation.LastDisplayedSecRecord + 1).ToString() +
					" до " +
					lastDeviceRecord.ToString());
				var thread = new Thread(() =>
				{
					SecNewItemRequests(deviceResponceRelation, lastDeviceRecord, deviceResponceRelation.LastDisplayedSecRecord);
				});
				thread.Start();
			}
		}

		void SecNewItemRequests(SecDeviceResponceRelation deviceResponceRelation, int lastDeviceRecord, int lastDisplayedRecord)
		{
			for (int i = lastDisplayedRecord + 1; i <= lastDeviceRecord; i++)
			{
				SecNewItemRequest(deviceResponceRelation, i);
				while (deviceResponceRelation.UnAnswered)
				{
					;
				}
			}
			deviceResponceRelation.LastDisplayedSecRecord = lastDeviceRecord;
		}

		#endregion Дубликаты для охранных записей
	}

	public class DeviceResponceRelation
	{
		public DeviceResponceRelation()
		{
			;
		}
		public DeviceResponceRelation(Device device)
		{
			Device = device;
			Requests = new List<Request>();
			LastDisplayedRecord = XmlJournalHelper.GetLastId(device);
			FirstDisplayedRecord = -1;
		}
		public Device Device { get; set; }
		public List<Request> Requests { get; set; }
		public bool UnAnswered { get { return Requests.Count > 0; } }
		public int FirstDisplayedRecord { get; set; }
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

	public class SecDeviceResponceRelation : DeviceResponceRelation
	{
		public SecDeviceResponceRelation(Device device)
		{
			Device = device;
			Requests = new List<Request>();
			LastDisplayedRecord = XmlJournalHelper.GetLastId(device);
			LastDisplayedSecRecord = XmlJournalHelper.GetLastSecId(device);
		}
		int lastDisplayedSecRecord;
		public int LastDisplayedSecRecord
		{
			get { return lastDisplayedSecRecord; }
			set
			{
				lastDisplayedSecRecord = value;
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
		SecReadIndex,
		SecReadItem,
		Other
	}
}