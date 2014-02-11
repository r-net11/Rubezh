using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

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
			InitializeEventNames(archiveFilter);
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
			JournalItemTypes = new CheckBoxItemList<JournalItemTypeViewModel>();
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
				var JournalItemTypeViewModel = JournalItemTypes.Items.FirstOrDefault(x => (x as JournalItemTypeViewModel).JournalItemType == journalItemType);
				if (JournalItemTypeViewModel != null)
				{
					JournalItemTypeViewModel.IsChecked = true;
				}
			}
		}

		void InitializeStateClasses(XArchiveFilter archiveFilter)
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

		public void InitializeEventNames(XArchiveFilter archiveFilter)
		{
			EventNames = new CheckBoxItemList<EventNameViewModel>();
            foreach (var eventName in EventNameHelper.EventNames)
			{
				EventNames.Add(new EventNameViewModel(eventName, DistinctDatabaseNames));
			}
			EventNames.Items.Sort(EventNameViewModel.Compare);
			foreach (var eventName in archiveFilter.EventNames)
			{
				var eventNameViewModel = EventNames.Items.FirstOrDefault(x => (x as EventNameViewModel).EventName.Name == eventName);
				if (eventNameViewModel != null)
				{
					eventNameViewModel.IsChecked = true;
				}
			}
		}

		public void InitializeZones(XArchiveFilter archiveFilter)
		{
			ArchiveZones = new CheckBoxItemList<ArchiveZoneViewModel>();
			foreach (var zone in XManager.Zones)
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

		public void InitializeDirections(XArchiveFilter archiveFilter)
		{
			ArchiveDirections = new CheckBoxItemList<ArchiveDirectionViewModel>();
			foreach (var direction in XManager.Directions)
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

		public void InitializeDescriptions(XArchiveFilter archiveFilter)
		{
			ArchiveDescriptions = new CheckBoxItemList<ArchiveDescriptionViewModel>();
			foreach (var description in DescriptionsHelper.GetAllDescriptions())
			{
				ArchiveDescriptions.Add(new ArchiveDescriptionViewModel(description, DistinctDatabaseDescriptions));
			}
			ArchiveDescriptions.Items.Sort(ArchiveDescriptionViewModel.Compare);
			foreach (var description in archiveFilter.Descriptions)
			{
				var descriptionViewModel = ArchiveDescriptions.Items.FirstOrDefault(x => (x as ArchiveDescriptionViewModel).Description.Name == description);
				if (descriptionViewModel != null)
				{
					descriptionViewModel.IsChecked = true;
				}
			}
		}

		void InitializeSubsystemTypes(XArchiveFilter archiveFilter)
		{
			SubsystemTypes = new CheckBoxItemList<SubsystemTypeViewModel>();
			foreach (XSubsystemType item in Enum.GetValues(typeof(XSubsystemType)))
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

		void InitializePumpStations(XArchiveFilter archiveFilter)
		{
			PumpStations = new CheckBoxItemList<ArchivePumpStationViewModel>();
			foreach (var direction in XManager.PumpStations)
			{
				var archiveDirectionViewModel = new ArchivePumpStationViewModel(direction);
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

		void InitializePIMs(XArchiveFilter archiveFilter)
		{
			PIMs = new CheckBoxItemList<ArchivePimViewModel>();
			foreach (var pim in XManager.Pims)
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

			OnPropertyChanged("RootDevices");
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

		public CheckBoxItemList<JournalItemTypeViewModel> JournalItemTypes { get; private set; }
		public CheckBoxItemList<StateClassViewModel> StateClasses { get; private set; }
		public CheckBoxItemList<EventNameViewModel> EventNames { get; private set; }
		public CheckBoxItemList<ArchiveZoneViewModel> ArchiveZones { get; private set; }
		public CheckBoxItemList<ArchiveDirectionViewModel> ArchiveDirections { get; private set; }
		public CheckBoxItemList<ArchiveDescriptionViewModel> ArchiveDescriptions { get; private set; }
		public CheckBoxItemList<SubsystemTypeViewModel> SubsystemTypes { get; private set; }
		public CheckBoxItemList<ArchivePumpStationViewModel> PumpStations { get; private set; }
		public CheckBoxItemList<ArchivePimViewModel> PIMs { get; private set; }
		List<string> DistinctDatabaseNames = FiresecManager.FiresecService.GetGkEventNames();
		List<string> DistinctDatabaseDescriptions = FiresecManager.FiresecService.GetGkEventDescriptions();

		public XArchiveFilter GetModel()
		{
			var archiveFilter = new XArchiveFilter()
			{
				StartDate = StartDateTime.DateTime,
				EndDate = EndDateTime.DateTime,
				UseDeviceDateTime = UseDeviceDateTime
			};
			foreach (var journalItemType in JournalItemTypes.Items)
			{
				if (journalItemType.IsChecked)
					archiveFilter.JournalItemTypes.Add(journalItemType.JournalItemType);
			}
			foreach (var stateClass in StateClasses.Items)
			{
				if (stateClass.IsChecked)
					archiveFilter.StateClasses.Add(stateClass.StateClass);
			}
			foreach (var eventName in EventNames.Items)
			{
				if (eventName.IsChecked)
                    archiveFilter.EventNames.Add(eventName.EventName.Name);
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
			foreach (var description in ArchiveDescriptions.Items)
			{
				if (description.IsChecked)
					archiveFilter.Descriptions.Add(description.Description.Name);
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
			foreach (var pim in PIMs.Items)
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
			OnPropertyChanged(()=>JournalItemTypes);
			OnPropertyChanged(()=>StateClasses);
			OnPropertyChanged(()=>EventNames);
			OnPropertyChanged(()=>AllDevices);
			OnPropertyChanged(()=>ArchiveZones);
			OnPropertyChanged(()=>ArchiveDirections);
			OnPropertyChanged(()=>ArchiveDescriptions);
			OnPropertyChanged(()=>SubsystemTypes);
			OnPropertyChanged(()=>PumpStations);
			OnPropertyChanged(()=>PIMs);
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