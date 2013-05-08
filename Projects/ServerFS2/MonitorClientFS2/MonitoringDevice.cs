using System.Diagnostics;
using System.Threading;
using FiresecAPI.Models;
using ServerFS2.DataBase;

namespace MonitorClientFS2
{
	public class MonitoringDevice
	{
		bool HasSecItems;
		bool DoMonitoring;
		DeviceJournal DeviceJournal;

		Device device;
		public Device Device
		{
			get { return device; }
		}
		int lastDisplayedRecord;
		public int LastDisplayedRecord
		{
			get { return lastDisplayedRecord; }
		}
		int lastDisplayedSecRecord;
		public int LastDisplayedSecRecord
		{
			get { return lastDisplayedSecRecord; }
		}
		int lastDeviceRecord;
		public int LastDeviceRecord
		{
			get { return lastDeviceRecord; }
		}
		int lastDeviceSecRecord;
		public int LastDeviceSecRecord
		{
			get { return lastDeviceSecRecord; }
		}

		public MonitoringDevice(Device device)
		{
			this.device = device;
			DeviceJournal = new DeviceJournal();
			DoMonitoring = false;

			lastDeviceRecord = DeviceJournal.GetLastJournalItemId(device);
			lastDisplayedRecord = lastDeviceRecord;
			lastDisplayedRecord = XmlJournalHelper.GetLastId(this);
			if (lastDisplayedRecord == -1)
			{
				lastDisplayedRecord = lastDeviceRecord;
				XmlJournalHelper.SetLastId(this);
			}
			Trace.WriteLine(device.PresentationAddressAndName + " " + lastDisplayedRecord.ToString());

			if (device.Driver.DriverType == DriverType.Rubezh_2OP)
			{
				HasSecItems = true;
				lastDeviceSecRecord = DeviceJournal.GetLastSecJournalItemId2Op(Device);
				lastDisplayedSecRecord = XmlJournalHelper.GetLastSecId(this);
				if (lastDisplayedSecRecord == -1)
				{
					lastDisplayedSecRecord = lastDeviceSecRecord;
					XmlJournalHelper.SetLastSecId(this);
				}
				Trace.WriteLine(Device.PresentationAddressAndName + " " + LastDisplayedSecRecord.ToString());
			}
			else
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
				lastDeviceRecord = DeviceJournal.GetLastJournalItemId(device);
				lastDeviceSecRecord = DeviceJournal.GetLastSecJournalItemId2Op(device);

				if (lastDeviceRecord > lastDisplayedRecord)
				{
					ReadNewItems();
					lastDisplayedRecord = lastDeviceRecord;
					XmlJournalHelper.SetLastId(this);
				}

				if (HasSecItems && lastDeviceSecRecord > lastDisplayedSecRecord)
				{
					ReadNewSecItems();
					lastDisplayedSecRecord = lastDeviceSecRecord;
					XmlJournalHelper.SetLastSecId(this);
				}
			}
		}

		void ReadNewItems()
		{
			Trace.Write("Дочитываю записи с " + lastDisplayedRecord.ToString() + " до " + lastDeviceRecord.ToString() + "с прибора " + device.PresentationName + "\n");
			var newItems = DeviceJournal.GetJournalItems(device, lastDeviceRecord, lastDisplayedRecord + 1);
			foreach (var journalItem in newItems)
			{
				if (journalItem != null)
				{
					//AddToJournalObservable(journalItem);
					DBJournalHelper.AddJournalItem(journalItem);
					Trace.Write(device.PresentationAddress + " ");
				}
			}
			Trace.WriteLine(" дочитал");
		}

		void ReadNewSecItems()
		{
			Trace.Write("Дочитываю охранные записи с " + lastDisplayedSecRecord.ToString() + " до " + lastDeviceSecRecord.ToString() + "с прибора " + device.PresentationName + "\n");
			var newItems = DeviceJournal.GetSecJournalItems2Op(device, lastDeviceSecRecord, lastDisplayedSecRecord + 1);
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