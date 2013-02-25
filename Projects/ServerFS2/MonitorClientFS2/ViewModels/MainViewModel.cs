using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using FiresecAPI;
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
		public MainViewModel()
		{
			StartMonitoringCommand = new RelayCommand(OnStartMonitoring);
			StopMonitoringCommand = new RelayCommand(OnStopMonitoring);
			TestCommand = new RelayCommand(OnTest);

			DevicesViewModel = new DevicesViewModel();
			JournalItems = new ObservableCollection<FSJournalItem>();

			devicesLastRecord = new Dictionary<Device, int>();
			Guid guid = new Guid();
			JournalItems = new ObservableCollection<FSJournalItem>(DBJournalHelper.GetJournalItems(guid));

			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.IsPanel)
				{
					try
					{
						devicesLastRecord.Add(device, DBJournalHelper.GetLastId(guid));
					}
					catch { }
				}
			}

			autoResetEvent = new AutoResetEvent(false);
			StartMonitoring();
		}

		public DevicesViewModel DevicesViewModel { get; private set; }

		bool stop = false;
		AutoResetEvent autoResetEvent;
		Thread monitoringThread;
		Dictionary<Device, int> devicesLastRecord;

		public void StartMonitoring()
		{
			stop = false;
			monitoringThread = new Thread(new ThreadStart(() =>
			{
				Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
				{
					Trace.WriteLine("Начинаю мониторинг");
					int lastDisplayedRecord;
					int lastDeviceRecord;
					while (!stop)
					{
						autoResetEvent.WaitOne(1000);
						foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
						{
							if (device.Driver.IsPanel)
							{
								devicesLastRecord.TryGetValue(device, out lastDisplayedRecord);
								lastDeviceRecord = JournalHelper.GetLastJournalItemId(device);
								if (lastDeviceRecord != lastDisplayedRecord)
								{
									Trace.Write("Дочитываю записи с " +
										lastDisplayedRecord.ToString() +
										" до " +
										lastDeviceRecord.ToString());
									var newItems = JournalHelper.GetJournalItems(device, lastDeviceRecord, lastDisplayedRecord + 1);
									foreach (var journalItem in newItems)
									{
										Dispatcher.Invoke(new Action(() =>
										{
											JournalItems.Add(journalItem);
										}));
										DBJournalHelper.AddJournalItem(journalItem);
										Trace.Write(".");
									}
									Trace.WriteLine(" дочитал");
									devicesLastRecord.Remove(device);
									devicesLastRecord.Add(device, lastDeviceRecord);
									DBJournalHelper.SetLastId(new Guid(), lastDeviceRecord);
								}
							}
						}
					}
					autoResetEvent.Set();
				}));
			}));

			monitoringThread.IsBackground = true;
			monitoringThread.Start();
		}

		private void StopMonitoring()
		{
			Trace.WriteLine(monitoringThread.ThreadState.ToString());
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
			var deviceUID = new Guid("444C11C8-D5E7-4309-9209-56F6720262B9");
			var fsJournalItems = new List<FSJournalItem>();
			for (int i = 0; i < 100; i++)
			{
				var fsJournalItem = new FSJournalItem()
				{
					DeviceTime = DateTime.Now,
					SystemTime = DateTime.Now,
					ZoneName = "Зона 1",
					Description = "Описание",
					DeviceName = "Название устройства",
					PanelName = "Название прибора",
					DeviceUID = Guid.NewGuid(),
					PanelUID = Guid.NewGuid(),
					UserName = "Пользователь",
					SubsystemType = SubsystemType.Fire,
					StateType = StateType.Fire,
					Detalization = "Детализация",
					DeviceCategory = 0
				};
				fsJournalItems.Add(fsJournalItem);
			}
			DBJournalHelper.AddJournalItems(fsJournalItems);
			foreach (var item in fsJournalItems)
			{
				JournalItems.Add(item);
			}
		}
	}
}