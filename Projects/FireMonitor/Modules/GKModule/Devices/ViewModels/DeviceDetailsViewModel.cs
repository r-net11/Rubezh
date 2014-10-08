using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;

namespace GKModule.ViewModels
{
	public class DeviceDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public GKDevice Device { get; private set; }
		public GKState State
		{
			get { return Device.State; }
		}
		public DeviceStateViewModel DeviceStateViewModel { get; private set; }
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }
		public DevicePropertiesViewModel DevicePropertiesViewModel { get; private set; }
		BackgroundWorker BackgroundWorker;
		bool CancelBackgroundWorker = false;

		public DeviceDetailsViewModel(GKDevice device)
		{
			ShowCommand = new RelayCommand(OnShow);
			ShowParentCommand = new RelayCommand(OnShowParent, CanShowParent);
			ShowOnPlanCommand = new RelayCommand<Plan>(OnShowOnPlan);
			ShowZoneCommand = new RelayCommand(OnShowZone);
			ShowJournalCommand = new RelayCommand(OnShowJournal);

			Device = device;
			DeviceStateViewModel = new DeviceStateViewModel(State);
			State.StateChanged += new Action(OnStateChanged);
			State.MeasureParametersChanged += new Action(OnMeasureParametersChanged);
			DeviceCommandsViewModel = new DeviceCommandsViewModel(Device);
			DevicePropertiesViewModel = new DevicePropertiesViewModel(Device);
			InitializePlans();

			Title = Device.PresentationName;
			if (!string.IsNullOrEmpty(Device.Description))
				Title += " (" + Device.Description + ")";

			StartMeasureParametersMonitoring();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => DevicePicture);
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => DeviceStateViewModel);
			OnPropertyChanged(() => HasOnDelay);
			OnPropertyChanged(() => HasHoldDelay);
			OnPropertyChanged(() => HasOffDelay);
			CommandManager.InvalidateRequerySuggested();
		}

		public Brush DevicePicture
		{
			get { return PictureCacheSource.GKDevicePicture.GetDynamicBrush(Device); }
		}

		public string PresentationZone
		{
			get { return GKManager.GetPresentationZone(Device); }
		}

		public bool HasOnDelay
		{
			get { return State.StateClasses.Contains(XStateClass.TurningOn) && State.OnDelay > 0; }
		}
		public bool HasHoldDelay
		{
			get { return State.StateClasses.Contains(XStateClass.On) && State.HoldDelay > 0; }
		}
		public bool HasOffDelay
		{
			get { return State.StateClasses.Contains(XStateClass.TurningOff) && State.OffDelay > 0; }
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
			get { return Device.Driver.MeasureParameters.Where(x => !x.IsDelay).Count() > 0; }
		}

		void StartMeasureParametersMonitoring()
		{
			MeasureParameters = new ObservableCollection<MeasureParameterViewModel>();
			foreach (var measureParameter in Device.Driver.MeasureParameters)
			{
				var measureParameterViewModel = new MeasureParameterViewModel()
				{
					Name = measureParameter.Name,
					IsDelay = measureParameter.IsDelay
				};
				MeasureParameters.Add(measureParameterViewModel);
			}

			BackgroundWorker = new BackgroundWorker();
			BackgroundWorker.DoWork += new DoWorkEventHandler(UpdateMeasureParameters);
			BackgroundWorker.RunWorkerAsync();
		}

		void UpdateMeasureParameters(object sender, DoWorkEventArgs e)
		{
			while (true)
			{
				if (CancelBackgroundWorker)
					break;
				FiresecManager.FiresecService.GKStartMeasureMonitoring(Device);
				Thread.Sleep(TimeSpan.FromSeconds(10));
			}
			FiresecManager.FiresecService.GKStopMeasureMonitoring(Device);
		}


		void OnMeasureParametersChanged()
		{
			foreach (var measureParameter in Device.State.XMeasureParameterValues)
			{
				var measureParameterViewModel = MeasureParameters.FirstOrDefault(x => x.Name == measureParameter.Name);
				measureParameterViewModel.Value = measureParameter.Value;
				measureParameterViewModel.StringValue = measureParameter.StringValue;
			}
		}
		#endregion

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(Device.UID);
		}

		public RelayCommand ShowParentCommand { get; private set; }
		void OnShowParent()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(Device.Parent.UID);
		}
		bool CanShowParent()
		{
			return Device.Parent != null;
		}

		public ObservableCollection<PlanLinkViewModel> Plans { get; private set; }
		public bool HasPlans
		{
			get { return Plans.Count > 0; }
		}

		void InitializePlans()
		{
			Plans = new ObservableCollection<PlanLinkViewModel>();
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				ElementBase elementBase = plan.ElementGKDevices.FirstOrDefault(x => x.DeviceUID == Device.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.Device = Device;
					Plans.Add(alarmPlanViewModel);
				}
			}
		}

		public ObservableCollection<PlanViewModel> PlanNames
		{
			get
			{
				var planes = FiresecManager.PlansConfiguration.AllPlans.Where(item => item.ElementGKDevices.Any(element => element.DeviceUID == Device.UID));
				var planViewModels = new ObservableCollection<PlanViewModel>();
				foreach (var plan in planes)
				{
					planViewModels.Add(new PlanViewModel(plan, Device));
				}
				return planViewModels;
			}
		}
		public RelayCommand<Plan> ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan(Plan plan)
		{
			ShowOnPlanHelper.ShowDevice(Device, plan);
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			var zone = Device.Zones.FirstOrDefault();
			if (zone != null)
			{
				ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Publish(zone.UID);
			}
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showArchiveEventArgs = new ShowArchiveEventArgs()
			{
				GKDevice = Device
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showArchiveEventArgs);
		}

		public bool CanNotControl
		{
			get { return !(Device.Driver.IsControlDevice || (Device.Driver.IsDeviceOnShleif && !Device.Driver.IsControlDevice)) || !FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices); }
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
			State.StateChanged -= new Action(OnStateChanged);
		}
	}

	public class PlanViewModel : BaseViewModel
	{
		Plan Plan;
		GKDevice Device;
		public string Name
		{
			get { return Plan.Caption; }
		}
		public PlanViewModel(Plan plan, GKDevice device)
		{
			Plan = plan;
			Device = device;
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		private void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowDevice(Device, Plan);
		}
	}
}