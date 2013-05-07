using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using MonitorClientFS2.ViewModels;
using ServerFS2;
using ServerFS2.DataBase;

namespace MonitorClientFS2
{
	public class MainViewModel : BaseViewModel
	{
		List<DeviceLastRecord> devicesLastRecord;

		public MainViewModel()
		{
			StartMonitoringCommand = new RelayCommand(OnStartMonitoring);
			StopMonitoringCommand = new RelayCommand(OnStopMonitoring);
			TestCommand = new RelayCommand(OnTest);

			DevicesViewModel = new DevicesViewModel();

			devicesLastRecord = new List<DeviceLastRecord>();
			JournalItems = new ObservableCollection<FSJournalItem>(DBJournalHelper.GetJournalItems(new Guid()));

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
			OnStartMonitoring();
		}

		public DevicesViewModel DevicesViewModel { get; private set; }

		void AddToJournalObservable(FSJournalItem journalItem)
		{
			Dispatcher.Invoke(new Action(() =>
			{
				JournalItems.Add(journalItem);
			}));
		}

		ObservableCollection<FSJournalItem> _journalItems;
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
		void OnStartMonitoring()
		{
			foreach (var device in devicesLastRecord)
			{
				device.StartMonitoring();
			}
			Trace.WriteLine("Начинаю мониторинг");
		}

		public RelayCommand StopMonitoringCommand { get; private set; }
		void OnStopMonitoring()
		{
			foreach (var device in devicesLastRecord)
			{
				device.StopMonitoring();
			}
			Trace.WriteLine("Останавливаю мониторинг");
		}

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
		}
	}
}