using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class HolidayYearViewModel : BaseViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public int Year { get; private set; }

		public HolidayYearViewModel(int year)
		{
			Year = year;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			Initialize();
		}

		public void Initialize()
		{
			var employeeHolidays = new List<EmployeeHoliday>();
			if (employeeHolidays.Count == 0)
			{
				employeeHolidays.Add(new EmployeeHoliday() { Name = "Новый год", DateTime = new DateTime(2014, 1, 1) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "Новый год", DateTime = new DateTime(2014, 1, 2) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "Праздник продолжается", DateTime = new DateTime(2014, 1, 3) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "Праздник продолжается", DateTime = new DateTime(2014, 1, 4) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "Праздник продолжается", DateTime = new DateTime(2014, 1, 5) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "Рождество христово", DateTime = new DateTime(2014, 1, 6), EmployeeHolidayType = EmployeeHolidayType.BeforeHoliday, ShortageTime = new DateTime(2014, 1, 6, 1, 0, 0) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "Рождество христово", DateTime = new DateTime(2014, 1, 7) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "День советской армии и военно-морского флота", DateTime = new DateTime(2014, 2, 23) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "Международный женский день", DateTime = new DateTime(2014, 3, 8) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "День весны и труда", DateTime = new DateTime(2014, 4, 30), EmployeeHolidayType = EmployeeHolidayType.BeforeHoliday, ShortageTime = new DateTime(2014, 4, 30, 1, 0, 0) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "День весны и труда", DateTime = new DateTime(2014, 5, 1) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "День весны и труда", DateTime = new DateTime(2014, 5, 2) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "День победы", DateTime = new DateTime(2014, 5, 9) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "День России", DateTime = new DateTime(2014, 6, 11), EmployeeHolidayType = EmployeeHolidayType.BeforeHoliday, ShortageTime = new DateTime(2014, 6, 11, 1, 0, 0) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "День России", DateTime = new DateTime(2014, 6, 12) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "День народного удинства", DateTime = new DateTime(2014, 11, 4) });
				employeeHolidays.Add(new EmployeeHoliday() { Name = "Новый год", DateTime = new DateTime(2014, 12, 31), EmployeeHolidayType = EmployeeHolidayType.BeforeHoliday, ShortageTime = new DateTime(2014, 12, 31, 1, 0, 0) });
			}

			Holidays = new ObservableCollection<HolidayViewModel>();
			foreach (var holiday in employeeHolidays)
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
	}
}