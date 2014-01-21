using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FS2Api;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using MonitorClientFS2.ViewModels;
using MonitorTestClientFS2.ViewModels;
using ServerFS2;
using ServerFS2.Monitoring;
using ServerFS2.Processor;
using ServerFS2.Service;
using ServerFS2.USB;
using Infrastructure.Common.Windows;
using ServerFS2.ViewModels;

namespace MonitorClientFS2
{
	public class MainViewModel : BaseViewModel
	{
		public DevicesViewModel DevicesViewModel { get; private set; }
		public ZonesViewModel ZonesViewModel { get; private set; }

		public MainViewModel()
		{
			StartMonitoringCommand = new RelayCommand(OnStartMonitoring);
			StopMonitoringCommand = new RelayCommand(OnStopMonitoring);
			SuspendMonitoringCommand = new RelayCommand(OnSuspendMonitoring);
			ResumeMonitoringCommand = new RelayCommand(OnResumeMonitoring);
			TestComPortCommand = new RelayCommand(OnTestComPort);
			SetNewConfigurationCommand = new RelayCommand(OnSetNewConfiguration);
			GetSerialListCommand = new RelayCommand(OnGetSerialList);
			SetInitializingTestCommand = new RelayCommand(OnSetInitializingTest);
			ReadConfigurationCommand = new RelayCommand(OnReadConfiguration);

			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			JournalItems = new ObservableCollection<FS2JournalItem>();

			CallbackManager.NewJournalItem += new Action<FS2JournalItem>(ShowNewItem);
			ProgressInfos = new ObservableRangeCollection<FS2ProgressInfo>();
			Logs = new ObservableCollection<string>();
			CallbackManager.ProgressEvent += new System.Action<FS2ProgressInfo>(CallbackManager_ProgressEvent);
			CallbackManager.LogEvent += new System.Action<string>(CallbackManager_LogEvent);
		}

		#region Progress
		public ObservableRangeCollection<FS2ProgressInfo> ProgressInfos { get; private set; }

		void CallbackManager_ProgressEvent(FS2ProgressInfo fs2ProgressInfos)
		{
			Application.Current.Dispatcher.Invoke(new Action(
				() =>
				{
					ProgressInfos.Insert(0, fs2ProgressInfos);
					if (ProgressInfos.Count > 1000)
						ProgressInfos.RemoveAt(1000);
				}
				));
			Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
		}
		#endregion

		void ShowNewItem(FS2JournalItem journalItem)
		{
			Dispatcher.Invoke(new Action(() =>
			{
				JournalItems.Insert(0, journalItem);
				JournalItemsCount++;
			}));
		}

		void ShowNewItems(List<FS2JournalItem> journalItems)
		{
			journalItems.ForEach(x => ShowNewItem(x));
		}

		int _journalItemsCount = 0;
		public int JournalItemsCount
		{
			get { return _journalItemsCount; }
			set
			{
				_journalItemsCount = value;
				OnPropertyChanged("JournalItemsCount");
			}
		}

		ObservableCollection<FS2JournalItem> _journalItems;
		public ObservableCollection<FS2JournalItem> JournalItems
		{
			get { return _journalItems; }
			set
			{
				_journalItems = value;
				OnPropertyChanged("JournalItems");
			}
		}

		FS2JournalItem _selectedJournalItem;
		public FS2JournalItem SelectedJournalItem
		{
			get { return _selectedJournalItem; }
			set
			{
				_selectedJournalItem = value;
				OnPropertyChanged("SelectedJournalItem");
			}
		}

		public RelayCommand StartMonitoringCommand { get; private set; }
		void OnStartMonitoring()
		{
			MainManager.StartMonitoring();
		}

		public RelayCommand StopMonitoringCommand { get; private set; }
		void OnStopMonitoring()
		{
			MainManager.StopMonitoring();
		}

		public RelayCommand SuspendMonitoringCommand { get; private set; }
		void OnSuspendMonitoring()
		{
			MonitoringManager.SuspendMonitoring();
		}

		public RelayCommand ResumeMonitoringCommand { get; private set; }
		void OnResumeMonitoring()
		{
			MonitoringManager.ResumeMonitoring();
		}

		public RelayCommand SetNewConfigurationCommand { get; private set; }
		void OnSetNewConfiguration()
		{
			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					var fs2Contract = new FS2Contract();
					fs2Contract.SetNewConfiguration(ConfigurationManager.DeviceConfiguration, null);
					Thread.Sleep(TimeSpan.FromSeconds(5));
				}
			});
		}

		public RelayCommand GetSerialListCommand { get; private set; }
		void OnGetSerialList()
		{
			var fs2Contract = new FS2Contract();
			var result = fs2Contract.DeviceGetSerialList(DevicesViewModel.SelectedDevice.Device.UID, "Тестовый пользователь");
			MessageBox.Show("DeviceGetSerialList Count " + result.Result.Count);
		}

		public RelayCommand SetInitializingTestCommand { get; private set; }
		void OnSetInitializingTest()
		{
			MonitoringManager.MonitoringUSBs[0].SetAllInitializing();
		}

		public RelayCommand ReadConfigurationCommand { get; private set; }
		void OnReadConfiguration()
		{
			var fs2Contract = new FS2Contract();
			var result = fs2Contract.DeviceReadConfiguration(DevicesViewModel.SelectedDevice.Device.UID, false, "Тестовый пользователь");
		}

		public RelayCommand TestComPortCommand { get; private set; }
		void OnTestComPort()
		{
			ComPortHelper.Run();
		}

		void CallbackManager_LogEvent(string value)
		{
			Dispatcher.Invoke(new Action(() =>
			{
				Logs.Insert(0, value);
			}));
		}

		public ObservableCollection<string> Logs { get; private set; }
	}
}