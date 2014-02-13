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

			AvailableYears = new ObservableCollection<HolidayYearViewModel>();
			for (int i = 2010; i <= 2020; i++)
			{
				AvailableYears.Add(new HolidayYearViewModel(i));
			}
			SelectedYear = AvailableYears.FirstOrDefault(x => x.Year == DateTime.Now.Year);
		}

		public ObservableCollection<HolidayYearViewModel> AvailableYears { get; private set; }

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