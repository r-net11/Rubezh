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

namespace SKDModule.ViewModels
{
	public class SchedulesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		private bool _isInitialized;
		private Schedule _clipboard;

		public SchedulesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			_isInitialized = false;
		}

		public void Initialize()
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			if (organisations == null)
				return;
			var filter = new ScheduleFilter()
			{
				UserUID = FiresecManager.CurrentUser.UID,
				OrganisationUIDs = organisations.Select(item => item.UID).ToList(),
			};
			var schedules = ScheduleHelper.Get(filter);
			if (schedules == null)
				return;

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
			OnPropertyChanged(() => Organisations);
			SelectedSchedule = Organisations.FirstOrDefault();
		}
		public override void OnShow()
		{
			base.OnShow();
			if (!_isInitialized)
			{
				Initialize();
				_isInitialized = true;
			}
		}

		public List<ScheduleViewModel> Organisations { get; private set; }
		private List<ScheduleViewModel> AllSchedules { get; set; }

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

		private ScheduleViewModel _selectedSchedule;
		public ScheduleViewModel SelectedSchedule
		{
			get { return _selectedSchedule; }
			set
			{
				_selectedSchedule = value;
				if (value != null)
				{
					value.ExpandToThis();
					value.Initialize();
				}
				OnPropertyChanged(() => SelectedSchedule);
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
		private void OnAdd()
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
		private bool CanAdd()
		{
			return SelectedSchedule != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
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
		private bool CanRemove()
		{
			return SelectedSchedule != null && !SelectedSchedule.IsOrganisation;
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel(SelectedSchedule.Organisation, SelectedSchedule.Schedule);
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel))
				SelectedSchedule.Update();
		}
		private bool CanEdit()
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
			return SelectedSchedule != null && !SelectedSchedule.IsOrganisation;
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
			return SelectedSchedule != null && _clipboard != null;
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