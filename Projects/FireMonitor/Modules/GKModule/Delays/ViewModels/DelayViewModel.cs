﻿using FiresecAPI.GK;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace GKModule.ViewModels
{
	public class DelayViewModel : BaseViewModel
	{
		public GKDelay Delay { get; private set; }
		public GKState State
		{
			get { return Delay.State; }
		}

		public DelayViewModel(GKDelay delay)
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			Delay = delay;
			State.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => HasOnDelay);
			OnPropertyChanged(() => HasHoldDelay);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (Delay != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { Delay.UID });
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new DelayDetailsViewModel(Delay));
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
				var presentationZone = GKManager.GetPresentationLogic(Delay.Logic);
				return presentationZone;
			}
		}
	}
}