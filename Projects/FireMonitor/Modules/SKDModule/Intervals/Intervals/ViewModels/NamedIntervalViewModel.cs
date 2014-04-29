using System;
using Common;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class NamedIntervalViewModel : TreeNodeViewModel<NamedIntervalViewModel>, IEditingViewModel
	{
		public FiresecAPI.Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public NamedInterval NamedInterval { get; private set; }

		public NamedIntervalViewModel(FiresecAPI.Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
		}

		public NamedIntervalViewModel(FiresecAPI.Organisation organisation, NamedInterval namedInterval)
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);

			Organisation = organisation;
			NamedInterval = namedInterval;
			IsOrganisation = false;
			Name = namedInterval.Name;

			TimeIntervals = new SortableObservableCollection<TimeIntervalViewModel>();
			foreach (var timeInterval in namedInterval.TimeIntervals)
			{
				var timeIntervalViewModel = new TimeIntervalViewModel(timeInterval);
				TimeIntervals.Add(timeIntervalViewModel);
			}
		}

		public void Update(NamedInterval namedInterval)
		{
			Name = namedInterval.Name;
			//Description = namedInterval.Description;
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
		}

		public SortableObservableCollection<TimeIntervalViewModel> TimeIntervals { get; private set; }

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
			var timeIntervalDetailsViewModel = new TimeIntervalDetailsViewModel(NamedInterval);
			if (DialogService.ShowModalWindow(timeIntervalDetailsViewModel) && TimeIntervalHelper.Save(timeIntervalDetailsViewModel.TimeInterval))
			{
				var timeInterval = timeIntervalDetailsViewModel.TimeInterval;
				NamedInterval.TimeIntervals.Add(timeInterval);
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
				NamedInterval.TimeIntervals.Remove(SelectedTimeInterval.TimeInterval);
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
			var timeIntervalDetailsViewModel = new TimeIntervalDetailsViewModel(NamedInterval, SelectedTimeInterval.TimeInterval);
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