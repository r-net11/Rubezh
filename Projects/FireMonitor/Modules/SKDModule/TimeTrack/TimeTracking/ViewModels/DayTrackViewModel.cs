using System;
using System.Linq;
using RubezhAPI;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Model;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class DayTrackViewModel : BaseViewModel
	{
		public DayTimeTrack DayTimeTrack { get; private set; }
		public TimeTrackFilter TimeTrackFilter { get; private set; }
		public ShortEmployee ShortEmployee { get; private set; }

		public DayTrackViewModel(DayTimeTrack dayTimeTrack, TimeTrackFilter timeTrackFilter, ShortEmployee shortEmployee)
		{
			DayTimeTrack = dayTimeTrack;
			TimeTrackFilter = timeTrackFilter;
			ShortEmployee = shortEmployee;
			Update();
		}

		public void Update()
		{
			OnPropertyChanged(() => DayTimeTrack);

			Totals = new List<TimeTrackTotal>();
			foreach (var totalTimeTrackTypeFilter in TimeTrackFilter.TotalTimeTrackTypeFilters)
			{
				var timeTrackTotal = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == totalTimeTrackTypeFilter);
				if (timeTrackTotal != null)
				{
					Totals.Add(timeTrackTotal);
				}
			}
			OnPropertyChanged(() => Totals);
		}

		public List<TimeTrackTotal> Totals { get; private set; }
	}
}