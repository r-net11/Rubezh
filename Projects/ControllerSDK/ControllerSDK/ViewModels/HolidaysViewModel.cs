using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using System;
using ControllerSDK.SDK;
using ControllerSDK.Views;
using ControllerSDK.API;
using System.Windows;
using System.Collections.Generic;

namespace ControllerSDK.ViewModels
{
	public class HolidaysViewModel : BaseViewModel
	{
		public HolidaysViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			GetInfoCommand = new RelayCommand(OnGetInfo);
			RemoveCommand = new RelayCommand(OnRemove);
			RemoveAllCommand = new RelayCommand(OnRemoveAll);
			GetCountCommand = new RelayCommand(OnGetCount);
			GetAllCommand = new RelayCommand(OnGetAll);
			Holidays = new ObservableCollection<HolidayViewModel>();

			StartDateTime = DateTime.Now;
			EndDateTime = DateTime.Now;
			IsEnabled = true;
			DoorsCount = 1;
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
			var holiday = new Holiday();
			holiday.StartDateTime = StartDateTime;
			holiday.EndDateTime = EndDateTime;
			holiday.IsEnabled = IsEnabled;
			holiday.DoorsCount = DoorsCount;
			var newHolidayNo = SDKWrapper.AddHoliday(MainWindow.LoginID, holiday);
			MessageBox.Show("newHolidayNo = " + newHolidayNo);
		}

		public RelayCommand GetInfoCommand { get; private set; }
		void OnGetInfo()
		{
			var result = SDKWrapper.GetHolidayInfo(MainWindow.LoginID, 0);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (SelectedHoliday != null)
			{
				var result = SDKImport.WRAP_DevCtrl_RemoveRecordSet(MainWindow.LoginID, SelectedHoliday.Holiday.RecordNo, 4);
				MessageBox.Show("result = " + result);
			}
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			var result = SDKImport.WRAP_DevCtrl_ClearRecordSet(MainWindow.LoginID, 4);
			MessageBox.Show("result = " + result);
		}

		public RelayCommand GetCountCommand { get; private set; }
		void OnGetCount()
		{
			var holidaysCount = SDKImport.WRAP_Get_HolidaysCount(MainWindow.LoginID);
			MessageBox.Show("holidaysCount = " + holidaysCount);
		}

		public RelayCommand GetAllCommand { get; private set; }
		void OnGetAll()
		{
			var holidays = SDKWrapper.GetAllHolidays(MainWindow.LoginID);

			Holidays.Clear();
			foreach (var holiday in holidays)
			{
				var holidayViewModel = new HolidayViewModel(holiday);
				Holidays.Add(holidayViewModel);
			}
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

		bool _isEnabled;
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
				OnPropertyChanged(() => IsEnabled);
			}
		}

		int _doorsCount;
		public int DoorsCount
		{
			get { return _doorsCount; }
			set
			{
				_doorsCount = value;
				OnPropertyChanged(() => DoorsCount);
			}
		}
	}
}