using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common;
using Common;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.TreeList;

namespace SKDModule.Intervals.ScheduleShemes.ViewModels
{
	public class ScheduleSchemeViewModel : TreeNodeViewModel<ScheduleSchemeViewModel>
	{
		public ScheduleSchemesViewModel ScheduleSchemesViewModel;
		public FiresecAPI.Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public ScheduleScheme ScheduleScheme { get; private set; }

		void Initialize()
		{
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			DayIntervals = new SortableObservableCollection<DayIntervalViewModel>();
		}

		public ScheduleSchemeViewModel(ScheduleSchemesViewModel scheduleSchemesViewModel, FiresecAPI.Organisation organisation)
		{
			Initialize();
			ScheduleSchemesViewModel = scheduleSchemesViewModel;
			Organisation = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
		}

		public ScheduleSchemeViewModel(ScheduleSchemesViewModel scheduleSchemesViewModel, FiresecAPI.Organisation organisation, ScheduleScheme scheduleScheme)
		{
			Initialize();
			ScheduleSchemesViewModel = scheduleSchemesViewModel;
			Organisation = organisation;
			ScheduleScheme = scheduleScheme;
			IsOrganisation = false;
			Name = scheduleScheme.Name;
			Description = scheduleScheme.Description;

			DayIntervals = new SortableObservableCollection<DayIntervalViewModel>(scheduleScheme.DayIntervals.Select(item => new DayIntervalViewModel(this, item)));
		}

		public void Update(ScheduleScheme scheduleScheme)
		{
			Name = scheduleScheme.Name;
			Description = scheduleScheme.Description;
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
		}

		public SortableObservableCollection<DayIntervalViewModel> DayIntervals { get; private set; }

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