using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class PimViewModel : BaseViewModel
	{
		public XPim Pim { get; private set; }
		public XState State
		{
			get { return Pim.State; }
		}

		public PimViewModel(XPim pim)
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			Pim = pim;
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