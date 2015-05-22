﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
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
		bool CancelBackgroundWorker;
		public ObservableCollection<MeasureViewModel> Measures { get; private set; }

		public DeviceDetailsViewModel(GKDevice device)
		{
			ShowCommand = new RelayCommand(OnShow);
			ShowParentCommand = new RelayCommand(OnShowParent, CanShowParent);
			ShowOnPlanCommand = new RelayCommand<Plan>(OnShowOnPlan);
			ShowZoneCommand = new RelayCommand(OnShowZone);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			GetKauMeasuresCommand = new RelayCommand<List<object>>(OnGetKauMesures);

			Device = device;
			DeviceStateViewModel = new DeviceStateViewModel(State, device.Driver.IsAm);
			State.StateChanged += OnStateChanged;
			State.MeasureParametersChanged += OnMeasureParametersChanged;
			DeviceCommandsViewModel = new DeviceCommandsViewModel(Device);
			DevicePropertiesViewModel = new DevicePropertiesViewModel(Device);
			InitializePlans();

			Title = Device.PresentationName;
			StartMeasureParametersMonitoring();
		}

		public RelayCommand<List<object>> GetKauMeasuresCommand { get; private set; }
		public void OnGetKauMesures(List<object> objects)
		{
			Measures = new ObservableCollection<MeasureViewModel>();
			if (objects == null || objects.Count != 2)
			    return;
			var measuresResult = FiresecManager.FiresecService.GetCurrentConsumption(new CurrentConsumptionFilter
				{ 
					AlsUID = Device.UID, 
					StartDateTime = (DateTime)objects[0], 
					EndDateTime = (DateTime)objects[1]
				});
			if (measuresResult == null)
				return;
			if (measuresResult.HasError)
				MessageBoxService.Show(measuresResult.Error);
			else
			{
				foreach (var measure in measuresResult.Result)
				{
					var measureTime = measure.DateTime;
					var measureValue = measure.Current;
					Measures.Add(new MeasureViewModel(measureTime, measureValue));
				}
			}
			OnPropertyChanged(() => Measures);
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => DevicePicture);
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => DeviceStateViewModel);
			OnPropertyChanged(() => HasOnDelay);
			OnPropertyChanged(() => HasHoldDelay);
			OnPropertyChanged(() => HasOffDelay);
			OnPropertyChanged(() => HasRunTime);
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
			get { return State.StateClasses.Contains(XStateClass.On) && State.HoldDelay > 0; }
		}
		public bool HasOffDelay
		{
			get { return State.StateClasses.Contains(XStateClass.TurningOff) && State.OffDelay > 0; }
		}
		public bool HasRunTime
		{
			get { return Device.DriverType == GKDriverType.RSR2_MDU || Device.DriverType == GKDriverType.RSR2_MDU24; }
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
			get { return Device.Driver.MeasureParameters.Where(x => !x.IsDelay).Count() > 0 || Device.DriverType == GKDriverType.RSR2_Valve_DU || Device.DriverType == GKDriverType.RSR2_Valve_KV || Device.DriverType == GKDriverType.RSR2_Valve_KVMV; }
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
			BackgroundWorker.DoWork += UpdateMeasureParameters;
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
			MeasureParameters = new ObservableCollection<MeasureParameterViewModel>();
			foreach (var measureParameter in Device.State.XMeasureParameterValues)
			{
				var measureParameterViewModel = new MeasureParameterViewModel()
				{
					Name = measureParameter.Name,
					StringValue = measureParameter.StringValue
				};
				MeasureParameters.Add(measureParameterViewModel);
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
				ServiceFactory.Events.GetEvent<ShowGKZoneEvent>().Publish(zone.UID);
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
			State.StateChanged -= OnStateChanged;
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