﻿using FiresecAPI.GK;
using FiresecAPI.Models;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using FiresecClient;
using System.Collections.Generic;
using System;

namespace GKModule.ViewModels
{
	public class DirectionViewModel : BaseViewModel
	{
		public GKDirection Direction { get; private set; }
		public GKState State
		{
			get { return Direction.State; }
		}

		public DirectionViewModel(GKDirection direction)
		{
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			ShowOnPlanOrPropertiesCommand = new RelayCommand(OnShowOnPlanOrProperties);
			Direction = direction;
			State.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => HasOnDelay);
			OnPropertyChanged(() => HasHoldDelay);
		}

		public RelayCommand ShowOnPlanOrPropertiesCommand { get; private set; }
		void OnShowOnPlanOrProperties()
		{
			if (CanShowOnPlan())
				ShowOnPlanHelper.ShowDirection(Direction);
			else
				DialogService.ShowWindow(new DirectionDetailsViewModel(Direction));
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowDirection(Direction);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowDirection(Direction);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (Direction != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { Direction.UID });
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new DirectionDetailsViewModel(Direction));
		}

		public bool HasOnDelay
		{
			get { return State.StateClasses.Contains(XStateClass.TurningOn) && State.OnDelay > 0; }
		}
		public bool HasHoldDelay
		{
			get { return State.StateClasses.Contains(XStateClass.On) && State.HoldDelay > 0; }
		}

		public string PresentationLogic
		{
			get
			{
				var presentationZone = GKManager.GetPresentationLogic(Direction.Logic);
				return presentationZone;
			}
		}
	}
}