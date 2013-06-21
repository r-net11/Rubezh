using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using FS2Api;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using MonitorClientFS2.ViewModels;
using ServerFS2;
using ServerFS2.Processor;
using Ionic.Zip;
using System.Text;
using System.IO;
using ServerFS2.Service;
using ServerFS2.Monitoring;
using System.Windows;
using System.Threading;

namespace MonitorClientFS2
{
	public class MainViewModel : BaseViewModel
	{
		public DevicesViewModel DevicesViewModel { get; private set; }

		public MainViewModel()
		{
			StartMonitoringCommand = new RelayCommand(OnStartMonitoring);
			StopMonitoringCommand = new RelayCommand(OnStopMonitoring);
			SuspendMonitoringCommand = new RelayCommand(OnSuspendMonitoring);
			ResumeMonitoringCommand = new RelayCommand(OnResumeMonitoring);
			ReadStatesCommand = new RelayCommand(OnReadStates);
			SetNewConfigCommand = new RelayCommand(OnSetNewConfig);
			GetSerialListCommand = new RelayCommand(OnGetSerialList);

			DevicesViewModel = new DevicesViewModel();
			JournalItems = new ObservableCollection<FS2JournalItem>();

			MainManager.NewJournalItem += new Action<FS2JournalItem>(ShowNewItem);
			ProgressInfos = new ObservableRangeCollection<FS2ProgressInfo>();
			CallbackManager.ProgressEvent += new System.Action<FS2Api.FS2ProgressInfo>(CallbackManager_ProgressEvent);

			if (StartMonitoring)
				MainManager.StartMonitoring();
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

		public bool StartMonitoring
		{
			get { return RegistrySettingsHelper.GetBool("MonitorClientFS2.StartMonitoring"); }
			set
			{
				RegistrySettingsHelper.SetBool("MonitorClientFS2.StartMonitoring", value);
				OnPropertyChanged("StartMonitoring");
			}
		}

		void ShowNewItem(FS2JournalItem journalItem)
		{
			Dispatcher.Invoke(new Action(() =>
			{
				JournalItems.Insert(0, journalItem);
			}));
		}

		void ShowNewItems(List<FS2JournalItem> journalItems)
		{
			journalItems.ForEach(x => ShowNewItem(x));
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
			MonitoringProcessor.SuspendMonitoring();
		}

		public RelayCommand ResumeMonitoringCommand { get; private set; }
		void OnResumeMonitoring()
		{
			MonitoringProcessor.ResumeMonitoring();
		}

		public RelayCommand SetNewConfigCommand { get; private set; }
		void OnSetNewConfig()
		{
			//MainManager.SetNewConfig(ConfigurationManager.DeviceConfiguration);

			var fs2Contract = new FS2Contract();
			fs2Contract.SetNewConfig(ConfigurationManager.DeviceConfiguration);
		}

		public RelayCommand GetSerialListCommand { get; private set; }
		void OnGetSerialList()
		{
			var fs2Contract = new FS2Contract();
			var result = fs2Contract.DeviceGetSerialList(DevicesViewModel.SelectedDevice.Device.UID);
			MessageBox.Show("DeviceGetSerialList Count " + result.Result.Count);
		}

		public RelayCommand ReadStatesCommand { get; private set; }
		void OnReadStates()
		{
		}
	}
}