using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Common;
using OrganisationFilter = FiresecAPI.SKD.OrganisationFilter;

namespace SKDModule.ViewModels
{
	public class SchedulesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		ScheduleFilter Filter;
		Schedule _clipboard;

		public SchedulesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			Filter = new ScheduleFilter() { OrganisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs };
			Initialize(Filter);
		}

		public void Initialize(ScheduleFilter filter)
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { UIDs = FiresecManager.CurrentUser.OrganisationUIDs });
			var schedules = ScheduleHelper.Get(filter);

			AllSchedules = new List<ScheduleViewModel>();
			Organisations = new List<ScheduleViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new ScheduleViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllSchedules.Add(organisationViewModel);
				foreach (var schedule in schedules)
				{
					if (schedule.OrganisationUID == organisation.UID)
					{
						var scheduleViewModel = new ScheduleViewModel(organisation, schedule);
						organisationViewModel.AddChild(scheduleViewModel);
						AllSchedules.Add(scheduleViewModel);
					}
				}
			}
			OnPropertyChanged("Organisations");
			SelectedSchedule = Organisations.FirstOrDefault();
		}

		public List<ScheduleViewModel> Organisations { get; private set; }
		List<ScheduleViewModel> AllSchedules { get; set; }

		public void Select(Guid scheduleUID)
		{
			if (scheduleUID != Guid.Empty)
			{
				var scheduleViewModel = AllSchedules.FirstOrDefault(x => x.Schedule != null && x.Schedule.UID == scheduleUID);
				if (scheduleViewModel != null)
					scheduleViewModel.ExpandToThis();
				SelectedSchedule = scheduleViewModel;
			}
		}

		ScheduleViewModel _selectedSchedule;
		public ScheduleViewModel SelectedSchedule
		{
			get { return _selectedSchedule; }
			set
			{
				_selectedSchedule = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedSchedule");
			}
		}

		public ScheduleViewModel ParentOrganisation
		{
			get
			{
				ScheduleViewModel OrganisationViewModel = SelectedSchedule;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedSchedule.Parent;

				if (OrganisationViewModel.Organisation != null)
					return OrganisationViewModel;

				return null;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel(SelectedSchedule.Organisation);
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel))
			{
				var scheduleViewModel = new ScheduleViewModel(SelectedSchedule.Organisation, scheduleDetailsViewModel.Schedule);

				ScheduleViewModel OrganisationViewModel = SelectedSchedule;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedSchedule.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
					return;

				OrganisationViewModel.AddChild(scheduleViewModel);
				SelectedSchedule = scheduleViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedSchedule != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			ScheduleViewModel OrganisationViewModel = SelectedSchedule;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedSchedule.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedSchedule);
			var Schedule = SelectedSchedule.Schedule;
			bool removeResult = ScheduleHelper.MarkDeleted(Schedule);
			if (!removeResult)
				return;
			OrganisationViewModel.RemoveChild(SelectedSchedule);
			index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
			if (index > -1)
				SelectedSchedule = OrganisationViewModel.Children.ToList()[index];
			else
				SelectedSchedule = OrganisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedSchedule != null && !SelectedSchedule.IsOrganisation;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel(SelectedSchedule.Organisation, SelectedSchedule.Schedule);
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel))
			{
				SelectedSchedule.Update(scheduleDetailsViewModel.Schedule);
			}
		}
		bool CanEdit()
		{
			return SelectedSchedule != null && SelectedSchedule.Parent != null && !SelectedSchedule.IsOrganisation;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_clipboard = CopySchedule(SelectedSchedule.Schedule, false);
		}
		private bool CanCopy()
		{
			return SelectedSchedule != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newInterval = CopySchedule(_clipboard);
			if (ScheduleHelper.Save(newInterval))
			{
				var scheduleViewModel = new ScheduleViewModel(SelectedSchedule.Organisation, newInterval);
				if (ParentOrganisation != null)
				{
					ParentOrganisation.AddChild(scheduleViewModel);
					AllSchedules.Add(scheduleViewModel);
				}
				SelectedSchedule = scheduleViewModel;
			}
		}
		private bool CanPaste()
		{
			return _clipboard != null;
		}

		private Schedule CopySchedule(Schedule source, bool newName = true)
		{
			var copy = new Schedule();
			copy.Name = newName ? CopyHelper.CopyName(source.Name, ParentOrganisation.Children.Select(item => item.Name)) : source.Name;
			copy.ScheduleSchemeUID = source.ScheduleSchemeUID;
			copy.IsIgnoreHoliday = source.IsIgnoreHoliday;
			copy.IsOnlyFirstEnter = source.IsOnlyFirstEnter;
			copy.OrganisationUID = ParentOrganisation.Organisation.UID;
			foreach (var scheduleZone in source.Zones)
				copy.Zones.Add(new ScheduleZone()
				{
					ScheduleUID = copy.UID,
					ZoneUID = scheduleZone.ZoneUID,
					IsControl = scheduleZone.IsControl,
				});
			return copy;
		}
	}
}