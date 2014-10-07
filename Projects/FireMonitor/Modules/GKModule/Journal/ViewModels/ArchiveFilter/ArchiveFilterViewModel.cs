using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecClient;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.CheckBoxList;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ArchiveFilterViewModel : DialogViewModel
	{
		public ArchiveFilterViewModel(GKArchiveFilter archiveFilter)
		{
			Title = "Настройки фильтра";
			ClearCommand = new RelayCommand(OnClear);
			SaveCommand = new RelayCommand(OnSave);
			CancelCommand = new RelayCommand(OnCancel);
			Initialize(archiveFilter);
		}

		void Initialize(GKArchiveFilter archiveFilter)
		{
			StartDateTime = new DateTimePairViewModel(archiveFilter.StartDate);
			EndDateTime = new DateTimePairViewModel(archiveFilter.EndDate);

			InitializeJournalItemTypes(archiveFilter);
			InitializeStateClasses(archiveFilter);
			InitializeEventNames(archiveFilter);
			InitializeDescriptions(archiveFilter);
			InitializeDevices(archiveFilter);
			InitializeZones(archiveFilter);
			InitializeDirections(archiveFilter);
			InitializeSubsystemTypes(archiveFilter);
			InitializePumpStations(archiveFilter);
			InitializeMPTs(archiveFilter);
			InitializeDelays(archiveFilter);
			InitializePIMs(archiveFilter);
			InitializeGuardZones(archiveFilter);
		}

		void InitializeJournalItemTypes(GKArchiveFilter archiveFilter)
		{
			JournalItemTypes = new CheckBoxItemList<JournalItemTypeViewModel>();
			JournalItemTypes.Add(new JournalItemTypeViewModel(GKJournalObjectType.Device));
			JournalItemTypes.Add(new JournalItemTypeViewModel(GKJournalObjectType.Direction));
			JournalItemTypes.Add(new JournalItemTypeViewModel(GKJournalObjectType.GK));
			JournalItemTypes.Add(new JournalItemTypeViewModel(GKJournalObjectType.System));
			JournalItemTypes.Add(new JournalItemTypeViewModel(GKJournalObjectType.Zone));
			JournalItemTypes.Add(new JournalItemTypeViewModel(GKJournalObjectType.PumpStation));
			JournalItemTypes.Add(new JournalItemTypeViewModel(GKJournalObjectType.MPT));
			JournalItemTypes.Add(new JournalItemTypeViewModel(GKJournalObjectType.Delay));
			JournalItemTypes.Add(new JournalItemTypeViewModel(GKJournalObjectType.Pim));
			JournalItemTypes.Add(new JournalItemTypeViewModel(GKJournalObjectType.GuardZone));
			foreach (var journalItemType in archiveFilter.XJournalObjectTypes)
			{
				var JournalItemTypeViewModel = JournalItemTypes.Items.FirstOrDefault(x => (x as JournalItemTypeViewModel).JournalItemType == journalItemType);
				if (JournalItemTypeViewModel != null)
				{
					JournalItemTypeViewModel.IsChecked = true;
				}
			}
		}

		void InitializeStateClasses(GKArchiveFilter archiveFilter)
		{
			StateClasses = new CheckBoxItemList<StateClassViewModel>();
			foreach (XStateClass stateClass in Enum.GetValues(typeof(XStateClass)))
			{
				var stateClassViewModel = new StateClassViewModel(stateClass);
				StateClasses.Add(stateClassViewModel);
			}
			foreach (var stateClass in archiveFilter.StateClasses)
			{
				var stateClassViewModel = StateClasses.Items.FirstOrDefault(x => (x as StateClassViewModel).StateClass == stateClass);
				if (stateClassViewModel != null)
				{
					stateClassViewModel.IsChecked = true;
				}
			}
		}

		public void InitializeEventNames(GKArchiveFilter archiveFilter)
		{
			EventNames = new CheckBoxItemList<EventNameViewModel>();
			foreach (JournalEventNameType enumValue in Enum.GetValues(typeof(JournalEventNameType)))
			{
				var eventNameViewModel = new EventNameViewModel(enumValue, DistinctDatabaseNames);
				if (eventNameViewModel.JournalSubsystemType == JournalSubsystemType.GK)
				{
					EventNames.Add(eventNameViewModel);
				}
			}

			EventNames.Items.Sort(EventNameViewModel.Compare);
			foreach (var eventName in archiveFilter.EventNames)
			{
				var eventNameViewModel = EventNames.Items.FirstOrDefault(x => (x as EventNameViewModel).Name == eventName);
				if (eventNameViewModel != null)
				{
					eventNameViewModel.IsChecked = true;
				}
			}
		}

		public void InitializeDescriptions(GKArchiveFilter archiveFilter)
		{
			ArchiveDescriptions = new CheckBoxItemList<ArchiveDescriptionViewModel>();
			//foreach (var description in DescriptionsHelper.GetAllDescriptions())
			//{
			//	ArchiveDescriptions.Add(new ArchiveDescriptionViewModel(description, DistinctDatabaseDescriptions));
			//}
			ArchiveDescriptions.Items.Sort(ArchiveDescriptionViewModel.Compare);
			foreach (var description in archiveFilter.Descriptions)
			{
				var descriptionViewModel = ArchiveDescriptions.Items.FirstOrDefault(x => (x as ArchiveDescriptionViewModel).Description == description);
				if (descriptionViewModel != null)
				{
					descriptionViewModel.IsChecked = true;
				}
			}
		}

		public void InitializeZones(GKArchiveFilter archiveFilter)
		{
			ArchiveZones = new CheckBoxItemList<ArchiveZoneViewModel>();
			foreach (var zone in GKManager.Zones)
			{
				var archiveZoneViewModel = new ArchiveZoneViewModel(zone);
				ArchiveZones.Add(archiveZoneViewModel);
			}
			foreach (var zoneUID in archiveFilter.ZoneUIDs)
			{
				var archiveZone = ArchiveZones.Items.FirstOrDefault(x => (x as ArchiveZoneViewModel).Zone.UID == zoneUID);
				if (archiveZone != null)
				{
					archiveZone.IsChecked = true;
				}
			}
		}

		public void InitializeDirections(GKArchiveFilter archiveFilter)
		{
			ArchiveDirections = new CheckBoxItemList<ArchiveDirectionViewModel>();
			foreach (var direction in GKManager.Directions)
			{
				var archiveDirectionViewModel = new ArchiveDirectionViewModel(direction);
				ArchiveDirections.Add(archiveDirectionViewModel);
			}
			foreach (var directionUID in archiveFilter.DirectionUIDs)
			{
				var archiveDirection = ArchiveDirections.Items.FirstOrDefault(x => (x as ArchiveDirectionViewModel).Direction.UID == directionUID);
				if (archiveDirection != null)
				{
					archiveDirection.IsChecked = true;
				}
			}
		}

		void InitializeSubsystemTypes(GKArchiveFilter archiveFilter)
		{
			SubsystemTypes = new CheckBoxItemList<SubsystemTypeViewModel>();
			foreach (GKSubsystemType item in Enum.GetValues(typeof(GKSubsystemType)))
			{
				SubsystemTypes.Add(new SubsystemTypeViewModel(item));
			}
			foreach (var subsystemType in archiveFilter.SubsystemTypes)
			{
				var subsystemTypeViewModel = SubsystemTypes.Items.FirstOrDefault(x => x.SubsystemType == subsystemType);
				if (subsystemTypeViewModel != null)
				{
					subsystemTypeViewModel.IsChecked = true;
				}
			}
		}

		void InitializePumpStations(GKArchiveFilter archiveFilter)
		{
			PumpStations = new CheckBoxItemList<ArchivePumpStationViewModel>();
			foreach (var pumpStation in GKManager.PumpStations)
			{
				var archiveDirectionViewModel = new ArchivePumpStationViewModel(pumpStation);
				PumpStations.Add(archiveDirectionViewModel);
			}
			foreach (var uid in archiveFilter.PumpStationUIDs)
			{
				var pumpStation = PumpStations.Items.FirstOrDefault(x => x.PumpStation.UID == uid);
				if (pumpStation != null)
				{
					pumpStation.IsChecked = true;
				}
			}
		}

		void InitializeMPTs(GKArchiveFilter archiveFilter)
		{
			MPTs = new CheckBoxItemList<ArchiveMPTViewModel>();
			foreach (var mpt in GKManager.MPTs)
			{
				var archiveMPTViewModel = new ArchiveMPTViewModel(mpt);
				MPTs.Add(archiveMPTViewModel);
			}
			foreach (var uid in archiveFilter.MPTUIDs)
			{
				var mpt = MPTs.Items.FirstOrDefault(x => x.MPT.UID == uid);
				if (mpt != null)
				{
					mpt.IsChecked = true;
				}
			}
		}

		void InitializeDelays(GKArchiveFilter archiveFilter)
		{
			Delays = new CheckBoxItemList<ArchiveDelayViewModel>();
			foreach (var delay in GKManager.Delays)
			{
				var archiveDelayViewModel = new ArchiveDelayViewModel(delay);
				Delays.Add(archiveDelayViewModel);
			}
			foreach (var uid in archiveFilter.DelayUIDs)
			{
				var delay = Delays.Items.FirstOrDefault(x => x.Delay.UID == uid);
				if (delay != null)
				{
					delay.IsChecked = true;
				}
			}
		}

		void InitializePIMs(GKArchiveFilter archiveFilter)
		{
			PIMs = new CheckBoxItemList<ArchivePimViewModel>();
			foreach (var pim in GKManager.AutoGeneratedPims)
			{
				var archivePimViewModel = new ArchivePimViewModel(pim);
				PIMs.Add(archivePimViewModel);
			}
			foreach (var uid in archiveFilter.PimUIDs)
			{
				var pim = PIMs.Items.FirstOrDefault(x => x.Pim.UID == uid);
				if (pim != null)
				{
					pim.IsChecked = true;
				}
			}
		}

		public void InitializeGuardZones(GKArchiveFilter archiveFilter)
		{
			ArchiveGuardZones = new CheckBoxItemList<ArchiveGuardZoneViewModel>();
			foreach (var guardZone in GKManager.GuardZones)
			{
				var archiveGuardZoneViewModel = new ArchiveGuardZoneViewModel(guardZone);
				ArchiveGuardZones.Add(archiveGuardZoneViewModel);
			}
			foreach (var guardZoneUID in archiveFilter.GuardZoneUIDs)
			{
				var archiveGuardZone = ArchiveGuardZones.Items.FirstOrDefault(x => (x as ArchiveGuardZoneViewModel).GuardZone.UID == guardZoneUID);
				if (archiveGuardZone != null)
				{
					archiveGuardZone.IsChecked = true;
				}
			}
		}

		#region Devices
		public void InitializeDevices(GKArchiveFilter archiveFilter)
		{
			BuildTree();
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
				SelectedDevice = RootDevice;
			}

			foreach (var deviceUID in archiveFilter.DeviceUIDs)
			{
				foreach (ArchiveDeviceViewModel archiveDevice in AllDevices.Items)
				{
					if (archiveDevice.Device.UID == deviceUID)
					{
						archiveDevice.IsChecked = true;
						archiveDevice.ExpandToThis();
						return;
					}
				}
			}

			OnPropertyChanged(() => RootDevices);
		}

		#region DeviceSelection
		public CheckBoxItemList<ArchiveDeviceViewModel> AllDevices { get; private set; }

		public void FillAllDevices()
		{
			AllDevices = new CheckBoxItemList<ArchiveDeviceViewModel>();
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
				foreach (ArchiveDeviceViewModel archiveDevice in AllDevices.Items)
				{
					if (archiveDevice.Device.UID == deviceUID)
					{
						archiveDevice.ExpandToThis();
						SelectedDevice = archiveDevice;
						return;
					}
				}
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
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		ArchiveDeviceViewModel _rootDevice;
		public ArchiveDeviceViewModel RootDevice
		{
			get { return _rootDevice; }
			private set
			{
				_rootDevice = value;
				OnPropertyChanged(() => RootDevice);
			}
		}

		public ArchiveDeviceViewModel[] RootDevices
		{
			get { return new ArchiveDeviceViewModel[] { RootDevice }; }
		}

		void BuildTree()
		{
			RootDevice = AddDeviceInternal(GKManager.DeviceConfiguration.RootDevice, null);
			FillAllDevices();
		}

		private ArchiveDeviceViewModel AddDeviceInternal(GKDevice device, ArchiveDeviceViewModel parentDeviceViewModel)
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
				OnPropertyChanged(() => StartDateTime);
			}
		}

		DateTimePairViewModel _endDateTime;
		public DateTimePairViewModel EndDateTime
		{
			get { return _endDateTime; }
			set
			{
				_endDateTime = value;
				OnPropertyChanged(() => EndDateTime);
			}
		}

		bool useDeviceDateTime;
		public bool UseDeviceDateTime
		{
			get { return useDeviceDateTime; }
			set
			{
				useDeviceDateTime = value;
				OnPropertyChanged(() => UseDeviceDateTime);
			}
		}

		public CheckBoxItemList<JournalItemTypeViewModel> JournalItemTypes { get; private set; }
		public CheckBoxItemList<StateClassViewModel> StateClasses { get; private set; }
		public CheckBoxItemList<EventNameViewModel> EventNames { get; private set; }
		public CheckBoxItemList<ArchiveDescriptionViewModel> ArchiveDescriptions { get; private set; }
		public CheckBoxItemList<ArchiveZoneViewModel> ArchiveZones { get; private set; }
		public CheckBoxItemList<ArchiveDirectionViewModel> ArchiveDirections { get; private set; }
		public CheckBoxItemList<SubsystemTypeViewModel> SubsystemTypes { get; private set; }
		public CheckBoxItemList<ArchivePumpStationViewModel> PumpStations { get; private set; }
		public CheckBoxItemList<ArchiveMPTViewModel> MPTs { get; private set; }
		public CheckBoxItemList<ArchiveDelayViewModel> Delays { get; private set; }
		public CheckBoxItemList<ArchivePimViewModel> PIMs { get; private set; }
		public CheckBoxItemList<ArchiveGuardZoneViewModel> ArchiveGuardZones { get; private set; }
		List<string> DistinctDatabaseNames = FiresecManager.FiresecService.GetGkEventNames();
		List<string> DistinctDatabaseDescriptions = FiresecManager.FiresecService.GetGkEventDescriptions();

		public GKArchiveFilter GetModel()
		{
			var archiveFilter = new GKArchiveFilter()
			{
				StartDate = StartDateTime.DateTime,
				EndDate = EndDateTime.DateTime,
				UseDeviceDateTime = UseDeviceDateTime
			};
			foreach (var journalItemType in JournalItemTypes.Items)
			{
				if (journalItemType.IsChecked)
					archiveFilter.XJournalObjectTypes.Add(journalItemType.JournalItemType);
			}
			foreach (var stateClass in StateClasses.Items)
			{
				if (stateClass.IsChecked)
					archiveFilter.StateClasses.Add(stateClass.StateClass);
			}
			foreach (var eventName in EventNames.Items)
			{
				if (eventName.IsChecked)
					archiveFilter.EventNames.Add(eventName.Name);
			}
			foreach (var description in ArchiveDescriptions.Items)
			{
				if (description.IsChecked)
					archiveFilter.Descriptions.Add(description.Description);
			}
			foreach (var archiveDevice in AllDevices.Items)
			{
				if (archiveDevice.IsChecked)
					archiveFilter.DeviceUIDs.Add(archiveDevice.Device.UID);
			}
			foreach (var archiveZone in ArchiveZones.Items)
			{
				if (archiveZone.IsChecked)
					archiveFilter.ZoneUIDs.Add(archiveZone.Zone.UID);
			}
			foreach (var archiveDirection in ArchiveDirections.Items)
			{
				if (archiveDirection.IsChecked)
					archiveFilter.DirectionUIDs.Add(archiveDirection.Direction.UID);
			}
			foreach (var subsystemType in SubsystemTypes.Items)
			{
				if (subsystemType.IsChecked)
					archiveFilter.SubsystemTypes.Add(subsystemType.SubsystemType);
			}
			foreach (var pumpStation in PumpStations.Items)
			{
				if (pumpStation.IsChecked)
					archiveFilter.PumpStationUIDs.Add(pumpStation.PumpStation.UID);
			}
			foreach (var mpt in MPTs.Items)
			{
				if (mpt.IsChecked)
					archiveFilter.MPTUIDs.Add(mpt.MPT.UID);
			}
			foreach (var delay in Delays.Items)
			{
				if (delay.IsChecked)
					archiveFilter.DelayUIDs.Add(delay.Delay.UID);
			}
			foreach (var pim in PIMs.Items)
			{
				if (pim.IsChecked)
					archiveFilter.PimUIDs.Add(pim.Pim.UID);
			}
			foreach (var archiveGuardZone in ArchiveGuardZones.Items)
			{
				if (archiveGuardZone.IsChecked)
					archiveFilter.GuardZoneUIDs.Add(archiveGuardZone.GuardZone.UID);
			}
			return archiveFilter;
		}

		public RelayCommand ClearCommand { get; private set; }
		void OnClear()
		{
			Initialize(new GKArchiveFilter());
			OnPropertyChanged(() => JournalItemTypes);
			OnPropertyChanged(() => StateClasses);
			OnPropertyChanged(() => EventNames);
			OnPropertyChanged(() => ArchiveDescriptions);
			OnPropertyChanged(() => AllDevices);
			OnPropertyChanged(() => ArchiveZones);
			OnPropertyChanged(() => ArchiveDirections);
			OnPropertyChanged(() => SubsystemTypes);
			OnPropertyChanged(() => PumpStations);
			OnPropertyChanged(() => MPTs);
			OnPropertyChanged(() => Delays);
			OnPropertyChanged(() => PIMs);
			OnPropertyChanged(() => ArchiveGuardZones);
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			if (StartDateTime.DateTime > EndDateTime.DateTime)
			{
				MessageBoxService.ShowWarning2("Начальная дата должна быть меньше конечной");
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