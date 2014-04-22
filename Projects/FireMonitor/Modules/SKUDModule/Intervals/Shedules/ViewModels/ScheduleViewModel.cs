using Common;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.Intervals.Schedules.ViewModels
{
	public class ScheduleViewModel : BaseObjectViewModel<Schedule>, IEditingViewModel
	{
		public ScheduleViewModel(Schedule schedule)
			: base(schedule)
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			ScheduleZones = new SortableObservableCollection<ScheduleZoneViewModel>();
			foreach (var employeeScheduleZone in schedule.Zones)
			{
				var scheduleZoneViewModel = new ScheduleZoneViewModel(employeeScheduleZone);
				ScheduleZones.Add(scheduleZoneViewModel);
			}
		}

		public SortableObservableCollection<ScheduleZoneViewModel> ScheduleZones { get; private set; }

		private ScheduleZoneViewModel _selectedScheduleZone;
		public ScheduleZoneViewModel SelectedScheduleZone
		{
			get { return _selectedScheduleZone; }
			set
			{
				_selectedScheduleZone = value;
				OnPropertyChanged(() => SelectedScheduleZone);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var detailsViewModel = new ScheduleZoneDetailsViewModel(this.Model);
			if (DialogService.ShowModalWindow(detailsViewModel) && ScheduleZoneHelper.Save(detailsViewModel.ScheduleZone))
			{
				var scheduleZone = detailsViewModel.ScheduleZone;
				Model.Zones.Add(scheduleZone);
				var scheduleZoneViewModel = new ScheduleZoneViewModel(scheduleZone);
				ScheduleZones.Add(scheduleZoneViewModel);
				Sort();
				SelectedScheduleZone = scheduleZoneViewModel;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
			if (ScheduleZoneHelper.MarkDeleted(SelectedScheduleZone.Model))
			{
				Model.Zones.Remove(SelectedScheduleZone.Model);
				ScheduleZones.Remove(SelectedScheduleZone);
			}
		}
		private bool CanDelete()
		{
			return SelectedScheduleZone != null && ScheduleZones.Count > 1;
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var scheduleZoneDetailsViewModel = new ScheduleZoneDetailsViewModel(Model, SelectedScheduleZone.Model);
			if (DialogService.ShowModalWindow(scheduleZoneDetailsViewModel))
			{
				ScheduleZoneHelper.Save(SelectedScheduleZone.Model);
				var selectedTimeInterval = SelectedScheduleZone;
				Sort();
				SelectedScheduleZone = selectedTimeInterval;
			}
		}
		private bool CanEdit()
		{
			return SelectedScheduleZone != null;
		}

		private void Sort()
		{
			//ScheduleZones.Sort(item => item.IntervalTransitionType == IntervalTransitionType.NextDay ? item.BeginTime.Add(day) : item.BeginTime);
		}
	}
}