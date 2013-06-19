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

namespace MonitorClientFS2
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
			StartMonitoringCommand = new RelayCommand(OnStartMonitoring);
			StopMonitoringCommand = new RelayCommand(OnStopMonitoring);
			ReadStatesCommand = new RelayCommand(OnReadStates);
			TestCommand = new RelayCommand(OnTest);
			TestAdminCommand = new RelayCommand(OnTestAdmin);
			SuspendMonitoringCommand = new RelayCommand(OnSuspendMonitoring);
			ResumeMonitoringCommand = new RelayCommand(OnResumeMonitoring);

			DevicesViewModel = new DevicesViewModel();
			JournalItems = new ObservableCollection<FS2JournalItem>();

			MainManager.NewJournalItem += new Action<FS2JournalItem>(ShowNewItem);

			if (StartMonitoring)
				MainManager.StartMonitoring();
		}

		public DevicesViewModel DevicesViewModel { get; private set; }

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

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			var firmwareFileName = Path.Combine(AppDataFolderHelper.GetFolder("Server"), "frm.fscf");
			var hexInfo = FirmwareUpdateOperationHelper.GetHexInfo(firmwareFileName, DevicesViewModel.SelectedDevice.Device.Driver.ShortName + ".hex");
			var orrset = hexInfo.Offset;

			//USBManager.Initialize();

			//var stateBytes = new List<byte>() { 1, 2 };
			//var bitArray = new BitArray(stateBytes.ToArray());
			//for (int i = 0; i < 16; i++)
			//{
			//    var isBitSetted = bitArray[i];
			//    Trace.WriteLine(i + "-" + isBitSetted);
			//}
			//MonitoringProcessor.WriteStats();
			//MainManager.StopMonitoring();
			//DeviceStatesManager.UpdatePanelState(ConfigurationManager.Devices.FirstOrDefault(x => x.Driver.IsPanel && x.IntAddress == 15));
			//MainManager.StartMonitoring();
		}

		public RelayCommand TestAdminCommand { get; private set; }
		void OnTestAdmin()
		{
			var fs2Contract = new FS2Contract();
			var result = fs2Contract.DeviceReadConfig(DevicesViewModel.SelectedDevice.Device.UID, false);
		}

		public RelayCommand ReadStatesCommand { get; private set; }
		void OnReadStates()
		{
		}
	}
}