using System;
using System.Collections.Generic;
using System.Linq;
using GKProcessor;
using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class ArchiveFilterViewModel : DialogViewModel
	{
		public ArchiveFilterViewModel(XArchiveFilter archiveFilter)
		{
			Title = "Настройки фильтра";
			ClearCommand = new RelayCommand(OnClear);
			SaveCommand = new RelayCommand(OnSave);
			CancelCommand = new RelayCommand(OnCancel);
			Initialize(archiveFilter);
        }

		void Initialize(XArchiveFilter archiveFilter)
		{
			StartDateTime = new DateTimePairViewModel(archiveFilter.StartDate);
			EndDateTime = new DateTimePairViewModel(archiveFilter.EndDate);

			InitializeJournalItemTypes(archiveFilter);
			InitializeStateClasses(archiveFilter);
			InitializeJournalDescriptionStates(archiveFilter);
			InitializeDevices(archiveFilter);
			InitializeZones(archiveFilter);
			InitializeDirections(archiveFilter);
			InitializeDescriptions(archiveFilter);
			InitializeSubsystemTypes(archiveFilter);
			InitializePumpStations(archiveFilter);
			InitializePIMs(archiveFilter);
		}

		void InitializeJournalItemTypes(XArchiveFilter archiveFilter)
		{
			JournalItemTypes = new List<JournalItemTypeViewModel>();
			JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.Device));
			JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.Direction));
			JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.GK));
			JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.System));
			JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.Zone));
			JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.PumpStation));
			JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.Delay));
			JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.Pim));

			foreach (var journalItemType in archiveFilter.JournalItemTypes)
			{
				var JournalItemTypeViewModel = JournalItemTypes.FirstOrDefault(x => x.JournalItemType == journalItemType);
				if (JournalItemTypeViewModel != null)
				{
					JournalItemTypeViewModel.IsChecked = true;
				}
			}
		}

		void InitializeStateClasses(XArchiveFilter archiveFilter)
		{
			StateClasses = new List<StateClassViewModel>();
			foreach (XStateClass stateClass in Enum.GetValues(typeof(XStateClass)))
			{
				var stateClassViewModel = new StateClassViewModel(stateClass);
				StateClasses.Add(stateClassViewModel);
			}

			foreach (var stateClass in archiveFilter.StateClasses)
			{
				var stateClassViewModel = StateClasses.FirstOrDefault(x => x.StateClass == stateClass);
				if (stateClassViewModel != null)
				{
					stateClassViewModel.IsChecked = true;
				}
			}
		}

		public void InitializeJournalDescriptionStates(XArchiveFilter archiveFilter)
		{
			JournalDescriptionStates = new List<JournalDescriptionStateViewModel>();
            foreach (var journalDescriptionState in JournalDescriptionStateHelper.JournalDescriptionStates)
            {
                JournalDescriptionStates.Add(new JournalDescriptionStateViewModel(journalDescriptionState, DistinctDatabaseNames));
            }
            foreach (var journalDescriptionState in archiveFilter.JournalDescriptionState)
            {
                var eventNameViewModel = JournalDescriptionStates.FirstOrDefault(x => x.JournalDescriptionState == journalDescriptionState);
                if (eventNameViewModel != null)
                {
                    eventNameViewModel.IsChecked = true;
                }
            }
            JournalDescriptionStates.Sort(JournalDescriptionStateViewModel.Compare);
		}

		public void InitializeZones(XArchiveFilter archiveFilter)
		{
			ArchiveZones = new List<ArchiveZoneViewModel>();
			foreach (var zone in XManager.Zones)
			{
				var archiveZoneViewModel = new ArchiveZoneViewModel(zone);
				ArchiveZones.Add(archiveZoneViewModel);
			}
			foreach (var zoneUID in archiveFilter.ZoneUIDs)
			{
				var archiveZone = ArchiveZones.FirstOrDefault(x => x.Zone.UID == zoneUID);
				if (archiveZone != null)
				{
					archiveZone.IsChecked = true;
				}
			}
		}

		public void InitializeDirections(XArchiveFilter archiveFilter)
		{
			ArchiveDirections = new List<ArchiveDirectionViewModel>();
			foreach (var direction in XManager.Directions)
			{
				var archiveDirectionViewModel = new ArchiveDirectionViewModel(direction);
				ArchiveDirections.Add(archiveDirectionViewModel);
			}
			foreach (var directionUID in archiveFilter.DirectionUIDs)
			{
				var archiveDirection = ArchiveDirections.FirstOrDefault(x => x.Direction.UID == directionUID);
				if (archiveDirection != null)
				{
					archiveDirection.IsChecked = true;
				}
			}
		}

		public void InitializeDescriptions(XArchiveFilter archiveFilter)
		{
			ArchiveDescriptions = new List<DescriptionViewModel>();
			foreach (var description in DescriptionsHelper.GetAllDescriptions())
			{
				ArchiveDescriptions.Add(new DescriptionViewModel(description, DistinctDatabaseDescriptions));
            }
			foreach (var description in archiveFilter.Descriptions)
			{
				var descriptionViewModel = ArchiveDescriptions.FirstOrDefault(x => x.Description.Name == description);
                if(descriptionViewModel != null)
				{
					descriptionViewModel.IsChecked = true;
				}
			}
            ArchiveDescriptions.Sort(DescriptionViewModel.Compare);
        }

		void InitializeSubsystemTypes(XArchiveFilter archiveFilter)
		{
			SubsystemTypes = new List<SubsystemTypeViewModel>();
			foreach (XSubsystemType item in Enum.GetValues(typeof(XSubsystemType)))
			{
				SubsystemTypes.Add(new SubsystemTypeViewModel(item));
			}
			foreach (var subsystemType in archiveFilter.SubsystemTypes)
			{
				var subsystemTypeViewModel = SubsystemTypes.FirstOrDefault(x => x.SubsystemType == subsystemType);
				if (subsystemTypeViewModel != null)
				{
					subsystemTypeViewModel.IsChecked = true;
				}
			}
		}

		void InitializePumpStations(XArchiveFilter archiveFilter)
		{
			PumpStations = new List<ArchivePumpStationViewModel>();
			foreach (var direction in XManager.PumpStations)
			{
				var archiveDirectionViewModel = new ArchivePumpStationViewModel(direction);
				PumpStations.Add(archiveDirectionViewModel);
			}
			foreach (var uid in archiveFilter.PumpStationUIDs)
			{
				var pumpStation = PumpStations.FirstOrDefault(x => x.PumpStation.UID == uid);
				if (pumpStation != null)
				{
					pumpStation.IsChecked = true;
				}
			}
		}

		void InitializePIMs(XArchiveFilter archiveFilter)
		{
			PIMs = new List<ArchivePimViewModel>();
			foreach (var pim in XManager.Pims)
			{
				var archivePimViewModel = new ArchivePimViewModel(pim);
				PIMs.Add(archivePimViewModel);
			}
			foreach (var uid in archiveFilter.PimUIDs)
			{
				var pim = PIMs.FirstOrDefault(x => x.Pim.UID == uid);
				if (pim != null)
				{
					pim.IsChecked = true;
				}
			}
		}

		#region Devices
		public void InitializeDevices(XArchiveFilter archiveFilter)
		{
			BuildTree();
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
				SelectedDevice = RootDevice;
			}

			foreach (var deviceUID in archiveFilter.DeviceUIDs)
			{
				var archiveDevice = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
				if (archiveDevice != null)
				{
					archiveDevice.IsChecked = true;
					archiveDevice.ExpandToThis();
				}
			}

			OnPropertyChanged("RootDevices");
		}

		#region DeviceSelection
		public List<ArchiveDeviceViewModel> AllDevices;

		public void FillAllDevices()
		{
			AllDevices = new List<ArchiveDeviceViewModel>();
			AddChildPlainDevices(RootDevice);
		}

		void AddChildPlainDevices(ArchiveDeviceViewModel parentViewModel)
		{
			AllDevices.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainDevices(childViewModel);
		}

		public void Select(Guid deviceUID)
		{
			if (deviceUID != Guid.Empty)
			{
				var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
				if (deviceViewModel != null)
					deviceViewModel.ExpandToThis();
				SelectedDevice = deviceViewModel;
			}
		}
		#endregion

		ArchiveDeviceViewModel _selectedDevice;
		public ArchiveDeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedDevice");
			}
		}

		ArchiveDeviceViewModel _rootDevice;
		public ArchiveDeviceViewModel RootDevice
		{
			get { return _rootDevice; }
			private set
			{
				_rootDevice = value;
				OnPropertyChanged("RootDevice");
			}
		}

		public ArchiveDeviceViewModel[] RootDevices
		{
			get { return new ArchiveDeviceViewModel[] { RootDevice }; }
		}

		void BuildTree()
		{
			RootDevice = AddDeviceInternal(XManager.DeviceConfiguration.RootDevice, null);
			FillAllDevices();
		}

		private ArchiveDeviceViewModel AddDeviceInternal(XDevice device, ArchiveDeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new ArchiveDeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}
		#endregion

		public DateTime ArchiveFirstDate
		{
			get { return ArchiveViewModel.ArchiveFirstDate; }
		}

		public DateTime NowDate
		{
			get { return DateTime.Now; }
		}

		DateTimePairViewModel _startDateTime;
		public DateTimePairViewModel StartDateTime
		{
			get { return _startDateTime; }
			set
			{
				_startDateTime = value;
				OnPropertyChanged("StartDateTime");
			}
		}

		DateTimePairViewModel _endDateTime;
		public DateTimePairViewModel EndDateTime
		{
			get { return _endDateTime; }
			set
			{
				_endDateTime = value;
				OnPropertyChanged("EndDateTime");
			}
		}

		bool useDeviceDateTime;
		public bool UseDeviceDateTime
		{
			get { return useDeviceDateTime; }
			set
			{
				useDeviceDateTime = value;
				OnPropertyChanged("UseDeviceDateTime");
			}
		}

		public List<JournalItemTypeViewModel> JournalItemTypes { get; private set; }
		public List<StateClassViewModel> StateClasses { get; private set; }
		public List<JournalDescriptionStateViewModel> JournalDescriptionStates { get; private set; }
		public List<ArchiveZoneViewModel> ArchiveZones { get; private set; }
		public List<ArchiveDirectionViewModel> ArchiveDirections { get; private set; }
		public List<DescriptionViewModel> ArchiveDescriptions { get; private set; }
		public List<SubsystemTypeViewModel> SubsystemTypes { get; private set; }
		public List<ArchivePumpStationViewModel> PumpStations { get; private set; }
		public List<ArchivePimViewModel> PIMs { get; private set; }
        List<string> DistinctDatabaseNames = GKDBHelper.SelectDistinctNames();
        List<string> DistinctDatabaseDescriptions = GKDBHelper.SelectDistinctDescriptions();

		public XArchiveFilter GetModel()
		{
			var archiveFilter = new XArchiveFilter()
			{
				StartDate = StartDateTime.DateTime,
				EndDate = EndDateTime.DateTime,
				UseDeviceDateTime = UseDeviceDateTime
			};
			foreach (var journalItemType in JournalItemTypes)
			{
				if (journalItemType.IsChecked)
					archiveFilter.JournalItemTypes.Add(journalItemType.JournalItemType);
			}
			foreach (var stateClass in StateClasses)
			{
				if (stateClass.IsChecked)
					archiveFilter.StateClasses.Add(stateClass.StateClass);
			}
			foreach (var eventName in JournalDescriptionStates)
			{
				if (eventName.IsChecked)
                    archiveFilter.JournalDescriptionState.Add(eventName.JournalDescriptionState);
			}
			foreach (var archiveDevice in AllDevices)
			{
				if (archiveDevice.IsChecked)
					archiveFilter.DeviceUIDs.Add(archiveDevice.Device.UID);
			}
			foreach (var archiveZone in ArchiveZones)
			{
				if (archiveZone.IsChecked)
					archiveFilter.ZoneUIDs.Add(archiveZone.Zone.UID);
			}
			foreach (var archiveDirection in ArchiveDirections)
			{
				if (archiveDirection.IsChecked)
					archiveFilter.DirectionUIDs.Add(archiveDirection.Direction.UID);
			}
			foreach (var description in ArchiveDescriptions)
			{
				if (description.IsChecked)
					archiveFilter.Descriptions.Add(description.Description.Name);
			}
			foreach (var subsystemType in SubsystemTypes)
			{
				if (subsystemType.IsChecked)
					archiveFilter.SubsystemTypes.Add(subsystemType.SubsystemType);
			}
			foreach (var pumpStation in PumpStations)
			{
				if (pumpStation.IsChecked)
					archiveFilter.PumpStationUIDs.Add(pumpStation.PumpStation.UID);
			}
			foreach (var pim in PIMs)
			{
				if (pim.IsChecked)
					archiveFilter.PimUIDs.Add(pim.Pim.UID);
			}
			return archiveFilter;
		}

		public RelayCommand ClearCommand { get; private set; }
		void OnClear()
		{
			Initialize(new XArchiveFilter());
			OnPropertyChanged("JournalItemTypes");
			OnPropertyChanged("StateClasses");
			OnPropertyChanged("GKAddresses");
			OnPropertyChanged("EventNames");
			OnPropertyChanged("RootDevices");
			OnPropertyChanged("ArchiveZones");
			OnPropertyChanged("ArchiveDirections");
			OnPropertyChanged("ArchiveDescriptions");
			OnPropertyChanged("JournalDescriptionStates");
			OnPropertyChanged("SubsystemTypes");
			OnPropertyChanged("PumpStations");
			OnPropertyChanged("PIMs");
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			if (StartDateTime.DateTime > EndDateTime.DateTime)
			{
				MessageBoxService.ShowWarning("Начальная дата должна быть меньше конечной");
				return;
			}
			Close(true);
		}
		public RelayCommand CancelCommand { get; private set; }
		void OnCancel()
		{
			Close(false);
		}
    }
}