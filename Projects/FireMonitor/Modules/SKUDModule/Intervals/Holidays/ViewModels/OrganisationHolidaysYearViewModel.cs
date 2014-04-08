using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Organization = FiresecAPI.Organization;

namespace SKDModule.ViewModels
{
	public class OrganisationHolidaysYearViewModel : BaseViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public int Year { get; private set; }
		public Organization Organization { get; private set; }

		public OrganisationHolidaysYearViewModel(int year)
		{
			Year = year;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
		}

		public void Initialize(Organization organization, List<Holiday> holidays)
		{
			Organization = organization;

			//if (Holidays.Count == 0)
			//{
			//    Holidays.Add(new Holiday() { Name = "Новый год", Date = new DateTime(2014, 1, 1) });
			//    Holidays.Add(new Holiday() { Name = "Новый год", Date = new DateTime(2014, 1, 2) });
			//    Holidays.Add(new Holiday() { Name = "Праздник продолжается", Date = new DateTime(2014, 1, 3) });
			//    Holidays.Add(new Holiday() { Name = "Праздник продолжается", Date = new DateTime(2014, 1, 4) });
			//    Holidays.Add(new Holiday() { Name = "Праздник продолжается", Date = new DateTime(2014, 1, 5) });
			//    Holidays.Add(new Holiday() { Name = "Рождество христово", Date = new DateTime(2014, 1, 6), HolidayType = HolidayType.BeforeHoliday, ShortageTime = new Date(2014, 1, 6, 1, 0, 0) });
			//    Holidays.Add(new Holiday() { Name = "Рождество христово", Date = new DateTime(2014, 1, 7) });
			//    Holidays.Add(new Holiday() { Name = "День советской армии и военно-морского флота", Date = new Date(2014, 2, 23) });
			//    Holidays.Add(new Holiday() { Name = "Международный женский день", Date = new DateTime(2014, 3, 8) });
			//    Holidays.Add(new Holiday() { Name = "День весны и труда", Date = new DateTime(2014, 4, 30), HolidayType = HolidayType.BeforeHoliday, ShortageTime = new Date(2014, 4, 30, 1, 0, 0) });
			//    Holidays.Add(new Holiday() { Name = "День весны и труда", Date = new DateTime(2014, 5, 1) });
			//    Holidays.Add(new Holiday() { Name = "День весны и труда", Date = new DateTime(2014, 5, 2) });
			//    Holidays.Add(new Holiday() { Name = "День победы", Date = new DateTime(2014, 5, 9) });
			//    Holidays.Add(new Holiday() { Name = "День России", Date = new DateTime(2014, 6, 11), HolidayType = HolidayType.BeforeHoliday, ShortageTime = new Date(2014, 6, 11, 1, 0, 0) });
			//    Holidays.Add(new Holiday() { Name = "День России", Date = new DateTime(2014, 6, 12) });
			//    Holidays.Add(new Holiday() { Name = "День народного удинства", Date = new Date(2014, 11, 4) });
			//    Holidays.Add(new Holiday() { Name = "Новый год", Date = new DateTime(2014, 12, 31), HolidayType = HolidayType.BeforeHoliday, ShortageTime = new Date(2014, 12, 31, 1, 0, 0) });
			//}

			Holidays = new ObservableCollection<HolidayViewModel>();
			foreach (var holiday in holidays)
			{
				var holidayViewModel = new HolidayViewModel(holiday);
				Holidays.Add(holidayViewModel);
			}
			SelectedHoliday = Holidays.FirstOrDefault();
		}

		ObservableCollection<HolidayViewModel> _holidays;
		public ObservableCollection<HolidayViewModel> Holidays
		{
			get { return _holidays; }
			set
			{
				_holidays = value;
				OnPropertyChanged("Holidays");
			}
		}

		HolidayViewModel _selectedHoliday;
		public HolidayViewModel SelectedHoliday
		{
			get { return _selectedHoliday; }
			set
			{
				_selectedHoliday = value;
				OnPropertyChanged("SelectedHoliday");
			}
		}

		public void Select(Guid holidayUID)
		{
			if (holidayUID != Guid.Empty)
			{
				var holidayViewModel = Holidays.FirstOrDefault(x => x.Holiday.UID == holidayUID);
				if (holidayViewModel != null)
				{
					SelectedHoliday = holidayViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var holidayDetailsViewModel = new HolidayDetailsViewModel(this);
			if (DialogService.ShowModalWindow(holidayDetailsViewModel))
			{
				var holidayViewModel = new HolidayViewModel(holidayDetailsViewModel.Holiday);
				Holidays.Add(holidayViewModel);
				SelectedHoliday = holidayViewModel;
			}
		}
		bool CanAdd()
		{
			return Holidays.Count < 100;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			Holidays.Remove(SelectedHoliday);
		}
		bool CanDelete()
		{
			return SelectedHoliday != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var holidayDetailsViewModel = new HolidayDetailsViewModel(this, SelectedHoliday.Holiday);
			if (DialogService.ShowModalWindow(holidayDetailsViewModel))
			{
				SelectedHoliday.Update();
			}
		}
		bool CanEdit()
		{
			return SelectedHoliday != null;
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		void OnShowSettings()
		{
			var holidaySettingsViewModel = new HolidaySettingsViewModel();
			DialogService.ShowModalWindow(holidaySettingsViewModel);
		}
	}
}