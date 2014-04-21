using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Organization = FiresecAPI.Organization;
using FiresecClient.SKDHelpers;
using Common;

namespace SKDModule.ViewModels
{
	public class OrganisationHolidaysYearViewModel : BaseViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public int Year { get; private set; }
		public Organization Organization { get; private set; }

		public OrganisationHolidaysYearViewModel(int year, Organization organization)
		{
			Year = year;
			Organization = organization;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
		}

		public void Initialize(List<Holiday> holidays)
		{
			Holidays = new SortableObservableCollection<HolidayViewModel>();
			foreach (var holiday in holidays)
			{
				var holidayViewModel = new HolidayViewModel(holiday);
				Holidays.Add(holidayViewModel);
			}
			SelectedHoliday = Holidays.FirstOrDefault();
		}

		private SortableObservableCollection<HolidayViewModel> _holidays;
		public SortableObservableCollection<HolidayViewModel> Holidays
		{
			get { return _holidays; }
			set
			{
				_holidays = value;
				OnPropertyChanged(() => Holidays);
			}
		}

		private HolidayViewModel _selectedHoliday;
		public HolidayViewModel SelectedHoliday
		{
			get { return _selectedHoliday; }
			set
			{
				_selectedHoliday = value;
				OnPropertyChanged(() => SelectedHoliday);
			}
		}

		public void Select(Guid holidayUID)
		{
			if (holidayUID != Guid.Empty)
			{
				var holidayViewModel = Holidays.FirstOrDefault(x => x.Holiday.UID == holidayUID);
				if (holidayViewModel != null)
					SelectedHoliday = holidayViewModel;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var holidayDetailsViewModel = new HolidayDetailsViewModel(this);
			if (DialogService.ShowModalWindow(holidayDetailsViewModel) && HolidayHelper.Save(holidayDetailsViewModel.Holiday))
			{
				var holidayViewModel = new HolidayViewModel(holidayDetailsViewModel.Holiday);
				Holidays.Add(holidayViewModel);
				Sort();
				SelectedHoliday = holidayViewModel;
			}
		}
		private bool CanAdd()
		{
			return Holidays.Count < 100;
		}

		public RelayCommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
			if (HolidayHelper.MarkDeleted(SelectedHoliday.Holiday))
				Holidays.Remove(SelectedHoliday);
		}
		private bool CanDelete()
		{
			return SelectedHoliday != null;
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var holidayDetailsViewModel = new HolidayDetailsViewModel(this, SelectedHoliday.Holiday);
			if (DialogService.ShowModalWindow(holidayDetailsViewModel))
			{
				HolidayHelper.Save(SelectedHoliday.Holiday);
				SelectedHoliday.Update();
				Sort();
			}
		}
		private bool CanEdit()
		{
			return SelectedHoliday != null;
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		private void OnShowSettings()
		{
			var holidaySettingsViewModel = new HolidaySettingsViewModel();
			DialogService.ShowModalWindow(holidaySettingsViewModel);
		}

		private void Sort()
		{
			Holidays.Sort(item => item.Holiday.Date);
		}
	}
}