using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using MonitorClientFS2.ViewModels;
using ServerFS2;
using ServerFS2.DataBase;

namespace MonitorClientFS2
{
	public class MainViewModel : BaseViewModel
	{
		public class DeviceLastRecord
		{
			Device device;
			int lastDisplayedRecord;
			int lastDeviceRecord;
			//int LastSecRecord;

			public DeviceLastRecord(Device _device)
			{
				device = _device;
				lastDeviceRecord = JournalHelper.GetLastJournalItemId(device);
				lastDisplayedRecord = lastDeviceRecord;
				Trace.WriteLine(device.PresentationAddressAndName + " " + lastDisplayedRecord.ToString());
			}

			public void StartMonitoring()
			{
				var thread = new Thread(() =>
					{
						while (true)
						{
							lock (Locker)
							{
								lastDeviceRecord = JournalHelper.GetLastJournalItemId(device);
							}
							if (lastDeviceRecord > lastDisplayedRecord)
							{
								ReadNewItems();
								lastDisplayedRecord = lastDeviceRecord;
								DBJournalHelper.SetLastId(new Guid(), lastDeviceRecord);
							}
						}
					});
				thread.Start();
			}

			void ReadNewItems()
			{
				Trace.Write("Дочитываю записи с " +
					lastDisplayedRecord.ToString() +
					" до " +
					lastDeviceRecord.ToString() +
					"с прибора " +
					device.PresentationName +
					"\n");
				var newItems = JournalHelper.GetJournalItems(device, lastDeviceRecord, lastDisplayedRecord + 1);
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

		List<DeviceLastRecord> devicesLastRecord;
		public static readonly Object Locker = new object();

		public MainViewModel()
		{
			StartMonitoringCommand = new RelayCommand(OnStartMonitoring);
			StopMonitoringCommand = new RelayCommand(OnStopMonitoring);
			TestCommand = new RelayCommand(OnTest);

			DevicesViewModel = new DevicesViewModel();

			devicesLastRecord = new List<DeviceLastRecord>();
			Guid guid = new Guid();
			JournalItems = new ObservableCollection<FSJournalItem>(DBJournalHelper.GetJournalItems(guid));

			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.IsPanel)
				{
					try
					{
						devicesLastRecord.Add(new DeviceLastRecord(device));
					}
					catch
					{
						Trace.Write("Ошибка при считывании последней записи");
					}
				}
			}

			autoResetEvent = new AutoResetEvent(false);
			StartMonitoring();
		}

		public DevicesViewModel DevicesViewModel { get; private set; }

		bool stop = false;
		AutoResetEvent autoResetEvent;

		public void StartMonitoring()
		{
			foreach (var device in devicesLastRecord)
			{
				//var thread = new Thread(() =>
				device.StartMonitoring();
				//    );
				//thread.Start();
			}
		}

		private void AddToJournalObservable(FSJournalItem journalItem)
		{
			Dispatcher.Invoke(new Action(() =>
			{
				JournalItems.Add(journalItem);
			}));
		}

		private void StopMonitoring()
		{
			stop = true;
			Trace.WriteLine("Останавливаю мониторинг");
		}

		private ObservableCollection<FSJournalItem> _journalItems;

		public ObservableCollection<FSJournalItem> JournalItems
		{
			get { return _journalItems; }
			set
			{
				_journalItems = value;
				OnPropertyChanged("JournalItems");
			}
		}

		public RelayCommand StartMonitoringCommand { get; private set; }

		private void OnStartMonitoring()
		{
			if (stop)
			{
				StartMonitoring();
			}
		}

		public RelayCommand StopMonitoringCommand { get; private set; }

		private void OnStopMonitoring()
		{
			StopMonitoring();
		}

		public RelayCommand TestCommand { get; private set; }

		private void OnTest()
		{
		}
	}
}