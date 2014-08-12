using System;
using System.Linq;
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
		bool _isInitialized;
		public FiresecAPI.SKD.Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public NamedInterval NamedInterval { get; private set; }

		public NamedIntervalViewModel(FiresecAPI.SKD.Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			IsExpanded = true;
			_isInitialized = true;
		}
		public NamedIntervalViewModel(FiresecAPI.SKD.Organisation organisation, NamedInterval namedInterval)
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);

			Organisation = organisation;
			NamedInterval = namedInterval;
			IsOrganisation = false;

			_isInitialized = false;
		}

		public void Initialize()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				if (!IsOrganisation)
				{
					TimeIntervals = new SortableObservableCollection<TimeIntervalViewModel>();
					foreach (var timeInterval in NamedInterval.TimeIntervals)
					{
						var timeIntervalViewModel = new TimeIntervalViewModel(timeInterval);
						TimeIntervals.Add(timeIntervalViewModel);
					}
					SelectedTimeInterval = TimeIntervals.FirstOrDefault();
				}
			}
		}
		public void Update()
		{
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
		}

		public string Name
		{
			get { return IsOrganisation ? Organisation.Name : NamedInterval.Name; }
		}
		public string Description
		{
			get { return IsOrganisation ? Organisation.Description : NamedInterval.Description; }
		}

		public SortableObservableCollection<TimeIntervalViewModel> TimeIntervals { get; private set; }

		TimeIntervalViewModel _selectedTimeInterval;
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
		void OnAdd()
		{
			var timeIntervalDetailsViewModel = new TimeIntervalDetailsViewModel(NamedInterval);
			if (DialogService.ShowModalWindow(timeIntervalDetailsViewModel) && TimeIntervalHelper.Save(timeIntervalDetailsViewModel.TimeInterval))
			{
				var timeInterval = timeIntervalDetailsViewModel.TimeInterval;
				NamedInterval.TimeIntervals.Add(timeInterval);
				var timeIntervalViewModel = new TimeIntervalViewModel(timeInterval);
				TimeIntervals.Add(timeIntervalViewModel);
				TimeIntervals.Sort(item => item.BeginTime);
				SelectedTimeInterval = timeIntervalViewModel;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (TimeIntervalHelper.MarkDeleted(SelectedTimeInterval.TimeInterval))
			{
				NamedInterval.TimeIntervals.Remove(SelectedTimeInterval.TimeInterval);
				TimeIntervals.Remove(SelectedTimeInterval);
			}
		}
		bool CanDelete()
		{
			return SelectedTimeInterval != null && TimeIntervals.Count > 1;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var timeIntervalDetailsViewModel = new TimeIntervalDetailsViewModel(NamedInterval, SelectedTimeInterval.TimeInterval);
			if (DialogService.ShowModalWindow(timeIntervalDetailsViewModel))
			{
				TimeIntervalHelper.Save(SelectedTimeInterval.TimeInterval);
				SelectedTimeInterval.Update();
				var selectedTimeInterval = SelectedTimeInterval;
				TimeIntervals.Sort(item => item.BeginTime);
				SelectedTimeInterval = selectedTimeInterval;
			}
		}
		bool CanEdit()
		{
			return SelectedTimeInterval != null;
		}
	}
}