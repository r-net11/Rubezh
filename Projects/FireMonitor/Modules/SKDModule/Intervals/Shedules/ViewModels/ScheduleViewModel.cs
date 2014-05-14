using Common;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ScheduleViewModel : TreeNodeViewModel<ScheduleViewModel>, IEditingViewModel
	{
		public FiresecAPI.SKD.Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public Schedule Schedule { get; private set; }

		public ScheduleViewModel(FiresecAPI.SKD.Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
		}

		public ScheduleViewModel(FiresecAPI.SKD.Organisation organisation, Schedule schedule)
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);

			Organisation = organisation;
			Schedule = schedule;
			IsOrganisation = false;
			Name = schedule.Name;

			ScheduleZones = new SortableObservableCollection<ScheduleZoneViewModel>();
			foreach (var employeeScheduleZone in schedule.Zones)
			{
				var scheduleZoneViewModel = new ScheduleZoneViewModel(employeeScheduleZone);
				ScheduleZones.Add(scheduleZoneViewModel);
			}
		}

		public void Update(Schedule schedule)
		{
			Name = schedule.Name;
			//Description = schedule.Description;
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
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
			var detailsViewModel = new ScheduleZoneDetailsViewModel(Schedule);
			if (DialogService.ShowModalWindow(detailsViewModel) && ScheduleZoneHelper.Save(detailsViewModel.ScheduleZone))
			{
				var scheduleZone = detailsViewModel.ScheduleZone;
				Schedule.Zones.Add(scheduleZone);
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
				Schedule.Zones.Remove(SelectedScheduleZone.Model);
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
			var scheduleZoneDetailsViewModel = new ScheduleZoneDetailsViewModel(Schedule, SelectedScheduleZone.Model);
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