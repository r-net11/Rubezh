using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using SKDModule.Intervals.Common;
using SKDModule.Intervals.Common.ViewModels;
using Organisation = FiresecAPI.Organisation;

namespace SKDModule.Intervals.Schedules.ViewModels
{
	public class OrganisationSchedulesViewModel : OrganisationViewModel<ScheduleViewModel, Schedule>
	{
		private Schedule _clipboard;

		public OrganisationSchedulesViewModel(Organisation organisation)
			: base(organisation)
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
		}

		protected override ScheduleViewModel CreateViewModel(Schedule model)
		{
			return new ScheduleViewModel(model);
		}

		private void OnAdd()
		{
			var scheduleDetailsViewModel = new ScheduleDetailsViewModel(this);
			if (DialogService.ShowModalWindow(scheduleDetailsViewModel) && ScheduleHelper.Save(scheduleDetailsViewModel.Schedule))
			{
				var viewModel = new ScheduleViewModel(scheduleDetailsViewModel.Schedule);
				ViewModels.Add(viewModel);
				SelectedViewModel = viewModel;
			}
		}
		private void OnDelete()
		{
			if (ScheduleHelper.MarkDeleted(SelectedViewModel.Model))
				ViewModels.Remove(SelectedViewModel);
		}
		private bool CanDelete()
		{
			return SelectedViewModel != null;
		}
		private void OnEdit()
		{
			var detailsViewModel = new ScheduleDetailsViewModel(this, SelectedViewModel.Model);
			if (DialogService.ShowModalWindow(detailsViewModel))
			{
				ScheduleHelper.Save(SelectedViewModel.Model);
				SelectedViewModel.Update();
			}
		}
		private bool CanEdit()
		{
			return SelectedViewModel != null;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_clipboard = CopySchedule(SelectedViewModel.Model, false);
		}
		private bool CanCopy()
		{
			return SelectedViewModel != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newSchedule = CopySchedule(_clipboard);
			if (ScheduleHelper.Save(newSchedule))
			{
				var scheduleViewModel = new ScheduleViewModel(newSchedule);
				ViewModels.Add(scheduleViewModel);
				SelectedViewModel = scheduleViewModel;
			}
		}
		private bool CanPaste()
		{
			return _clipboard != null;
		}

		private Schedule CopySchedule(Schedule source, bool newName = true)
		{
			var copy = new Schedule();
			copy.Name = newName ? CopyHelper.CopyName(source.Name, ViewModels.Select(item => item.Model.Name)) : source.Name;
			copy.ScheduleSchemeUID = source.ScheduleSchemeUID;
			copy.IsIgnoreHoliday = source.IsIgnoreHoliday;
			copy.IsOnlyFirstEnter = source.IsOnlyFirstEnter;
			copy.OrganisationUID = source.OrganisationUID;
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