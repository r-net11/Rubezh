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
			var journalItemTypeList = new List<ICheckBoxItem>();
			journalItemTypeList.Add(new JournalItemTypeViewModel(JournalItemType.Device));
			journalItemTypeList.Add(new JournalItemTypeViewModel(JournalItemType.Direction));
			journalItemTypeList.Add(new JournalItemTypeViewModel(JournalItemType.GK));
			journalItemTypeList.Add(new JournalItemTypeViewModel(JournalItemType.System));
			journalItemTypeList.Add(new JournalItemTypeViewModel(JournalItemType.Zone));
			journalItemTypeList.Add(new JournalItemTypeViewModel(JournalItemType.PumpStation));
			journalItemTypeList.Add(new JournalItemTypeViewModel(JournalItemType.Delay));
			journalItemTypeList.Add(new JournalItemTypeViewModel(JournalItemType.Pim));

			JournalItemTypes = new CheckBoxItemList(journalItemTypeList);

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
			var stateClassesList = new List<ICheckBoxItem>();
			foreach (XStateClass stateClass in Enum.GetValues(typeof(XStateClass)))
			{
				var stateClassViewModel = new StateClassViewModel(stateClass);
				stateClassesList.Add(stateClassViewModel);
			}
			StateClasses = new CheckBoxItemList(stateClassesList);

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
			var journalDescriptionStateList = new List<EventNameViewModel>();
            foreach (var journalDescriptionState in EventNameHelper.EventNames)
            {
				journalDescriptionStateList.Add(new EventNameViewModel(journalDescriptionState, DistinctDatabaseNames));
            }
			journalDescriptionStateList.Sort(EventNameViewModel.Compare);
			EventNames = new CheckBoxItemList(journalDescriptionStateList.ToList<ICheckBoxItem>());
			
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
			var archiveZoneList = new List<ICheckBoxItem>();
			foreach (var zone in XManager.Zones)
			{
				var archiveZoneViewModel = new ArchiveZoneViewModel(zone);
				archiveZoneList.Add(archiveZoneViewModel);
			}
			ArchiveZones = new CheckBoxItemList(archiveZoneList);

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
			var archiveDirectionList = new List<ICheckBoxItem>();
			foreach (var direction in XManager.Directions)
			{
				var archiveDirectionViewModel = new ArchiveDirectionViewModel(direction);
				archiveDirectionList.Add(archiveDirectionViewModel);
			}
			ArchiveDirections = new CheckBoxItemList(archiveDirectionList);
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
			var archiveDescriptionList = new List<ArchiveDescriptionViewModel>();
			foreach (var description in DescriptionsHelper.GetAllDescriptions())
			{
				archiveDescriptionList.Add(new ArchiveDescriptionViewModel(description, DistinctDatabaseDescriptions));
            }
			archiveDescriptionList.Sort(ArchiveDescriptionViewModel.Compare);
			ArchiveDescriptions = new CheckBoxItemList(archiveDescriptionList.ToList<ICheckBoxItem>());
			foreach (var description in archiveFilter.Descriptions)
			{
				var descriptionViewModel = ArchiveDescriptions.Items.FirstOrDefault(x => (x as ArchiveDescriptionViewModel).Description.Name == description);
                if(descriptionViewModel != null)
				{
					descriptionViewModel.IsChecked = true;
				}
			}
		}

		void InitializeSubsystemTypes(XArchiveFilter archiveFilter)
		{
			var subsystemTypeList = new List<ICheckBoxItem>();
			foreach (XSubsystemType item in Enum.GetValues(typeof(XSubsystemType)))
			{
				subsystemTypeList.Add(new SubsystemTypeViewModel(item));
			}
			SubsystemTypes = new CheckBoxItemList(subsystemTypeList);
			foreach (var subsystemType in archiveFilter.SubsystemTypes)
			{
				var subsystemTypeViewModel = SubsystemTypes.Items.FirstOrDefault(x => (x as SubsystemTypeViewModel).SubsystemType == subsystemType);
				if (subsystemTypeViewModel != null)
				{
					subsystemTypeViewModel.IsChecked = true;
				}
			}
		}

		void InitializePumpStations(XArchiveFilter archiveFilter)
		{
			var pumpStationList = new List<ICheckBoxItem>();
			foreach (var direction in XManager.PumpStations)
			{
				var archiveDirectionViewModel = new ArchivePumpStationViewModel(direction);
				pumpStationList.Add(archiveDirectionViewModel);
			}
			PumpStations = new CheckBoxItemList(pumpStationList);
			foreach (var uid in archiveFilter.PumpStationUIDs)
			{
				var pumpStation = PumpStations.Items.FirstOrDefault(x => (x as ArchivePumpStationViewModel).PumpStation.UID == uid);
				if (pumpStation != null)
				{
					pumpStation.IsChecked = true;
				}
			}
		}

		void InitializePIMs(XArchiveFilter archiveFilter)
		{
			var PIMList = new List<ICheckBoxItem>();
			foreach (var pim in XManager.Pims)
			{
				var archivePimViewModel = new ArchivePimViewModel(pim);
				PIMList.Add(archivePimViewModel);
			}
			PIMs = new CheckBoxItemList(PIMList);
			foreach (var uid in archiveFilter.PimUIDs)
			{
				var pim = PIMs.Items.FirstOrDefault(x => (x as ArchivePimViewModel).Pim.UID == uid);
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
		public CheckBoxItemList AllDevices { get; private set; }

		public void FillAllDevices()
		{
			AllDevices = new CheckBoxItemList();
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

		public CheckBoxItemList JournalItemTypes { get; private set; }
		public CheckBoxItemList StateClasses { get; private set; }
		public CheckBoxItemList EventNames { get; private set; }
		public CheckBoxItemList ArchiveZones { get; private set; }
		public CheckBoxItemList ArchiveDirections { get; private set; }
		public CheckBoxItemList ArchiveDescriptions { get; private set; }
		public CheckBoxItemList SubsystemTypes { get; private set; }
		public CheckBoxItemList PumpStations { get; private set; }
		public CheckBoxItemList PIMs { get; private set; }
        List<string> DistinctDatabaseNames = FiresecManager.FiresecService.GetDistinctGKJournalNames();
		List<string> DistinctDatabaseDescriptions = FiresecManager.FiresecService.GetDistinctGKJournalDescriptions();

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
					archiveFilter.JournalItemTypes.Add((journalItemType as JournalItemTypeViewModel).JournalItemType);
			}
			foreach (var stateClass in StateClasses.Items)
			{
				if (stateClass.IsChecked)
					archiveFilter.StateClasses.Add((stateClass as StateClassViewModel).StateClass);
			}
			foreach (var eventName in EventNames.Items)
			{
				if (eventName.IsChecked)
                    archiveFilter.EventNames.Add((eventName as EventNameViewModel).EventName.Name);
			}
			foreach (var archiveDevice in AllDevices.Items)
			{
				if (archiveDevice.IsChecked)
					archiveFilter.DeviceUIDs.Add((archiveDevice as ArchiveDeviceViewModel).Device.UID);
			}
			foreach (var archiveZone in ArchiveZones.Items)
			{
				if (archiveZone.IsChecked)
					archiveFilter.ZoneUIDs.Add((archiveZone as ArchiveZoneViewModel).Zone.UID);
			}
			foreach (var archiveDirection in ArchiveDirections.Items)
			{
				if (archiveDirection.IsChecked)
					archiveFilter.DirectionUIDs.Add((archiveDirection as ArchiveDirectionViewModel).Direction.UID);
			}
			foreach (var description in ArchiveDescriptions.Items)
			{
				if (description.IsChecked)
					archiveFilter.Descriptions.Add((description as ArchiveDescriptionViewModel).Description.Name);
			}
			foreach (var subsystemType in SubsystemTypes.Items)
			{
				if (subsystemType.IsChecked)
					archiveFilter.SubsystemTypes.Add((subsystemType as SubsystemTypeViewModel).SubsystemType);
			}
			foreach (var pumpStation in PumpStations.Items)
			{
				if (pumpStation.IsChecked)
					archiveFilter.PumpStationUIDs.Add((pumpStation as ArchivePumpStationViewModel).PumpStation.UID);
			}
			foreach (var pim in PIMs.Items)
			{
				if (pim.IsChecked)
					archiveFilter.PimUIDs.Add((pim as ArchivePimViewModel).Pim.UID);
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
			OnPropertyChanged("EventNames");
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