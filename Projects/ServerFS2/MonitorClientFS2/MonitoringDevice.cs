using System.Diagnostics;
using System.Threading;
using FiresecAPI.Models;
using ServerFS2.DataBase;

namespace MonitorClientFS2
{
	public class MonitoringDevice
	{
		public Device Device;
		public int LastDisplayedRecord;
		int LastDeviceRecord;
		int LastDeviceSecRecord;
		public int LastDisplayedSecRecord;
		bool HasSecItems;
		bool DoMonitoring;
		DeviceJournal DeviceJournal;

		public MonitoringDevice(Device device)
		{
			Device = device;
			DeviceJournal = new DeviceJournal();
			DoMonitoring = false;

			LastDeviceRecord = DeviceJournal.GetLastJournalItemId(Device);
			LastDisplayedRecord = LastDeviceRecord - 120;
			//LastDisplayedRecord = DBJournalHelper.GetLastId(Device.DriverUID); // LastDeviceRecord;
			Trace.WriteLine(Device.PresentationAddressAndName + " " + LastDisplayedRecord.ToString());

			//if (device.Driver.DriverType == DriverType.Rubezh_2OP)
			//{
			//    HasSecItems = true;
			//    LastDeviceSecRecord = DeviceJournal.GetLastSecJournalItemId2Op(Device);
			//    LastDisplayedSecRecord = LastDeviceSecRecord;
			//    Trace.WriteLine(Device.PresentationAddressAndName + " " + LastDisplayedRecord.ToString());
			//}
			//else
			HasSecItems = false;
		}

		public void StartMonitoring()
		{
			if (!DoMonitoring)
			{
				DoMonitoring = true;
				var thread = new Thread(Run);
				thread.IsBackground = true;
				thread.Start();
			}
		}

		public void StopMonitoring()
		{
			DoMonitoring = false;
		}

		void Run()
		{
			while (DoMonitoring)
			{
				LastDeviceRecord = DeviceJournal.GetLastJournalItemId(Device);
				LastDeviceSecRecord = DeviceJournal.GetLastSecJournalItemId2Op(Device);

				if (LastDeviceRecord > LastDisplayedRecord)
				{
					ReadNewItems();
					LastDisplayedRecord = LastDeviceRecord;
					LastRecorsdXml.SetLastId(this);
					//DBJournalHelper.SetLastId(Device.Driver.UID, LastDeviceRecord);
				}

				if (HasSecItems && LastDeviceSecRecord > LastDisplayedSecRecord)
				{
					ReadNewSecItems();
					LastDisplayedSecRecord = LastDeviceSecRecord;
					DBJournalHelper.SetLastSecId(Device.Driver.UID, LastDeviceSecRecord);
				}
			}
		}

		void ReadNewItems()
		{
			Trace.Write("Дочитываю записи с " + LastDisplayedRecord.ToString() + " до " + LastDeviceRecord.ToString() + "с прибора " + Device.PresentationName + "\n");
			var newItems = DeviceJournal.GetJournalItems(Device, LastDeviceRecord, LastDisplayedRecord + 1);
			foreach (var journalItem in newItems)
			{
				if (journalItem != null)
				{
					//AddToJournalObservable(journalItem);
					DBJournalHelper.AddJournalItem(journalItem);
					Trace.Write(Device.PresentationAddress + " ");
				}
			}
			Trace.WriteLine(" дочитал");
		}

		void ReadNewSecItems()
		{
			Trace.Write("Дочитываю охранные записи с " + LastDisplayedSecRecord.ToString() + " до " + LastDeviceSecRecord.ToString() + "с прибора " + Device.PresentationName + "\n");
			var newItems = DeviceJournal.GetSecJournalItems2Op(Device, LastDeviceSecRecord, LastDisplayedSecRecord + 1);
			foreach (var journalItem in newItems)
			{
				if (journalItem != null)
				{
					//AddToJournalObservable(journalItem);
					DBJournalHelper.AddJournalItem(journalItem);
					Trace.Write(".");
				}
			}
			Trace.WriteLine(" дочитал");
		}
	}
}