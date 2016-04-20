using RubezhAPI.GK;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace GKModule.ViewModels
{
	public class PimViewModel : BaseViewModel
	{
		public GKPim Pim { get; private set; }
		public GKState State
		{
			get { return Pim.State; }
		}

		public PimViewModel(GKPim pim)
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			Pim = pim;
			State.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (Pim != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { Pim.UID });
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new PimDetailsViewModel(Pim));
		}
	}
}