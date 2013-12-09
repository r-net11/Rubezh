using System.Collections.Generic;
using System.Text;
using System.Linq;
using GKProcessor;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Events;
using Infrastructure;

namespace GKModule.ViewModels
{
	public class PimViewModel : BaseViewModel
	{
		public XState State { get; private set; }
		public XPim Pim
		{
			get { return State.Pim; }
		}

		public PimViewModel(XState state)
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			State = state;
			State.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged("State");
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				Pim = Pim
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
			DialogService.ShowWindow(new PimDetailsViewModel(Pim));
        }
	}
}