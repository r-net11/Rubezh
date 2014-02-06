 using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using DeviceControls;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;
using FiresecAPI.Models;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Elements;
using System.Windows.Input;
using FiresecAPI;

namespace SKDModule.ViewModels
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

			Device = device;
			DeviceStateViewModel = new DeviceStateViewModel(State);
			State.StateChanged += new Action(OnStateChanged);
			DeviceCommandsViewModel = new DeviceCommandsViewModel(Device);
			DevicePropertiesViewModel = new DevicePropertiesViewModel(Device);
			InitializePlans();

			Title = Device.PresentationName;
			TopMost = true;
		}

		void OnStateChanged()
		{
			OnPropertyChanged("DevicePicture");
			OnPropertyChanged("State");
			OnPropertyChanged("DeviceStateViewModel");
			CommandManager.InvalidateRequerySuggested();
		}

		public Brush DevicePicture
		{
			get { return PictureCacheSource.SKDDevicePicture.GetDynamicSKDBrush(Device); }
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
				var planes = FiresecManager.PlansConfiguration.AllPlans.Where(item => item.ElementXDevices.Any(element => element.XDeviceUID == Device.UID));
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
			//var zone = Device.Zones.FirstOrDefault();
			//if (zone != null)
			//{
			//    ServiceFactory.Events.GetEvent<ShowSKDZoneEvent>().Publish(zone.UID);
			//}
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowSKDArchiveEventArgs()
			{
				Device = Device
			};
			ServiceFactory.Events.GetEvent<ShowSKDArchiveEvent>().Publish(showSKDArchiveEventArgs);
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
			ShowOnPlanHelper.ShowDevice(Device, Plan);
		}
	}
}