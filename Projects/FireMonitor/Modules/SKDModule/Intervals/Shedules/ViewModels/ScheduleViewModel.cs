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
	public class ScheduleViewModel : TreeNodeViewModel<ScheduleViewModel>, IEditingViewModel
	{
		private bool _isInitialized;
		public FiresecAPI.SKD.Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public Schedule Schedule { get; private set; }

		public ScheduleViewModel(FiresecAPI.SKD.Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			IsExpanded = true;
			_isInitialized = true;
		}
		public ScheduleViewModel(FiresecAPI.SKD.Organisation organisation, Schedule schedule)
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);

			Organisation = organisation;
			Schedule = schedule;
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
				{
					ScheduleZones = new SortableObservableCollection<ScheduleZoneViewModel>();
					foreach (var employeeScheduleZone in Schedule.Zones)
					{
						var scheduleZoneViewModel = new ScheduleZoneViewModel(employeeScheduleZone);
						ScheduleZones.Add(scheduleZoneViewModel);
					}
					SelectedScheduleZone = ScheduleZones.FirstOrDefault();
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
			get { return IsOrganisation ? Organisation.Name : Schedule.Name; }
		}
		public string Description
		{
			get { return IsOrganisation ? Organisation.Description : string.Empty; }
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
			var scheduleZoneDetailsViewModel = new ScheduleZoneDetailsViewModel(Schedule);
			if (DialogService.ShowModalWindow(scheduleZoneDetailsViewModel) && ScheduleZoneHelper.Save(scheduleZoneDetailsViewModel.ScheduleZone))
			{
				var scheduleZone = scheduleZoneDetailsViewModel.ScheduleZone;
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
				SelectedScheduleZone.Update();
				Sort();
				SelectedScheduleZone = selectedTimeInterval;
			}
		}
		private bool CanEdit()
		{
			return SelectedScheduleZone != null;
		}
	}
}