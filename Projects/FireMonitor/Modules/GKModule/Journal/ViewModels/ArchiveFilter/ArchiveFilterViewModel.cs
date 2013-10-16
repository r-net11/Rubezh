using System;
using System.Collections.Generic;
using System.Linq;
using Common.GK;
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
			StartDateTime = archiveFilter.StartDate;
			EndDateTime = archiveFilter.EndDate;

			InitializeJournalItemTypes(archiveFilter);
			InitializeStateClasses(archiveFilter);
			InitializeGKAddresses(archiveFilter);
			InitializeEventNames(archiveFilter);
			InitializeDevices(archiveFilter);
			InitializeZones(archiveFilter);
			InitializeDirections(archiveFilter);
		}

		void InitializeJournalItemTypes(XArchiveFilter archiveFilter)
		{
			JournalItemTypes = new List<JournalItemTypeViewModel>();
			JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.Device));
			JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.Direction));
			JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.GK));
			JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.System));
			JournalItemTypes.Add(new JournalItemTypeViewModel(JournalItemType.Zone));

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

		void InitializeGKAddresses(XArchiveFilter archiveFilter)
		{
			GKAddresses = new List<GKAddressViewModel>();
			var addresses = GKDBHelper.GetGKIPAddresses();
			foreach (var address in addresses)
			{
				var addressViewModel = new GKAddressViewModel(address);
				GKAddresses.Add(addressViewModel);
			}

			foreach (var address in archiveFilter.GKAddresses)
			{
				var addressViewModel = GKAddresses.FirstOrDefault(x => x.Address == address);
				if (addressViewModel != null)
				{
					addressViewModel.IsChecked = true;
				}
			}
		}

		public void InitializeEventNames(XArchiveFilter archiveFilter)
		{
			EventNames = new List<EventNameViewModel>();
			foreach (var xEvent in EventHelper.GetAllEvents())
			{
				EventNames.Add(new EventNameViewModel(xEvent));
			}
			foreach (var xEvent in archiveFilter.Events)
			{
                var eventNameViewModel = EventNames.FirstOrDefault(x => x.XEvent == xEvent);
				if (eventNameViewModel != null)
				{
					eventNameViewModel.IsChecked = true;
				}
			}
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

		DateTime _startDateTime;
		public DateTime StartDateTime
		{
			get { return _startDateTime; }
			set
			{
				_startDateTime = value;
				OnPropertyChanged("StartDateTime");
			}
		}

		DateTime _endDateTime;
		public DateTime EndDateTime
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
		public List<GKAddressViewModel> GKAddresses { get; private set; }
		public List<EventNameViewModel> EventNames { get; private set; }
		public List<ArchiveZoneViewModel> ArchiveZones { get; private set; }
		public List<ArchiveDirectionViewModel> ArchiveDirections { get; private set; }

		public XArchiveFilter GetModel()
		{
			var archiveFilter = new XArchiveFilter()
			{
				StartDate = StartDateTime,
				EndDate = EndDateTime,
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
			foreach (var addresses in GKAddresses)
			{
				if (addresses.IsChecked)
					archiveFilter.GKAddresses.Add(addresses.Address);
			}
			foreach (var eventName in EventNames)
			{
				if (eventName.IsChecked)
					archiveFilter.Events.Add(eventName.XEvent);
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
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			if (StartDateTime > EndDateTime)
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