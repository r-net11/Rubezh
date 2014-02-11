using System;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class TimeIntervalPartViewModel : BaseViewModel
	{
		public EmployeeTimeIntervalPart TimeIntervalPart { get; set; }

		public TimeIntervalPartViewModel(EmployeeTimeIntervalPart timeIntervalPart)
		{
			TimeIntervalPart = timeIntervalPart;
			AvailableTransitions = new ObservableCollection<IntervalTransitionType>(Enum.GetValues(typeof(IntervalTransitionType)).OfType<IntervalTransitionType>());
			SelectedTransition = timeIntervalPart.IntervalTransitionType;
		}

		public DateTime StartTime
		{
			get { return TimeIntervalPart.StartTime; }
		}

		public DateTime EndTime
		{
			get { return TimeIntervalPart.EndTime; }
		}

		ObservableCollection<IntervalTransitionType> _availableTransitions;
		public ObservableCollection<IntervalTransitionType> AvailableTransitions
		{
			get { return _availableTransitions; }
			set
			{
				_availableTransitions = value;
				OnPropertyChanged("AvailableTransitions");
			}
		}

		IntervalTransitionType _selectedTransition;
		public IntervalTransitionType SelectedTransition
		{
			get { return _selectedTransition; }
			set
			{
				_selectedTransition = value;
				OnPropertyChanged("SelectedTransition");
			}
		}


		public void Update()
		{
			OnPropertyChanged("TimeInterval");
			OnPropertyChanged("StartTime");
			OnPropertyChanged("EndTime");
		}
	}
}