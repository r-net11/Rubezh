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
using System.Linq;
using System.IO;
using ServerFS2.Service;
using ServerFS2.Monitoring;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using FiresecAPI.Models;
using System.Diagnostics;

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
			SetNewConfigurationCommand = new RelayCommand(OnSetNewConfiguration);
			GetSerialListCommand = new RelayCommand(OnGetSerialList);
			SetInitializingTestCommand = new RelayCommand(OnSetInitializingTest);
			ReadConfigurationCommand = new RelayCommand(OnReadConfiguration);

			DevicesViewModel = new DevicesViewModel();
			JournalItems = new ObservableCollection<FS2JournalItem>();

			CallbackManager.NewJournalItem += new Action<FS2JournalItem>(ShowNewItem);
			ProgressInfos = new ObservableRangeCollection<FS2ProgressInfo>();
			CallbackManager.ProgressEvent += new System.Action<FS2ProgressInfo>(CallbackManager_ProgressEvent);

			if (StartMonitoring)
				ServerFS2.Bootstrapper.Run();
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
			var result = fs2Contract.DeviceGetSerialList(DevicesViewModel.SelectedDevice.Device.UID);
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
			var result = fs2Contract.DeviceReadConfiguration(DevicesViewModel.SelectedDevice.Device.UID, false);
		}

		public RelayCommand ReadStatesCommand { get; private set; }
		void OnReadStates()
		{
			//var result = new HashSet<string>();
			//foreach (var driver in ConfigurationManager.Drivers)
			//{
			//    foreach (var state in driver.States)
			//    {
			//        if (state.AffectChildren)
			//        {
			//            if (result.Add(state.Name))
			//                Trace.WriteLine(state.Name);
			//        }
			//    }
			//}
		}
	}
}