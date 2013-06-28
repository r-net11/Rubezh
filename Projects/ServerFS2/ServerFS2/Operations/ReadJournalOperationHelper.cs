using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FS2Api;
using ServerFS2.Service;
using Device = FiresecAPI.Models.Device;
using FiresecAPI.Models;

namespace ServerFS2
{
	public static class ReadJournalOperationHelper
	{
		static readonly object Locker = new object();

		public static FS2JournalItem ParseJournal(Device device, List<byte> bytes)
		{
			lock (Locker)
			{
				var journalParser = new JournalParser();
				var journalItem = journalParser.Parce(device, bytes);
				return journalItem;
			}
		}

		//public static List<FS2JournalItem> GetSecJournalItems2Op(Device device)
		//{
		//    int lastIndex = GetLastSecJournalItemId2Op(device);
		//    var journalItems = new List<FS2JournalItem>();
		//    for (int i = 0; i <= lastIndex; i++)
		//    {
		//        var response = USBManager.Send(device, 0x01, 0x20, 0x02, BitConverter.GetBytes(i).Reverse());
		//        if (response == null) continue;
		//        var journalItem = ParseJournal(device, response.Bytes);
		//        journalItems.Add(journalItem);
		//    }
		//    journalItems = journalItems.OrderByDescending(x => x.IntDate).ToList();
		//    var no = 0;
		//    foreach (var journalItem in journalItems)
		//    {
		//        no++;
		//        journalItem.No = no;
		//    }
		//    return journalItems;
		//}

		//public static int GetLastSecJournalItemId2Op(Device device)
		//{
		//    try
		//    {
		//        var lastindex = USBManager.Send(device, 0x01, 0x21, 0x02);
		//        return BytesHelper.ExtractInt(lastindex.Bytes, 0);
		//    }
		//    catch (NullReferenceException ex)
		//    {
		//        MessageBox.Show(ex.Message);
		//        throw;
		//    }
		//}

		//public static int GetJournalCount(Device device)
		//{
		//    try
		//    {
		//        var response = USBManager.Send(device, 0x01, 0x24, 0x01);
		//        return BytesHelper.ExtractShort(response.Bytes, 0);
		//    }
		//    catch (NullReferenceException ex)
		//    {
		//        MessageBox.Show(ex.Message);
		//        throw;
		//    }
		//}

		//public static int GetFirstJournalItemId(Device device)
		//{
		//    var li = GetLastJournalItemId(device);
		//    var count = GetJournalCount(device);
		//    return li - count + 1;
		//}

		//public static int GetLastJournalItemId(Device device)
		//{
		//    try
		//    {
		//        var response = USBManager.Send(device, 0x01, 0x21, 0x00);
		//        return BytesHelper.ExtractInt(response.Bytes, 0);
		//    }
		//    catch (NullReferenceException ex)
		//    {
		//        MessageBox.Show(ex.Message);
		//        throw;
		//    }
		//}

		//public static FS2JournalItemsCollection GetJournalItems(Device device)
		//{
		//    var result = new FS2JournalItemsCollection();
		//    CallbackManager.AddProgress(new FS2ProgressInfo("Чтение индекса последней записи"));
		//    int lastIndex = GetLastJournalItemId(device);
		//    CallbackManager.AddProgress(new FS2ProgressInfo("Чтение индекса первой записи"));
		//    int firstIndex = GetFirstJournalItemId(device);
		//    if (device.Driver.DriverType == DriverType.Rubezh_2OP || device.Driver.DriverType == DriverType.USB_Rubezh_2OP)
		//    {
		//        result.SecurityJournalItems = GetSecJournalItems2Op(device);
		//    }
		//    for (int i = firstIndex; i <= lastIndex; i++)
		//    {
		//        CallbackManager.AddProgress(new FS2ProgressInfo("Чтение записей журнала " + (i - firstIndex).ToString() + " из " + (lastIndex - firstIndex + 1).ToString(),
		//            (i - firstIndex) * 100 / (lastIndex - firstIndex)));
		//        var response = USBManager.Send(device, 0x01, 0x20, 0x00, BitConverter.GetBytes(i).Reverse());
		//        if (response == null) continue;
		//        var journalItem = ParseJournal(device, response.Bytes);
		//        if (journalItem != null)
		//        {
		//            result.FireJournalItems.Add(journalItem);
		//        }
		//    }
		//    for(int i = 0; i < result.FireJournalItems.Count; i++)
		//    {
		//        result.FireJournalItems[i].No = i + 1;
		//    }
		//    for (int i = 0; i < result.SecurityJournalItems.Count; i++)
		//    {
		//        result.FireJournalItems[i].No = i + 1;
		//    }
		//    return result;
		//}

		public static FS2JournalItemsCollection GetJournalItemsCollection(Device device)
		{
			var result = new FS2JournalItemsCollection();
			result.FireJournalItems = GetJournalItems(device, 0);
			if (device.Driver.DriverType == DriverType.Rubezh_2OP || device.Driver.DriverType == DriverType.USB_Rubezh_2OP)
			{
				result.SecurityJournalItems = GetJournalItems(device, 2);
			}
			return result;
		}

		static List<FS2JournalItem> GetJournalItems(Device device, int journalType)
		{
			var result = new List<FS2JournalItem>();

			CallbackManager.AddProgress(new FS2ProgressInfo("Чтение индекса последней записи"));
			var response = USBManager.Send(device, 0x01, 0x21, journalType);
			if (response.HasError)
				return null;
			int lastIndex = BytesHelper.ExtractInt(response.Bytes, 0);

			CallbackManager.AddProgress(new FS2ProgressInfo("Чтение индекса первой записи"));
			response = USBManager.Send(device, 0x01, 0x24, 0x01);
			if (response.HasError)
				return null;
			var count = BytesHelper.ExtractShort(response.Bytes, 0);

			int firstIndex = lastIndex - count + 1;

			for (int i = firstIndex; i <= lastIndex; i++)
			{
				CallbackManager.AddProgress(new FS2ProgressInfo("Чтение записей журнала " + (i - firstIndex).ToString() + " из " + (lastIndex - firstIndex + 1).ToString(),
					(i - firstIndex) * 100 / (lastIndex - firstIndex)));
				response = USBManager.Send(device, 0x01, 0x20, 0x00, BitConverter.GetBytes(i).Reverse());
				if (response != null)
				{
					var journalItem = ParseJournal(device, response.Bytes);
					if (journalItem != null)
					{
						result.Add(journalItem);
					}
				}
			}
			for (int i = 0; i < result.Count; i++)
			{
				result[i].No = i + 1;
			}
			return result;
		}
	}
}