using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ServerFS2;
using ServerFS2.DataBase;

namespace MonitorClientFS2
{
	public class RequestManager
	{
		const int maxMessages = 1024;
		const int maxSecMessages = 1024;
		const int newItemRequestTimeout = 1000;
		const int lastIndexRequestTimeout = 50;

		static readonly object Locker = new object();
		int UsbRequestNo = 1;

		public void NewItemRequests(MonitoringDevice deviceResponceRelation, int lastDeviceRecord, int lastDisplayedRecord)
		{
			deviceResponceRelation.LastDisplayedRecord = lastDeviceRecord;
			for (int i = lastDisplayedRecord + 1; i <= lastDeviceRecord; i++)
			//for (int i = lastDeviceRecord - 10; i <= lastDeviceRecord; i++)
			{
				NewItemRequest(deviceResponceRelation, i);
			}
		}

		public void NewItemRequest(MonitoringDevice deviceResponceRelation, int ItemIndex)
		{
			++UsbRequestNo;
			var bytes = new List<byte> { 0x20, 0x00 };
			bytes.AddRange(BitConverter.GetBytes(ItemIndex).Reverse());
			Request request = new Request(UsbRequestNo, RequestTypes.ReadItem, bytes, newItemRequestTimeout);
			deviceResponceRelation.SendRequest(request);
		}

		public void NewItemReceived(MonitoringDevice deviceResponceRelation, Response response)
		{
			if (!NewItemValid(response))
				return;
			var journalItem = JournalParser.FSParce(response.Data);
			Trace.WriteLine("ReadItem Responce " + deviceResponceRelation.Device.PresentationAddressAndName + " " + UsbRequestNo);
			DBJournalHelper.AddJournalItem(journalItem);
			//OnNewItems(journalItem);
		}

		bool NewItemValid(Response response)
		{
			return response.Data.Count >= 24;
		}

		public void LastIndexRequest(MonitoringDevice deviceResponceRelation)
		{
			++UsbRequestNo;
			var request = new Request(UsbRequestNo, RequestTypes.ReadIndex, new List<byte> { 0x21, 0x00 }, lastIndexRequestTimeout);
			deviceResponceRelation.SendRequest(request);
		}

		public void LastIndexReceived(MonitoringDevice deviceResponceRelation, Response response)
		{
			if (!LastIndexValid(response))
				return;
			var lastDeviceRecord = 256 * response.Data[9] + response.Data[10];
			if (deviceResponceRelation.FirstDisplayedRecord == -1)
				deviceResponceRelation.FirstDisplayedRecord = lastDeviceRecord;
			if (deviceResponceRelation.LastDisplayedRecord == -1)
				deviceResponceRelation.LastDisplayedRecord = lastDeviceRecord;
			Trace.WriteLine(deviceResponceRelation.Device.PresentationAddressAndName + " ReadIndex Response " + (lastDeviceRecord - deviceResponceRelation.FirstDisplayedRecord));
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

		bool LastIndexValid(Response response)
		{
			return response.Data.Count() >= 10;
		}

		//#region Дубликаты для охранных записей

		//void SecNewItemRequest(SecDeviceResponceRelation deviceResponceRelation, int ItemIndex)
		//{
		//    ++UsbRequestNo;
		//    deviceResponceRelation.Requests.Add(new Request { Id = UsbRequestNo, RequestType = RequestTypes.SecReadItem });
		//    var bytes = new List<byte> { 0x20, 0x02 };
		//    bytes.AddRange(BitConverter.GetBytes(ItemIndex).Reverse());
		//    SendByteCommand(bytes, deviceResponceRelation.Device, UsbRequestNo);
		//}

		//void SecNewItemReceived(SecDeviceResponceRelation deviceResponceRelation, Response response)
		//{
		//    Trace.WriteLine("SecReadItem Responce " + deviceResponceRelation.Device.PresentationAddressAndName);
		//    var journalItem = JournalParser.FSParce(response.Data);
		//    DBJournalHelper.AddJournalItem(journalItem);
		//    OnNewItems(journalItem);
		//}

		//void SecLastIndexRequest(SecDeviceResponceRelation deviceResponceRelation)
		//{
		//    ++UsbRequestNo;
		//    var request = new Request { Id = UsbRequestNo, RequestType = RequestTypes.SecReadIndex };
		//    deviceResponceRelation.Requests.Add(request);
		//    SendByteCommand(new List<byte> { 0x21, 0x02 }, deviceResponceRelation.Device, UsbRequestNo);
		//}

		//void SecLastIndexReceived(SecDeviceResponceRelation deviceResponceRelation, Response response)
		//{
		//    var lastDeviceRecord = 256 * response.Data[9] + response.Data[10];
		//    Trace.WriteLine("SecReadIndex Response " + lastDeviceRecord);
		//    if (lastDeviceRecord - deviceResponceRelation.LastDisplayedSecRecord > maxSecMessages)
		//    {
		//        deviceResponceRelation.LastDisplayedSecRecord = lastDeviceRecord - maxSecMessages;
		//    }
		//    if (lastDeviceRecord > deviceResponceRelation.LastDisplayedSecRecord)
		//    {
		//        Trace.WriteLine("Дочитываю записи с " +
		//            (deviceResponceRelation.LastDisplayedSecRecord + 1).ToString() +
		//            " до " +
		//            lastDeviceRecord.ToString());
		//        var thread = new Thread(() =>
		//        {
		//            SecNewItemRequests(deviceResponceRelation, lastDeviceRecord, deviceResponceRelation.LastDisplayedSecRecord);
		//        });
		//        thread.Start();
		//    }
		//}

		//void SecNewItemRequests(SecDeviceResponceRelation deviceResponceRelation, int lastDeviceRecord, int lastDisplayedRecord)
		//{
		//    for (int i = lastDisplayedRecord + 1; i <= lastDeviceRecord; i++)
		//    {
		//        SecNewItemRequest(deviceResponceRelation, i);
		//        while (deviceResponceRelation.UnAnswered)
		//        {
		//            ;
		//        }
		//    }
		//    deviceResponceRelation.LastDisplayedSecRecord = lastDeviceRecord;
		//}

		//#endregion Дубликаты для охранных записей

		//void CheckForUnAnswered(DeviceResponceRelation deviceResponceRelation, int RequestId, int timeOut = 1)
		//{
		//    Thread.Sleep(timeOut);
		//    Request request = deviceResponceRelation.Requests.FirstOrDefault(x => x.Id == RequestId);
		//    if (request != null)
		//    {
		//        Trace.WriteLine("Request Expired:" + deviceResponceRelation.Device.PresentationAddressAndName + " " + request.Id + " " + request.RequestType);
		//        deviceResponceRelation.Requests.Remove(request);
		//    }
		//}
	}
}