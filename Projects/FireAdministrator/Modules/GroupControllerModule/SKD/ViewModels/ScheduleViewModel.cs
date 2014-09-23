using System;
using System.Linq;
using System.Collections.Generic;
using FiresecAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class ScheduleViewModel : BaseViewModel
	{
		public XSchedule Schedule { get; set; }

		public ScheduleViewModel(XSchedule schedule)
		{
			Schedule = schedule;
			Update();
		}

		public string Name
		{
			get { return Schedule.Name; }
			set
			{
				Schedule.Name = value;
				Schedule.OnChanged();
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string Description
		{
			get { return Schedule.Description; }
			set
			{
				Schedule.Description = value;
				Schedule.OnChanged();
				OnPropertyChanged(() => Description);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void Update(XSchedule schedule)
		{
			Schedule = schedule;
			OnPropertyChanged(() => Schedule);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			Update();
		}
		public void Update()
		{
		}
	}
}