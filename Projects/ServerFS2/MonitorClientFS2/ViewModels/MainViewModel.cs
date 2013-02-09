using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using ClientFS2;
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
			JournalItems = new ObservableCollection<JournalItem>();

			lastRecord = new Dictionary<Device, int>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == DriverType.Rubezh_2AM)
				{
					try
					{
						foreach (var journalItem in JournalHelper.GetJournalItems(device, JournalHelper.GetLastJournalItemId(device), JournalHelper.GetLastJournalItemId(device) - 1))
						{
							JournalItems.Add(journalItem);
							lastRecord.Add(device, JournalHelper.GetLastJournalItemId(device));
						}
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
		Thread thread;
		Dictionary<Device, int> lastRecord;

		public void StartMonitoring()
		{
			stop = false;
			thread = new Thread(new ThreadStart(() =>
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
							if (device.Driver.DriverType == DriverType.Rubezh_2AM)
							{
								lastRecord.TryGetValue(device, out lastDisplayedRecord);
								lastDeviceRecord = JournalHelper.GetLastJournalItemId(device);
								if (lastDeviceRecord != lastDisplayedRecord)
								{
									Trace.WriteLine("Дочитываю записи с " + lastDisplayedRecord.ToString() +
										" до " + JournalHelper.GetLastJournalItemId(device).ToString());
									foreach (var journalItem in JournalHelper.GetJournalItems(device, lastDeviceRecord, lastDisplayedRecord + 1))
										Dispatcher.Invoke(new Action(() =>
										{
											JournalItems.Add(journalItem);
										}));

									lastRecord.Remove(device);
									lastRecord.Add(device, lastDeviceRecord);
								}
								else
								{
									Trace.WriteLine("Новых записей нет");
								}
							}
						}
					}
				}));
			}));

			thread.IsBackground = true;
			thread.Start();
		}

		private void StopMonitoring()
		{
			stop = true;
			Trace.WriteLine("Останавливаю мониторинг");
		}

		private ObservableCollection<JournalItem> _journalItems;
		public ObservableCollection<JournalItem> JournalItems
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
			//DBJournalHelper.SetLastId(deviceUID, 100);
			//var lastID = DBJournalHelper.GetLastId(deviceUID);

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

			var topfsJournalItems = DBJournalHelper.GetJournalItems(deviceUID);
			;
		}
	}
}