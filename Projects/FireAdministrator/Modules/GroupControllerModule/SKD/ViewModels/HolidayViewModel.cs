using System;
using System.Linq;
using System.Collections.Generic;
using FiresecAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using FiresecClient;
using Common;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class HolidayViewModel : BaseViewModel
	{
		public GKHoliday Holiday { get; set; }

		public HolidayViewModel(GKHoliday holiday)
		{
			Holiday = holiday;
			Update();
		}

		public string Name
		{
			get { return Holiday.Name; }
			set
			{
				Holiday.Name = value;
				Holiday.OnChanged();
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string Description
		{
			get { return Holiday.Description; }
			set
			{
				Holiday.Description = value;
				Holiday.OnChanged();
				OnPropertyChanged(() => Description);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void Update(GKHoliday holiday)
		{
			Holiday = holiday;
			OnPropertyChanged(() => Holiday);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			Update();
		}

		public void Update()
		{
			OnPropertyChanged(() => ReductionTime);
			OnPropertyChanged(() => TransitionDate);
			OnPropertyChanged(() => Holiday);
		}

		public string ReductionTime
		{
			get
			{
				if (Holiday != null && Holiday.HolidayType == GKHolidayType.BeforeHoliday)
					return Holiday.Reduction.ToString("hh\\-mm");
				return null;
			}
		}
		public string TransitionDate
		{
			get
			{
				if (Holiday != null && Holiday.HolidayType == GKHolidayType.WorkingHoliday && Holiday.TransferDate.HasValue)
					return Holiday.TransferDate.Value.ToString("dd-MM");
				return null;
			}
		}
	}
}