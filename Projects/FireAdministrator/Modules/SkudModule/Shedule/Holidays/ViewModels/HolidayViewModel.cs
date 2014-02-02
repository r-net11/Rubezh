using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class HolidayViewModel : BaseViewModel
	{
		public SKDHoliday Holiday { get; set; }

		public HolidayViewModel(SKDHoliday holiday)
		{
			Holiday = holiday;
		}

		public void Update()
		{
			OnPropertyChanged("Holiday");
		}
	}
}