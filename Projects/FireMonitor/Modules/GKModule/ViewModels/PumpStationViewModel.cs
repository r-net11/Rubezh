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
	public class PumpStationViewModel : BaseViewModel
	{
		public XPumpStationState PumpStationState { get; private set; }
		public XPumpStation PumpStation
		{
			get { return PumpStationState.PumpStation; }
		}

		public PumpStationViewModel(XPumpStationState pumpStationState)
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			PumpStationState = pumpStationState;
			PumpStationState.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged("PumpStationState");
			OnPropertyChanged("HasOnDelay");
			OnPropertyChanged("HasHoldDelay");
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				PumpStation = PumpStation
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
			DialogService.ShowWindow(new PumpStationDetailsViewModel(PumpStation));
        }

		public bool HasOnDelay
		{
			get { return PumpStationState.StateClasses.Contains(XStateClass.TurningOn) && PumpStationState.OnDelay > 0; }
		}
		public bool HasHoldDelay
		{
			get { return PumpStationState.StateClasses.Contains(XStateClass.On) && PumpStationState.HoldDelay > 0; }
		}
	}
}