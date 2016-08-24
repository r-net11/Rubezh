using System.Windows.Input;
using Localization.Strazh.ViewModels;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using StrazhModule.Devices;

namespace StrazhModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		#region Properties
		public SKDDevice Device { get; private set; }

		public SKDDeviceState State
		{
			get { return Device.State; }
		}

		public DeviceStateViewModel DeviceStateViewModel { get; private set; }

		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }

		public string PresentationAddress
		{
			get { return Device.Address; }
		}

		public bool IsEnabled { get; private set; }

		public bool IsLock
		{
			get { return Device.DriverType == SKDDriverType.Lock; }
		}
		#endregion

		#region Constructors

		public DeviceViewModel(SKDDevice device)
		{
			Device = device;
			DeviceStateViewModel = new DeviceStateViewModel(State);
			State.StateChanged -= OnStateChanged;
			State.StateChanged += OnStateChanged;
			OnStateChanged();

			DeviceCommandsViewModel = new DeviceCommandsViewModel(Device);
			OpenCommand = new RelayCommand(OnOpen, CanOpen);
			CloseCommand = new RelayCommand(OnClose, CanClose);
			DeviceAccessStateNormalCommand = new RelayCommand(OnDeviceAccessStateNormal, CanDeviceAccessStateNormal);
			DeviceAccessStateCloseAlwaysCommand = new RelayCommand(OnDeviceAccessStateCloseAlways, CanDeviceAccessStateCloseAlways);
			DeviceAccessStateOpenAlwaysCommand = new RelayCommand(OnDeviceAccessStateOpenAlways, CanDeviceAccessStateOpenAlways);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal, CanShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);
			ShowZoneDetailsCommand = new RelayCommand(OnShowZoneDetails, CanShowZone);
			ShowDoorCommand = new RelayCommand(OnShowDoor, CanShowDoor);
			ClearPromptWarningCommand = new RelayCommand(OnClearPromptWarning, CanClearPromptWarning);

			if(Device.Zone != null)
			{
				Zone = new ZoneViewModel(Device.Zone);
			}

			IsEnabled = Device.IsEnabled;
		}

		#endregion

		#region Methods

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => DeviceStateViewModel);
		}

		#endregion

		#region <Zone>

		public ZoneViewModel Zone { get; private set; }

		public bool HasZone
		{
			get { return Device.Zone != null; }
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			ServiceFactoryBase.Events.GetEvent<ShowSKDZoneEvent>().Publish(Device.Zone.UID);
		}
		bool CanShowZone()
		{
			return Device.Zone != null;
		}

		public ICommand ShowZoneDetailsCommand { get; private set; }
		private void OnShowZoneDetails()
		{
			DialogService.ShowWindow(new ZoneDetailsViewModel(Zone.Zone));
		}

		#endregion </Zone>

		#region Commands

		public RelayCommand ShowOnPlanCommand { get; private set; }
		private void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowSKDDevice(Device);
		}
		private bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowSKDDevice(Device);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		private void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowArchiveEventArgs
			{
				SKDDevice = Device
			};
			ServiceFactoryBase.Events.GetEvent<ShowArchiveEvent>().Publish(showSKDArchiveEventArgs);
		}
		private bool CanShowJournal()
		{
			return Device.IsRealDevice;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new DeviceDetailsViewModel(Device));
		}
		public bool CanShowProperties()
		{
			return Device.IsRealDevice;
		}

		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			DeviceCommander.Open(Device);
		}
		bool CanOpen()
		{
			return DeviceCommander.CanOpen(Device);
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			DeviceCommander.Close(Device);
		}
		bool CanClose()
		{
			return DeviceCommander.CanClose(Device);
		}

		public RelayCommand DeviceAccessStateNormalCommand { get; private set; }
		void OnDeviceAccessStateNormal()
		{
			DeviceCommander.SetAccessStateToNormal(Device);
		}
		bool CanDeviceAccessStateNormal()
		{
			return DeviceCommander.CanSetAccessStateToNormal(Device);
		}

		public RelayCommand DeviceAccessStateCloseAlwaysCommand { get; private set; }
		void OnDeviceAccessStateCloseAlways()
		{
			DeviceCommander.SetAccessStateToCloseAlways(Device);
		}
		bool CanDeviceAccessStateCloseAlways()
		{
			return DeviceCommander.CanSetAccessStateToCloseAlways(Device);
		}

		public RelayCommand DeviceAccessStateOpenAlwaysCommand { get; private set; }
		void OnDeviceAccessStateOpenAlways()
		{
			DeviceCommander.SetAccessStateToOpenAlways(Device);
		}
		bool CanDeviceAccessStateOpenAlways()
		{
			return DeviceCommander.CanSetAccessStateToOpenAlways(Device);
		}

		public RelayCommand ClearPromptWarningCommand { get; private set; }
		private void OnClearPromptWarning()
		{
			DeviceCommander.ClearPromptWarning(Device);
		}
		private bool CanClearPromptWarning()
		{
			return DeviceCommander.CanClearPromptWarning(Device);
		}

		#endregion

		#region <Door>

		public SKDDoor Door
		{
			get { return Device.Door; }
		}

		public bool HasDoor
		{
			get { return Device.Door != null; }
		}

		public RelayCommand ShowDoorCommand { get; private set; }
		void OnShowDoor()
		{
			ServiceFactoryBase.Events.GetEvent<ShowSKDDoorEvent>().Publish(Device.Door.UID);
		}
		bool CanShowDoor()
		{
			return Device.Door != null;
		}

		#endregion </Door>

		/// <summary>
		/// Режим (односторонний / двухсторонний)
		/// </summary>
		public string DoorTypeDescription
		{
			get
			{
				if (Device.Driver.IsController)
				{
					switch (Device.DoorType)
					{
						case DoorType.OneWay:
							return CommonViewModels.Oneway;
						case DoorType.TwoWay:
							return CommonViewModels.Twoway;
					}
				}
				return string.Empty;
			}
		}
	}
}