using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using System.Collections.Generic;
using Infrustructure.Plans.Interfaces;
using Infrastructure.PlanLink.ViewModels;

namespace GKModule.ViewModels
{
	public class ZoneDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public GKZone Zone { get; private set; }
		public PlanLinksViewModel PlanLinks { get; private set; }
		public GKState State
		{
			get { return Zone.State; }
		}

		public ZoneDetailsViewModel(GKZone zone)
		{
			ShowCommand = new RelayCommand(OnShow);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ResetFireCommand = new RelayCommand(OnResetFire, CanResetFire);
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);

			Zone = zone;
			Title = Zone.PresentationName;
			State.StateChanged += new Action(OnStateChanged);
			PlanLinks = new PlanLinksViewModel(Zone);
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => ResetFireCommand);
			OnPropertyChanged(() => SetIgnoreCommand);
			OnPropertyChanged(() => ResetIgnoreCommand);
			CommandManager.InvalidateRequerySuggested();
		}

		public RelayCommand ResetFireCommand { get; private set; }
		void OnResetFire()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKReset(Zone);
			}
		}
		bool CanResetFire()
		{
			return State.StateClasses.Contains(XStateClass.Fire2) || State.StateClasses.Contains(XStateClass.Fire1) || State.StateClasses.Contains(XStateClass.Attention);
		}

		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKSetIgnoreRegime(Zone);
			}
		}
		bool CanSetIgnore()
		{
			return !State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Zone_Control);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKSetAutomaticRegime(Zone);
			}
		}
		bool CanResetIgnore()
		{
			return State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Zone_Control);
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowGKZoneEvent>().Publish(Zone.UID);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (Zone != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { Zone.UID });
		}

		public bool CanControl
		{
			get { return ClientManager.CheckPermission(PermissionType.Oper_Zone_Control); }
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Zone.UID.ToString(); }
		}
		#endregion

		public override void OnClosed()
		{
			State.StateChanged -= new Action(OnStateChanged);
		}
	}
}