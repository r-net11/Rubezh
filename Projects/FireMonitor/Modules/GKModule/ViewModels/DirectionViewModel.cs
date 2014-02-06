using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DirectionViewModel : BaseViewModel
	{
		public XDirection Direction { get; private set; }
		public XState State
		{
			get { return Direction.State; }
		}

		public DirectionViewModel(XDirection direction)
		{
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			Direction = direction;
			State.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged("State");
			OnPropertyChanged("HasOnDelay");
			OnPropertyChanged("HasHoldDelay");
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
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				Direction = Direction
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
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
	}
}