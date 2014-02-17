using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace SKDModule.ViewModels
{
	public class HolidaysViewModel : ViewPartViewModel
	{
		public HolidaysViewModel()
		{
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		void Initialize()
		{
			AvailableYears = new ObservableCollection<HolidayYearViewModel>();
			for (int i = 2010; i <= 2020; i++)
			{
				AvailableYears.Add(new HolidayYearViewModel(i));
			}
			SelectedYear = AvailableYears.FirstOrDefault(x => x.Year == DateTime.Now.Year);
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}

		ObservableCollection<HolidayYearViewModel> _availableYears;
		public ObservableCollection<HolidayYearViewModel> AvailableYears
		{
			get { return _availableYears; }
			set
			{
				_availableYears = value;
				OnPropertyChanged("AvailableYears");
			}
		}

		HolidayYearViewModel _selectedYear;
		public HolidayYearViewModel SelectedYear
		{
			get { return _selectedYear; }
			set
			{
				_selectedYear = value;
				OnPropertyChanged("SelectedYear");
			}
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		void OnShowSettings()
		{
			var holidaySettingsViewModel = new HolidaySettingsViewModel();
			DialogService.ShowModalWindow(holidaySettingsViewModel);
		}
	}
}