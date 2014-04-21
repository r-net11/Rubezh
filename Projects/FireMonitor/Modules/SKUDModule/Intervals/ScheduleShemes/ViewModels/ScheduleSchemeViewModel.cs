using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common;
using Common;
using FiresecClient.SKDHelpers;

namespace SKDModule.Intervals.ScheduleShemes.ViewModels
{
	public class ScheduleSchemeViewModel : BaseObjectViewModel<ScheduleScheme>
	{
		public SortableObservableCollection<DayIntervalViewModel> DayIntervals { get; private set; }
		public OrganisationScheduleSchemasViewModel OrganizationViewModel { get; private set; }

		public ScheduleSchemeViewModel(OrganisationScheduleSchemasViewModel organizationViewModel, ScheduleScheme scheduleScheme)
			: base(scheduleScheme)
		{
			OrganizationViewModel = organizationViewModel;
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			DayIntervals = new SortableObservableCollection<DayIntervalViewModel>(scheduleScheme.DayIntervals.Select(item => new DayIntervalViewModel(this, item)));
		}

		public bool IsSlide
		{
			get { return Model.Type == ScheduleSchemeType.SlideDay; }
		}

		private DayIntervalViewModel _selectedDayInterval;
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
		private void OnAdd()
		{
			var dayInterval = new DayInterval()
			{
				Number = Model.DayIntervals.Count,
				ScheduleSchemeUID = Model.UID,
				NamedIntervalUID = Guid.Empty,
			};
			if (DayIntervalHelper.Save(dayInterval))
			{
				Model.DayIntervals.Add(dayInterval);
				var viewModel = new DayIntervalViewModel(this, dayInterval);
				DayIntervals.Add(viewModel);
				Sort();
				SelectedDayInterval = viewModel;
			}
		}
		private bool CanAdd()
		{
			return IsSlide;
		}

		public RelayCommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
			var number = SelectedDayInterval.Model.Number;
			if (DayIntervalHelper.MarkDeleted(SelectedDayInterval.Model))
			{
				for (int i = number + 1; i < Model.DayIntervals.Count; i++)
					Model.DayIntervals[i].Number--;
				Model.DayIntervals.Remove(SelectedDayInterval.Model);
				DayIntervals.Remove(SelectedDayInterval);
				DayIntervals.ForEach(item => item.Update());
				SelectedDayInterval = number < DayIntervals.Count ? DayIntervals[number] : DayIntervals.LastOrDefault();
			}
		}
		private bool CanDelete()
		{
			return IsSlide && SelectedDayInterval != null && DayIntervals.Count > 1;
		}

		private void Sort()
		{
			DayIntervals.Sort(item => item.Model.Number);
		}
	}
}
