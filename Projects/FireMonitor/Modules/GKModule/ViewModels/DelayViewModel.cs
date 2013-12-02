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
	public class DelayViewModel : BaseViewModel
	{
		public XDelayState DelayState { get; private set; }
		public XDelay Delay
		{
			get { return DelayState.Delay; }
		}

		public DelayViewModel(XDelayState delayState)
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			DelayState = delayState;
			DelayState.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged("DelayState");
			OnPropertyChanged("HasOnDelay");
			OnPropertyChanged("HasHoldDelay");
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				Delay = Delay
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
			DialogService.ShowWindow(new DelayDetailsViewModel(Delay));
        }

		public bool HasOnDelay
		{
			get { return DelayState.StateClasses.Contains(XStateClass.TurningOn) && DelayState.OnDelay > 0; }
		}
		public bool HasHoldDelay
		{
			get { return DelayState.StateClasses.Contains(XStateClass.On) && DelayState.HoldDelay > 0; }
		}
	}
}