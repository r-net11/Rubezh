using System;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class ScheduleSchemeViewModel : TreeNodeViewModel<ScheduleSchemeViewModel>
	{
		private bool _isInitialized;
		public ScheduleSchemesViewModel ScheduleSchemesViewModel;
		public FiresecAPI.SKD.Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public ScheduleScheme ScheduleScheme { get; private set; }

		public ScheduleSchemeViewModel(ScheduleSchemesViewModel scheduleSchemesViewModel, FiresecAPI.SKD.Organisation organisation)
		{
			ScheduleSchemesViewModel = scheduleSchemesViewModel;
			Organisation = organisation;
			IsOrganisation = true;
			IsExpanded = true;
			_isInitialized = true;
		}
		public ScheduleSchemeViewModel(ScheduleSchemesViewModel scheduleSchemesViewModel, FiresecAPI.SKD.Organisation organisation, ScheduleScheme scheduleScheme)
		{
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);

			ScheduleSchemesViewModel = scheduleSchemesViewModel;
			Organisation = organisation;
			ScheduleScheme = scheduleScheme;
			IsOrganisation = false;
			Update();

			_isInitialized = false;
		}

		public void Initialize()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				if (!IsOrganisation)
					DayIntervals = new SortableObservableCollection<DayIntervalViewModel>(ScheduleScheme.DayIntervals.Select(item => new DayIntervalViewModel(this, item)));
			}
			OnPropertyChanged(() => NamedIntervals);
		}
		public void Update()
		{
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
		}
		public string Name
		{
			get { return IsOrganisation ? Organisation.Name : ScheduleScheme.Name; }
		}
		public string Description
		{
			get { return IsOrganisation ? Organisation.Description : ScheduleScheme.Description; }
		}

		public SortableObservableCollection<DayIntervalViewModel> DayIntervals { get; private set; }
		public ObservableCollection<NamedInterval> NamedIntervals
		{
			get { return ScheduleSchemesViewModel.GetNamedIntervals(Organisation.UID); }
		}

		public bool IsSlide
		{
			get { return ScheduleScheme != null && ScheduleScheme.Type == ScheduleSchemeType.SlideDay; }
		}

		DayIntervalViewModel _selectedDayInterval;
		public DayIntervalViewModel SelectedDayInterval
		{
			get { return _selectedDayInterval; }
			set
			{
				_selectedDayInterval = value;
				OnPropertyChanged(() => SelectedDayInterval);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var dayInterval = new DayInterval()
			{
				Number = ScheduleScheme.DayIntervals.Count,
				ScheduleSchemeUID = ScheduleScheme.UID,
				NamedIntervalUID = Guid.Empty,
			};
			if (DayIntervalHelper.Save(dayInterval))
			{
				ScheduleScheme.DayIntervals.Add(dayInterval);
				var viewModel = new DayIntervalViewModel(this, dayInterval);
				DayIntervals.Add(viewModel);
				Sort();
				SelectedDayInterval = viewModel;
			}
		}
		bool CanAdd()
		{
			return IsSlide;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var number = SelectedDayInterval.Model.Number;
			if (DayIntervalHelper.MarkDeleted(SelectedDayInterval.Model))
			{
				for (int i = number + 1; i < ScheduleScheme.DayIntervals.Count; i++)
					ScheduleScheme.DayIntervals[i].Number--;
				ScheduleScheme.DayIntervals.Remove(SelectedDayInterval.Model);
				DayIntervals.Remove(SelectedDayInterval);
				DayIntervals.ForEach(item => item.Update());
				SelectedDayInterval = number < DayIntervals.Count ? DayIntervals[number] : DayIntervals.LastOrDefault();
			}
		}
		bool CanDelete()
		{
			return IsSlide && SelectedDayInterval != null && DayIntervals.Count > 1;
		}

		void Sort()
		{
			DayIntervals.Sort(item => item.Model.Number);
		}
	}
}