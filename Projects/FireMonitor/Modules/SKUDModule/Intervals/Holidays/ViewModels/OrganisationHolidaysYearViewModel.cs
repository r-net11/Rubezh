using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using SKDModule.Intervals.Common.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationHolidaysYearViewModel : OrganisationViewModel<HolidayViewModel, Holiday>
	{
		public int Year { get; private set; }

		public OrganisationHolidaysYearViewModel(int year, FiresecAPI.Organisation organization)
			: base(organization)
		{
			Year = year;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
		}

		protected override HolidayViewModel CreateViewModel(Holiday model)
		{
			return new HolidayViewModel(model);
		}

		private void OnAdd()
		{
			var holidayDetailsViewModel = new HolidayDetailsViewModel(this);
			if (DialogService.ShowModalWindow(holidayDetailsViewModel) && HolidayHelper.Save(holidayDetailsViewModel.Holiday))
			{
				var holidayViewModel = new HolidayViewModel(holidayDetailsViewModel.Holiday);
				ViewModels.Add(holidayViewModel);
				Sort();
				SelectedViewModel = holidayViewModel;
			}
		}
		private bool CanAdd()
		{
			return ViewModels.Count < 100;
		}
		private void OnDelete()
		{
			if (HolidayHelper.MarkDeleted(SelectedViewModel.Model))
				ViewModels.Remove(SelectedViewModel);
		}
		private bool CanDelete()
		{
			return SelectedViewModel != null;
		}
		private void OnEdit()
		{
			var holidayDetailsViewModel = new HolidayDetailsViewModel(this, SelectedViewModel.Model);
			if (DialogService.ShowModalWindow(holidayDetailsViewModel))
			{
				HolidayHelper.Save(SelectedViewModel.Model);
				SelectedViewModel.Update();
				Sort();
			}
		}
		private bool CanEdit()
		{
			return SelectedViewModel != null;
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		private void OnShowSettings()
		{
			var holidaySettingsViewModel = new HolidaySettingsViewModel();
			DialogService.ShowModalWindow(holidaySettingsViewModel);
		}

		private void Sort()
		{
			ViewModels.Sort(item => item.Model.Date);
		}
	}
}