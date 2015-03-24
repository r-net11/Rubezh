﻿using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using Infrastructure.Events;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class SKDZoneViewModel : BaseViewModel
	{
		public GKSKDZone SKDZone { get; private set; }
		public GKState State
		{
			get { return SKDZone.State; }
		}

		public bool IsOff
		{
			get { return StateClasses.Contains(XStateClass.Ignore); }
		}

		public SKDZoneViewModel(GKSKDZone skdZone)
		{
			OpenCommand = new RelayCommand(OnOpen);
			CloseCommand = new RelayCommand(OnClose);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			ShowOnPlanOrPropertiesCommand = new RelayCommand(OnShowOnPlanOrProperties);

			SKDZone = skdZone;
			State.StateChanged += OnStateChanged;
			OnStateChanged();

			EnterDoors = new ObservableCollection<GKDoor>();
			ExitDoors = new ObservableCollection<GKDoor>();
			foreach (var door in GKManager.Doors)
			{
				if (door.EnterZoneUID == skdZone.UID)
				{
					EnterDoors.Add(door);
				}
				if (door.ExitZoneUID == skdZone.UID)
				{
					ExitDoors.Add(door);
				}
			}
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

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => SKDZone);
			OnPropertyChanged(() => StateClasses);
			OnPropertyChanged(() => IsOff);
		}

		public ObservableCollection<GKDoor> EnterDoors { get; private set; }
		public ObservableCollection<GKDoor> ExitDoors { get; private set; }

		public bool HasEnterDoors
		{
			get { return EnterDoors.Count > 0; }
		}
		public bool HasExitDoors
		{
			get { return ExitDoors.Count > 0; }
		}

		public RelayCommand ShowOnPlanOrPropertiesCommand { get; private set; }
		void OnShowOnPlanOrProperties()
		{
			if (CanShowOnPlan())
				ShowOnPlanHelper.ShowGKSKDZone(SKDZone);
			else
				DialogService.ShowWindow(new SKDZoneDetailsViewModel(SKDZone));
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		public void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowGKSKDZone(SKDZone);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowGKSKDZone(SKDZone);
		}


		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKOpenSKDZone(SKDZone);
			}
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKCloseSKDZone(SKDZone);
			}
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showArchiveEventArgs = new ShowArchiveEventArgs()
			{
				GKSKDZone = SKDZone
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new SKDZoneDetailsViewModel(SKDZone));
		}
	}
}