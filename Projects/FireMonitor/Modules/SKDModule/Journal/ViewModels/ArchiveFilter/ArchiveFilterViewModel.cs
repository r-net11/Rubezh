using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.CheckBoxList;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ArchiveFilterViewModel : DialogViewModel
	{
		public ArchiveFilterViewModel(SKDArchiveFilter archiveFilter)
		{
			Title = "Настройки фильтра";
			ClearCommand = new RelayCommand(OnClear);
			SaveCommand = new RelayCommand(OnSave);
			CancelCommand = new RelayCommand(OnCancel);
			Initialize(archiveFilter);
		}

		void Initialize(SKDArchiveFilter archiveFilter)
		{
			StartDateTime = new DateTimePairViewModel(archiveFilter.StartDate);
			EndDateTime = new DateTimePairViewModel(archiveFilter.EndDate);

			InitializeJournalItemTypes(archiveFilter);
			InitializeStateClasses(archiveFilter);
			InitializeEventNames(archiveFilter);
			InitializeDevices(archiveFilter);
			InitializeDescriptions(archiveFilter);
			InitializeSubsystemTypes(archiveFilter);
		}

		void InitializeJournalItemTypes(SKDArchiveFilter archiveFilter)
		{
			JournalItemTypes = new CheckBoxItemList<JournalItemTypeViewModel>();
			JournalItemTypes.Add(new JournalItemTypeViewModel(SKDJournalItemType.Reader));
			JournalItemTypes.Add(new JournalItemTypeViewModel(SKDJournalItemType.Controller));
			JournalItemTypes.Add(new JournalItemTypeViewModel(SKDJournalItemType.System));
			foreach (var journalItemType in archiveFilter.JournalItemTypes)
			{
				var JournalItemTypeViewModel = JournalItemTypes.Items.FirstOrDefault(x => (x as JournalItemTypeViewModel).JournalItemType == journalItemType);
				if (JournalItemTypeViewModel != null)
				{
					JournalItemTypeViewModel.IsChecked = true;
				}
			}
		}

		void InitializeStateClasses(SKDArchiveFilter archiveFilter)
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

		public void InitializeEventNames(SKDArchiveFilter archiveFilter)
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

		public void InitializeDescriptions(SKDArchiveFilter archiveFilter)
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

		void InitializeSubsystemTypes(SKDArchiveFilter archiveFilter)
		{
			SubsystemTypes = new CheckBoxItemList<SubsystemTypeViewModel>();
			foreach (SubsystemType item in Enum.GetValues(typeof(SubsystemType)))
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

		#region Devices
		public void InitializeDevices(SKDArchiveFilter archiveFilter)
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
					if (archiveDevice.Device.BaseUID == deviceUID)
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
					if (archiveDevice.Device.BaseUID == deviceUID)
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
		public CheckBoxItemList<ArchiveDescriptionViewModel> ArchiveDescriptions { get; private set; }
		public CheckBoxItemList<SubsystemTypeViewModel> SubsystemTypes { get; private set; }
		List<string> DistinctDatabaseNames = FiresecManager.FiresecService.GetGkEventNames();
		List<string> DistinctDatabaseDescriptions = FiresecManager.FiresecService.GetGkEventDescriptions();

		public SKDArchiveFilter GetModel()
		{
			var archiveFilter = new SKDArchiveFilter()
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
					archiveFilter.DeviceUIDs.Add(archiveDevice.Device.BaseUID);
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
			return archiveFilter;
		}

		public RelayCommand ClearCommand { get; private set; }
		void OnClear()
		{
			Initialize(new SKDArchiveFilter());
			OnPropertyChanged(()=>JournalItemTypes);
			OnPropertyChanged(()=>StateClasses);
			OnPropertyChanged(()=>EventNames);
			OnPropertyChanged(()=>AllDevices);
			OnPropertyChanged(()=>ArchiveDescriptions);
			OnPropertyChanged(()=>SubsystemTypes);
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