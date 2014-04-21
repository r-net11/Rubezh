using System.Collections.ObjectModel;
using Common;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace SKDModule.ViewModels
{
	public class NamedIntervalViewModel : BaseObjectViewModel<NamedInterval>, IEditingViewModel
	{
		public SortableObservableCollection<TimeIntervalViewModel> TimeIntervals { get; private set; }

		public NamedIntervalViewModel(NamedInterval namedInterval)
			: base(namedInterval)
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			TimeIntervals = new SortableObservableCollection<TimeIntervalViewModel>();
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

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var timeIntervalDetailsViewModel = new TimeIntervalDetailsViewModel(Model);
			if (DialogService.ShowModalWindow(timeIntervalDetailsViewModel) && TimeIntervalHelper.Save(timeIntervalDetailsViewModel.TimeInterval))
			{
				var timeInterval = timeIntervalDetailsViewModel.TimeInterval;
				Model.TimeIntervals.Add(timeInterval);
				var timeIntervalViewModel = new TimeIntervalViewModel(timeInterval);
				TimeIntervals.Add(timeIntervalViewModel);
				Sort();
				SelectedTimeInterval = timeIntervalViewModel;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
			if (TimeIntervalHelper.MarkDeleted(SelectedTimeInterval.TimeInterval))
			{
				Model.TimeIntervals.Remove(SelectedTimeInterval.TimeInterval);
				TimeIntervals.Remove(SelectedTimeInterval);
			}
		}
		private bool CanDelete()
		{
			return SelectedTimeInterval != null && TimeIntervals.Count > 1;
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var timeIntervalDetailsViewModel = new TimeIntervalDetailsViewModel(Model, SelectedTimeInterval.TimeInterval);
			if (DialogService.ShowModalWindow(timeIntervalDetailsViewModel))
			{
				TimeIntervalHelper.Save(SelectedTimeInterval.TimeInterval);
				SelectedTimeInterval.Update();
				var selectedTimeInterval = SelectedTimeInterval;
				Sort();
				SelectedTimeInterval = selectedTimeInterval;
			}
		}
		private bool CanEdit()
		{
			return SelectedTimeInterval != null;
		}

		private void Sort()
		{
			var day = TimeSpan.FromDays(1);
			TimeIntervals.Sort(item => item.IntervalTransitionType == IntervalTransitionType.NextDay ? item.BeginTime.Add(day) : item.BeginTime);
		}
	}
}