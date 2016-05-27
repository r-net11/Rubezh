using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using StrazhAPI.GK;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using StrazhAPI.Plans.Elements;
using StrazhModule.Doors;

namespace StrazhModule.ViewModels
{
	public class DoorDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		private SKDDevice _lock;

		public SKDDoor Door { get; private set; }
		public SKDDoorState State
		{
			get { return Door.State; }
		}

		public DoorDetailsViewModel(SKDDoor door)
		{
			ClearPromptWarningCommand = new RelayCommand(OnClearPromptWarning, CanClearPromptWarning);
			ShowCommand = new RelayCommand(OnShow);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			OpenCommand = new RelayCommand(OnOpen, CanOpen);
			CloseCommand = new RelayCommand(OnClose, CanClose);
			DoorAccessStateNormalCommand = new RelayCommand(OnDoorAccessStateNormal, CanDoorAccessStateNormal);
			DoorAccessStateCloseAlwaysCommand = new RelayCommand(OnDoorAccessStateCloseAlways, CanDoorAccessStateCloseAlways);
			DoorAccessStateOpenAlwaysCommand = new RelayCommand(OnDoorAccessStateOpenAlways, CanDoorAccessStateOpenAlways);

			Door = door;
			Title = Door.PresentationName;
			State.StateChanged -= new Action(OnStateChanged);
			State.StateChanged += new Action(OnStateChanged);
			InitializePlans();

			InitializeLock();
			if (_lock != null)
			{
				_lock.State.StateChanged -= new Action(OnLockStateChanged);
				_lock.State.StateChanged += new Action(OnLockStateChanged);
			}
		}

		void InitializeLock()
		{
			if (Door.InDevice != null)
			{
				var lockAddress = Door.InDevice.IntAddress;
				if (Door.DoorType == DoorType.TwoWay)
				{
					lockAddress = Door.InDevice.IntAddress / 2;
				}
				 _lock = Door.InDevice.Parent.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == lockAddress);
			}
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => IsPromptWarning);
			CommandManager.InvalidateRequerySuggested();
		}

		void OnLockStateChanged()
		{
			State.AccessState = _lock.State.AccessState;
			OnStateChanged();
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

		public bool IsPromptWarning
		{
			get
			{
				//return State.StateClass == XStateClass.Attention;
				return true;
			}
		}

		public RelayCommand ClearPromptWarningCommand { get; private set; }
		private void OnClearPromptWarning()
		{
			DoorCommander.ClearPromptWarning(Door);
		}
		private bool CanClearPromptWarning()
		{
			return DoorCommander.CanClearPromptWarning(Door);
		}


		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			DoorCommander.Open(Door);
		}
		bool CanOpen()
		{
			return DoorCommander.CanOpen(Door);
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			DoorCommander.Close(Door);
		}
		bool CanClose()
		{
			return DoorCommander.CanClose(Door);
		}

		public RelayCommand DoorAccessStateNormalCommand { get; private set; }
		void OnDoorAccessStateNormal()
		{
			DoorCommander.SetAccessStateToNormal(Door);
		}
		bool CanDoorAccessStateNormal()
		{
			return DoorCommander.CanSetAccessStateToNormal(Door);
		}

		public RelayCommand DoorAccessStateCloseAlwaysCommand { get; private set; }
		void OnDoorAccessStateCloseAlways()
		{
			DoorCommander.SetAccessStateToCloseAlways(Door);
		}
		bool CanDoorAccessStateCloseAlways()
		{
			return DoorCommander.CanSetAccessStateToCloseAlways(Door);
		}

		public RelayCommand DoorAccessStateOpenAlwaysCommand { get; private set; }
		void OnDoorAccessStateOpenAlways()
		{
			DoorCommander.SetAccessStateToOpenAlways(Door);
		}
		bool CanDoorAccessStateOpenAlways()
		{
			return DoorCommander.CanSetAccessStateToOpenAlways(Door);
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowSKDDoorEvent>().Publish(Door.UID);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowArchiveEventArgs()
			{
				SKDDoor = Door
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showSKDArchiveEventArgs);
		}

		public bool CanControl
		{
			get { return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control); }
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
			if (_lock != null)
				_lock.State.StateChanged -= new Action(OnLockStateChanged);
		}
	}
}