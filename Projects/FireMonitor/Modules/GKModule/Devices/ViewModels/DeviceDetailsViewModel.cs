using DeviceControls;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.PlanLink.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;

namespace GKModule.ViewModels
{
	public class DeviceDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		string _savedSelectedMeasureParameterName;
		public GKDevice Device { get; private set; }
		public GKState State
		{
			get { return Device.State; }
		}
		public DeviceStateViewModel DeviceStateViewModel { get; private set; }
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }
		public DevicePropertiesViewModel DevicePropertiesViewModel { get; private set; }
		BackgroundWorker BackgroundWorker;
		bool CancelBackgroundWorker;

		public DeviceDetailsViewModel(GKDevice device)
		{
			ShowCommand = new RelayCommand(OnShow);
			ShowParentCommand = new RelayCommand(OnShowParent, CanShowParent);
			ShowZoneCommand = new RelayCommand(OnShowZone);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			Device = device;
			DeviceStateViewModel = new DeviceStateViewModel(State, device.Driver.IsAm);
			State.StateChanged += OnStateChanged;
			State.MeasureParametersChanged += OnMeasureParametersChanged;
			DeviceCommandsViewModel = new DeviceCommandsViewModel(Device);
			DevicePropertiesViewModel = new DevicePropertiesViewModel(Device);
			PlanLinks = new PlanLinksViewModel(Device.PlanElementUIDs);

			Title = Device.PresentationName;
			StartMeasureParametersMonitoring();

			ServiceFactory.Events.GetEvent<GKObjectsPropertyChangedEvent>().Unsubscribe(OnGKObjectsPropertyChanged);
			ServiceFactory.Events.GetEvent<GKObjectsPropertyChangedEvent>().Subscribe(OnGKObjectsPropertyChanged);
		}
		public string DescriptorName
		{
			get { return Device.GetGKDescriptorName(GKManager.DeviceConfiguration.GKNameGenerationType); }
		}
		void OnStateChanged()
		{
			OnPropertyChanged(() => DevicePicture);
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => DeviceStateViewModel);
			OnPropertyChanged(() => HasOnDelay);
			OnPropertyChanged(() => HasHoldDelay);
			OnPropertyChanged(() => HasOffDelay);
			OnPropertyChanged(() => HasRunningTime);
			CommandManager.InvalidateRequerySuggested();
		}

		public Brush DevicePicture
		{
			get { return PictureCacheSource.GKDevicePicture.GetDynamicBrush(Device); }
		}

		public string PresentationZone
		{
			get { return GKManager.GetPresentationZoneOrLogic(Device); }
		}

		public bool HasOnDelay
		{
			get { return State.StateClasses.Contains(XStateClass.TurningOn) && State.OnDelay > 0; }
		}
		public bool HasHoldDelay
		{
			get
			{
				return State.StateClasses.Contains(XStateClass.On) && State.HoldDelay > 0;
			}
		}
		public bool HasRunningTime
		{
			get
			{
				return (State.StateClasses.Contains(XStateClass.TurningOn) || State.StateClasses.Contains(XStateClass.TurningOff)) && State.RunningTime > 0;
			}
		}
		public bool HasOffDelay
		{
			get
			{
				return State.StateClasses.Contains(XStateClass.TurningOff) && State.OffDelay > 0;
			}
		}

		#region Measure Parameters
		ObservableCollection<MeasureParameterViewModel> _measureParameters;
		public ObservableCollection<MeasureParameterViewModel> MeasureParameters
		{
			get { return _measureParameters; }
			set
			{
				_measureParameters = value;
				OnPropertyChanged(() => MeasureParameters);
			}
		}

		public bool HasMeasureParameters
		{
			get { return Device.Driver.MeasureParameters.Where(x => !x.IsDelay && !x.IsNotVisible).Count() > 0 || Device.DriverType == GKDriverType.RSR2_Valve_DU || Device.DriverType == GKDriverType.RSR2_Valve_KV || Device.DriverType == GKDriverType.RSR2_Valve_KVMV; }
		}
		MeasureParameterViewModel _selectedMeasureParemeter;
		public MeasureParameterViewModel SelectedMeasureParameter
		{
			get { return _selectedMeasureParemeter; }
			set
			{
				_selectedMeasureParemeter = value;
				if (value != null)
					_savedSelectedMeasureParameterName = value.Name;
				OnPropertyChanged(() => SelectedMeasureParameter);
			}
		}

		void StartMeasureParametersMonitoring()
		{
			MeasureParameters = new ObservableCollection<MeasureParameterViewModel>();
			foreach (var measureParameter in Device.Driver.MeasureParameters.Where(x => !x.IsNotVisible))
			{
				var measureParameterViewModel = new MeasureParameterViewModel()
				{
					Name = measureParameter.Name,
					IsDelay = measureParameter.IsDelay
				};
				MeasureParameters.Add(measureParameterViewModel);
			}

			BackgroundWorker = new BackgroundWorker();
			BackgroundWorker.DoWork += UpdateMeasureParameters;
			BackgroundWorker.RunWorkerAsync();
		}

		void UpdateMeasureParameters(object sender, DoWorkEventArgs e)
		{
			while (true)
			{
				if (CancelBackgroundWorker)
					break;
				ClientManager.FiresecService.GKStartMeasureMonitoring(Device);
				Thread.Sleep(TimeSpan.FromSeconds(10));
			}
			ClientManager.FiresecService.GKStopMeasureMonitoring(Device);
		}

		void OnMeasureParametersChanged()
		{
			MeasureParameters = new ObservableCollection<MeasureParameterViewModel>();
			foreach (var measureParameter in Device.State.XMeasureParameterValues)
			{
				if (Device.Driver.MeasureParameters.Any(x => x.Name == measureParameter.Name && x.IsNotVisible))
					continue;
				var measureParameterViewModel = new MeasureParameterViewModel()
				{
					Name = measureParameter.Name,
					StringValue = measureParameter.StringValue
				};
				MeasureParameters.Add(measureParameterViewModel);
				SelectedMeasureParameter = _measureParameters.FirstOrDefault(x => x.Name == _savedSelectedMeasureParameterName);
			}
		}
		#endregion

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowGKDeviceEvent>().Publish(Device.UID);
		}

		public RelayCommand ShowParentCommand { get; private set; }
		void OnShowParent()
		{
			ServiceFactory.Events.GetEvent<ShowGKDeviceEvent>().Publish(Device.Parent.UID);
		}
		bool CanShowParent()
		{
			return Device.Parent != null;
		}

		public PlanLinksViewModel PlanLinks { get; private set; }

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			var zone = Device.Zones.FirstOrDefault();
			if (zone != null)
			{
				ServiceFactory.Events.GetEvent<ShowGKZoneEvent>().Publish(zone.UID);
			}
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (Device != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { Device.UID });
		}

		public bool CanNotControl
		{
			get { return !(Device.Driver.IsControlDevice || (Device.Driver.IsDeviceOnShleif && !Device.Driver.IsControlDevice)) || !ClientManager.CheckPermission(PermissionType.Oper_Device_Control); }
		}

		void OnGKObjectsPropertyChanged(GKPropertyChangedCallback gkPropertyChangedCallback)
		{
			DevicePropertiesViewModel = new DevicePropertiesViewModel(Device);
			OnPropertyChanged(() => DevicePropertiesViewModel);
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Device.UID.ToString(); }
		}
		#endregion

		public override void OnClosed()
		{
			CancelBackgroundWorker = true;
			State.StateChanged -= OnStateChanged;
		}
	}
}