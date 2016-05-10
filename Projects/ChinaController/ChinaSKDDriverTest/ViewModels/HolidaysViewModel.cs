using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using StrazhDeviceSDK.API;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class HolidaysViewModel : BaseViewModel
	{
		public HolidaysViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit);
			RemoveCommand = new RelayCommand(OnRemove);
			RemoveAllCommand = new RelayCommand(OnRemoveAll);
			GetInfoCommand = new RelayCommand(OnGetInfo);
			GetCountCommand = new RelayCommand(OnGetCount);
			GetAllCommand = new RelayCommand(OnGetAll);
			Holidays = new ObservableCollection<HolidayViewModel>();

			HolidayNo = "1";
			StartDateTime = DateTime.Now;
			EndDateTime = DateTime.Now;

			Doors = new ObservableCollection<DoorItemViewModel>();
			for (int i = 0; i < 4; i++)
			{
				Doors.Add(new DoorItemViewModel(i));
			}
			Doors[0].IsChecked = true;
		}

		public void Initialize(List<Holiday> holidays)
		{
			Holidays.Clear();
			foreach (var holiday in holidays)
			{
				var holidayViewModel = new HolidayViewModel(holiday);
				Holidays.Add(holidayViewModel);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var holiday = GetModel();
			var newHolidayNo = MainViewModel.Wrapper.AddHoliday(holiday);
			MessageBox.Show("newHolidayNo = " + newHolidayNo);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var holiday = GetModel();
			var result = MainViewModel.Wrapper.EditHoliday(holiday);
			MessageBox.Show("result = " + result);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var result = MainViewModel.Wrapper.RemoveHoliday(Index);
			MessageBox.Show("result = " + result);
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			var result = MainViewModel.Wrapper.RemoveAllHolidays();
			MessageBox.Show("result = " + result);
		}

		public RelayCommand GetInfoCommand { get; private set; }
		void OnGetInfo()
		{
			var result = MainViewModel.Wrapper.GetHolidayInfo(Index);
			Initialize(new List<Holiday>() { result });
		}

		public RelayCommand GetCountCommand { get; private set; }
		void OnGetCount()
		{
			var holidaysCount = MainViewModel.Wrapper.GetHolidaysCount(); ;
			MessageBox.Show("holidaysCount = " + holidaysCount);
		}

		public RelayCommand GetAllCommand { get; private set; }
		void OnGetAll()
		{
			var holidays = MainViewModel.Wrapper.GetAllHolidays();

			Holidays.Clear();
			foreach (var holiday in holidays)
			{
				var holidayViewModel = new HolidayViewModel(holiday);
				Holidays.Add(holidayViewModel);
			}
		}

		Holiday GetModel()
		{
			var holiday = new Holiday();
			holiday.StartDateTime = StartDateTime;
			holiday.EndDateTime = EndDateTime;
			holiday.HolidayNo = HolidayNo;
			foreach (var door in Doors)
			{
				if (door.IsChecked)
				{
					holiday.Doors.Add(door.No);
				}
			}
			holiday.DoorsCount = holiday.Doors.Count;
			return holiday;
		}

		public ObservableCollection<HolidayViewModel> Holidays { get; private set; }

		HolidayViewModel _selectedHoliday;
		public HolidayViewModel SelectedHoliday
		{
			get { return _selectedHoliday; }
			set
			{
				_selectedHoliday = value;
				OnPropertyChanged(() => SelectedHoliday);
			}
		}

		int _index;
		public int Index
		{
			get { return _index; }
			set
			{
				_index = value;
				OnPropertyChanged(() => Index);
			}
		}

		string _holidayNo;
		public string HolidayNo
		{
			get { return _holidayNo; }
			set
			{
				_holidayNo = value;
				OnPropertyChanged(() => HolidayNo);
			}
		}

		DateTime _startDateTime;
		public DateTime StartDateTime
		{
			get { return _startDateTime; }
			set
			{
				_startDateTime = value;
				OnPropertyChanged(() => StartDateTime);
			}
		}

		DateTime _endDateTime;
		public DateTime EndDateTime
		{
			get { return _endDateTime; }
			set
			{
				_endDateTime = value;
				OnPropertyChanged(() => EndDateTime);
			}
		}

		public ObservableCollection<DoorItemViewModel> Doors { get; private set; }
	}
}