using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using DeviceControls;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using StrazhModule.Devices;

namespace StrazhModule.ViewModels
{
	public class DeviceDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public SKDDevice Device { get; private set; }
		public SKDDeviceState State
		{
			get { return Device.State; }
		}
		public DeviceStateViewModel DeviceStateViewModel { get; private set; }
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }
		public DevicePropertiesViewModel DevicePropertiesViewModel { get; private set; }

		public DeviceDetailsViewModel(SKDDevice device)
		{
			ShowCommand = new RelayCommand(OnShow);
			ShowParentCommand = new RelayCommand(OnShowParent, CanShowParent);
			ShowOnPlanCommand = new RelayCommand<Plan>(OnShowOnPlan);
			ShowZoneCommand = new RelayCommand(OnShowZone);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ClearPromptWarningCommand = new RelayCommand(OnClearPromptWarning, CanClearPromptWarning);

			Device = device;
			Title = Device.Name;
			DeviceStateViewModel = new DeviceStateViewModel(State);
			State.StateChanged -= new Action(OnStateChanged);
			State.StateChanged += new Action(OnStateChanged);
			DeviceCommandsViewModel = new DeviceCommandsViewModel(Device);
			DevicePropertiesViewModel = new DevicePropertiesViewModel(Device);
			InitializePlans();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => DevicePicture);
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => DeviceStateViewModel);
			CommandManager.InvalidateRequerySuggested();
		}

		public Brush DevicePicture
		{
			get { return PictureCacheSource.SKDDevicePicture.GetDynamicBrush(Device); }
		}

		public string PresentationZone
		{
			get { return SKDManager.GetPresentationZone(Device); }
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowSKDDeviceEvent>().Publish(Device.UID);
		}

		public RelayCommand ShowParentCommand { get; private set; }
		void OnShowParent()
		{
			ServiceFactory.Events.GetEvent<ShowSKDDeviceEvent>().Publish(Device.Parent.UID);
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
				ElementBase elementBase = plan.ElementSKDDevices.FirstOrDefault(x => x.DeviceUID == Device.UID);
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
				var planes = FiresecManager.PlansConfiguration.AllPlans.Where(item => item.ElementSKDDevices.Any(element => element.DeviceUID == Device.UID));
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
			ShowOnPlanHelper.ShowSKDDevice(Device, plan);
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			if (Device.Zone != null)
			{
				ServiceFactory.Events.GetEvent<ShowSKDZoneEvent>().Publish(Device.Zone.UID);
			}
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowArchiveEventArgs()
			{
				SKDDevice = Device
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showSKDArchiveEventArgs);
		}

		public bool CanShowClearPromptWarning
		{
			get
			{
				//return StateClasses.Any(x => x.StateClass == XStateClass.Attention);
				return Device.DriverType == SKDDriverType.Lock;
			}
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

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Device.UID.ToString(); }
		}
		#endregion

		public override void OnClosed()
		{
			State.StateChanged -= new Action(OnStateChanged);
		}
	}

	public class PlanViewModel : BaseViewModel
	{
		Plan Plan;
		SKDDevice Device;
		public string Name
		{
			get { return Plan.Caption; }
		}
		public PlanViewModel(Plan plan, SKDDevice device)
		{
			Plan = plan;
			Device = device;
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowSKDDevice(Device, Plan);
		}
	}
}