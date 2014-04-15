using System.Collections.ObjectModel;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class NamedIntervalViewModel : BaseViewModel
	{
		public NamedInterval NamedInterval { get; private set; }
		public ObservableCollection<TimeIntervalViewModel> TimeIntervals { get; private set; }

		public NamedIntervalViewModel(NamedInterval namedInterval)
		{
			NamedInterval = namedInterval;
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			TimeIntervals = new ObservableCollection<TimeIntervalViewModel>();
			foreach (var timeInterval in namedInterval.TimeIntervals)
			{
				var timeIntervalViewModel = new TimeIntervalViewModel(timeInterval);
				TimeIntervals.Add(timeIntervalViewModel);
			}
		}

		private TimeIntervalViewModel _selectedTimeInterval;
		public TimeIntervalViewModel SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged(() => SelectedTimeInterval);
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => NamedInterval);
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var timeIntervalDetailsViewModel = new TimeIntervalDetailsViewModel(NamedInterval);
			if (DialogService.ShowModalWindow(timeIntervalDetailsViewModel))
			{
				// save
				var timeInterval = timeIntervalDetailsViewModel.TimeInterval;
				NamedInterval.TimeIntervals.Add(timeInterval);
				var timeIntervalViewModel = new TimeIntervalViewModel(timeInterval);
				TimeIntervals.Add(timeIntervalViewModel);
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			// save
			NamedInterval.TimeIntervals.Remove(SelectedTimeInterval.TimeInterval);
			TimeIntervals.Remove(SelectedTimeInterval);
		}
		private bool CanRemove()
		{
			return SelectedTimeInterval != null && !NamedInterval.IsDefault && TimeIntervals.Count > 1;
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var timeIntervalDetailsViewModel = new TimeIntervalDetailsViewModel(NamedInterval, SelectedTimeInterval.TimeInterval);
			if (DialogService.ShowModalWindow(timeIntervalDetailsViewModel))
			{
				// save
				SelectedTimeInterval.Update();
			}
		}
		private bool CanEdit()
		{
			return SelectedTimeInterval != null && !NamedInterval.IsDefault;
		}

		public bool IsEnabled
		{
			get { return !NamedInterval.IsDefault; }
		}
	}
}