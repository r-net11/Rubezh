using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Events;
using SKDModule.Events;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalViewModel : BaseViewModel
	{
		public SKDDayInterval DayInterval { get; private set; }
		public DayIntervalPartsViewModel DayIntervalPartsViewModel { get; private set; }

		public DayIntervalViewModel(SKDDayInterval dayInterval)
		{
			DayInterval = dayInterval;
			DayIntervalPartsViewModel = new DayIntervalPartsViewModel(dayInterval);
			Update(DayInterval);
		}

		public void Update(SKDDayInterval dayInterval)
		{
			DayInterval = dayInterval;
			OnPropertyChanged(() => DayInterval);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
		}

		public string Name
		{
			get { return DayInterval.Name; }
		}

		public string Description
		{
			get { return DayInterval.Description; }
		}
	}
}