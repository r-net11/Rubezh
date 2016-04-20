using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.PlanLink.ViewModels;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace GKModule.ViewModels
{
	public class SKDZoneDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public GKSKDZone Zone { get; private set; }
		public PlanLinksViewModel PlanLinks { get; private set; }
		public GKState State
		{
			get { return Zone.State; }
		}
		public SKDZoneDetailsViewModel(GKSKDZone zone)
		{
			ShowCommand = new RelayCommand(OnShow);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			OpenCommand = new RelayCommand(OnOpen);
			CloseCommand = new RelayCommand(OnClose);

			Zone = zone;
			Title = Zone.PresentationName;
			State.StateChanged += new Action(OnStateChanged);
			PlanLinks = new PlanLinksViewModel(Zone);
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => StateClasses);
			OnPropertyChanged(() => State);
			CommandManager.InvalidateRequerySuggested();
		}

		public List<XStateClass> StateClasses
		{
			get
			{
				var stateClasses = State.StateClasses.ToList();
				stateClasses.Sort();
				return stateClasses;
			}
		}

		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKOpenSKDZone(Zone);
			}
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKCloseSKDZone(Zone);
			}
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowGKSKDZoneEvent>().Publish(Zone.UID);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (Zone != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { Zone.UID });
		}
		public bool CanControl
		{
			get { return ClientManager.CheckPermission(PermissionType.Oper_ZonesSKD); }
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