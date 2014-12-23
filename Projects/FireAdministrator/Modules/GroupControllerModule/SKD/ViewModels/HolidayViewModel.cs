using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class HolidayViewModel : BaseViewModel
	{
		GKHoliday Holiday { get; set; }

		public HolidayViewModel(GKHoliday holiday)
		{
			Holiday = holiday;
		}
	}
}
