using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
			public Device Device;
			public int LastRecord;
			public int LastSecRecord;
		}

		List<DeviceLastRecord> devicesLastRecord;

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
						int LastRecord;
						int LastSecRecord = -1;

						LastRecord = JournalHelper.GetLastJournalItemId(device);
						//LastRecord = DBJournalHelper.GetLastId(device);

						if (device.Driver.DriverType == DriverType.Rubezh_2OP)
							LastSecRecord = JournalHelper.GetLastSecJournalItemId2Op(device);

						devicesLastRecord.Add(new DeviceLastRecord
							{
								Device = device,
								LastRecord = LastRecord,
								LastSecRecord = LastSecRecord
							});
						Trace.WriteLine(device.PresentationAddressAndName + " " + LastRecord.ToString());
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
		Thread monitoringThread;

		public void StartMonitoring()
		{
			//Trace.WriteLine("Начинаю мониторинг");
			//while (true)
			//{
			//    foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			//    {
			//        if (device.Driver.IsPanel)
			//        {
			//            var panelDevice = device;
			//            Thread thread = new Thread(new ThreadStart(() =>
			//            {
			//                MonitoringPerDevice(panelDevice);
			//            }));
			//            thread.IsBackground = true;
			//            thread.Start();
			//            //MonitoringPerDevice(panelDevice);
			//        }
			//    }
			//}

			stop = false;
			monitoringThread = new Thread(new ThreadStart(() =>
			{
				Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
				{
					Trace.WriteLine("Начинаю мониторинг");
					while (!stop)
					{
						autoResetEvent.WaitOne(1000);
						foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
						{
							if (device.Driver.IsPanel)
							{
								var panelDevice = device;
								Thread thread = new Thread(new ThreadStart(() =>
								{
									MonitoringPerDevice(panelDevice);
								}));
								thread.IsBackground = true;
								thread.Start();
								//MonitoringPerDevice(panelDevice);
							}
						}
					}
					autoResetEvent.Set();
				}));
			}));

			monitoringThread.IsBackground = true;
			monitoringThread.Start();
		}

		private void MonitoringPerDevice(Device panelDevice)
		{
			int lastDisplayedRecord = devicesLastRecord.FirstOrDefault(x => x.Device == panelDevice).LastRecord;
			int lastDeviceRecord = JournalHelper.GetLastJournalItemId(panelDevice);
			if (lastDeviceRecord > lastDisplayedRecord)
			{
				ReadNewItems(lastDisplayedRecord, lastDeviceRecord, panelDevice);
			}

			//if (device.Driver.DriverType == DriverType.Rubezh_2OP)
			//{
			//    devicesLastSecRecord.TryGetValue(device, out lastDisplayedSecRecord);
			//    lastDeviceSecRecord = JournalHelper.GetLastSecJournalItemId2Op(device);
			//    if (lastDeviceSecRecord != lastDisplayedSecRecord)
			//    {
			//        ReadNewSecItems(lastDisplayedSecRecord, lastDeviceSecRecord, device);
			//    }
			//}
		}

		//private void ReadNewSecItems(int lastDisplayedSecRecord, int lastDeviceSecRecord, Device device)
		//{
		//    Trace.Write("Дочитываю охранные записи с " +
		//        lastDisplayedSecRecord.ToString() +
		//        " до " +
		//        lastDeviceSecRecord.ToString());
		//    var newItems = JournalHelper.GetSecJournalItems2Op(device, lastDeviceSecRecord, lastDisplayedSecRecord + 1);
		//    foreach (var journalItem in newItems)
		//    {
		//        if (journalItem != null)
		//        {
		//            AddToJournalObservable(journalItem);
		//            DBJournalHelper.AddJournalItem(journalItem);
		//            Trace.Write(".");
		//        }
		//    }
		//    Trace.WriteLine(" дочитал");
		//    devicesLastRecord.FirstOrDefault(x => x.Device == device).LastSecRecord = lastDeviceSecRecord;
		//    DBJournalHelper.SetLastSecId(new Guid(), lastDeviceSecRecord);
		//}

		private void ReadNewItems(int lastDisplayedRecord, int lastDeviceRecord, Device device)
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
					AddToJournalObservable(journalItem);
					DBJournalHelper.AddJournalItem(journalItem);
					Trace.Write(".");
				}
			}
			Trace.WriteLine(" дочитал");
			devicesLastRecord.FirstOrDefault(x => x.Device == device).LastRecord = lastDeviceRecord;
			DBJournalHelper.SetLastId(new Guid(), lastDeviceRecord);
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