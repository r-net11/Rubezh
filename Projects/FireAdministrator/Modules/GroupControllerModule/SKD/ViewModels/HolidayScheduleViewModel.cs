using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using FiresecClient;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{	
	public class HolidayScheduleViewModel : BaseViewModel
	{
		HolidayScheduleViewModel()
		{
		
		}

		public void Initialize()
		{
			Holidays = new ObservableCollection<HolidayViewModel>();
			foreach (var holiday in GKManager.DeviceConfiguration.Holidays.OrderBy(x => x.No))
			{
				var holidayViewModel = new HolidayViewModel(holiday);
				Holidays.Add(holidayViewModel);
			}
			SelectedHoliday = Holidays.FirstOrDefault();
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
	}
}
