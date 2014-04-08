using System.Collections.ObjectModel;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class TimeIntervalViewModel : BaseViewModel
	{
		public NamedInterval TimeInterval { get; private set; }

		public TimeIntervalViewModel(NamedInterval timeInterval)
		{
			TimeInterval = timeInterval;
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			TimeIntervalParts = new ObservableCollection<TimeIntervalPartViewModel>();
			foreach (var timeIntervalPart in timeInterval.TimeIntervals)
			{
				var timeIntervalPartViewModel = new TimeIntervalPartViewModel(timeIntervalPart);
				TimeIntervalParts.Add(timeIntervalPartViewModel);
			}
		}

		public ObservableCollection<TimeIntervalPartViewModel> TimeIntervalParts { get; private set; }

		TimeIntervalPartViewModel _selectedTimeIntervalPart;
		public TimeIntervalPartViewModel SelectedTimeIntervalPart
		{
			get { return _selectedTimeIntervalPart; }
			set
			{
				_selectedTimeIntervalPart = value;
				OnPropertyChanged("SelectedTimeIntervalPart");
			}
		}

		public void Update()
		{
			OnPropertyChanged("TimeInterval");
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var timeIntervalPartDetailsViewModel = new TimeIntervalPartDetailsViewModel(TimeInterval);
			if (DialogService.ShowModalWindow(timeIntervalPartDetailsViewModel))
			{
				var timeIntervalPart = timeIntervalPartDetailsViewModel.TimeIntervalPart;
				TimeInterval.TimeIntervals.Add(timeIntervalPart);
				var timeIntervalPartViewModel = new TimeIntervalPartViewModel(timeIntervalPart);
				TimeIntervalParts.Add(timeIntervalPartViewModel);
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			TimeInterval.TimeIntervals.Remove(SelectedTimeIntervalPart.TimeIntervalPart);
			TimeIntervalParts.Remove(SelectedTimeIntervalPart);
		}
		bool CanRemove()
		{
			return SelectedTimeIntervalPart != null && !TimeInterval.IsDefault && TimeIntervalParts.Count > 1;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var timeIntervalPartDetailsViewModel = new TimeIntervalPartDetailsViewModel(TimeInterval, SelectedTimeIntervalPart.TimeIntervalPart);
			if (DialogService.ShowModalWindow(timeIntervalPartDetailsViewModel))
			{
				SelectedTimeIntervalPart.TimeIntervalPart = SelectedTimeIntervalPart.TimeIntervalPart;
				SelectedTimeIntervalPart.Update();
			}
		}
		bool CanEdit()
		{
			return SelectedTimeIntervalPart != null && !TimeInterval.IsDefault;
		}

		public bool IsEnabled
		{
			get { return !TimeInterval.IsDefault; }
		}
	}
}