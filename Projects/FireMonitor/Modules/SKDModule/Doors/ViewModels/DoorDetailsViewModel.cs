using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using SKDModule.Events;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class DoorDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public Door Door { get; private set; }
		public DoorState State
		{
			get { return Door.State; }
		}

		public DoorDetailsViewModel(Door door)
		{
			ShowCommand = new RelayCommand(OnShow);
			OpenCommand = new RelayCommand(OnOpen, CanOpen);
			CloseCommand = new RelayCommand(OnClose, CanClose);

			Door = door;
			State.StateChanged += new Action(OnStateChanged);
			InitializePlans();

			Title = Door.Name;
			TopMost = true;
		}

		void OnStateChanged()
		{
			OnPropertyChanged("State");
			OnPropertyChanged("ResetFireCommand");
			OnPropertyChanged("SetIgnoreCommand");
			OnPropertyChanged("ResetIgnoreCommand");
			CommandManager.InvalidateRequerySuggested();
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
				ElementBase elementBase;
				elementBase = plan.ElementDoors.FirstOrDefault(x => x.DoorUID == Door.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.Door = Door;
					Plans.Add(alarmPlanViewModel);
					continue;
				}
			}
		}

		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var device = SKDManager.Devices.FirstOrDefault(x => x.UID == Door.InDeviceUID);
				if (device != null)
				{
					var result = FiresecManager.FiresecService.SKDOpenDevice(device);
					if (result.HasError)
					{
						MessageBoxService.ShowWarning(result.Error);
					}
				}
			}
		}
		bool CanOpen()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var device = SKDManager.Devices.FirstOrDefault(x => x.UID == Door.InDeviceUID);
				if (device != null)
				{
					var result = FiresecManager.FiresecService.SKDCloseDevice(device);
					if (result.HasError)
					{
						MessageBoxService.ShowWarning(result.Error);
					}
				}
			}
		}
		bool CanClose()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowDoorEvent>().Publish(Door.UID);
		}

		public bool CanControl
		{
			get { return FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices); }
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Door.UID.ToString(); }
		}
		#endregion

		public override void OnClosed()
		{
			State.StateChanged -= new Action(OnStateChanged);
		}
	}
}