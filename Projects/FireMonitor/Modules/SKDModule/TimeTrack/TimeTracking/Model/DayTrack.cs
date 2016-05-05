using StrazhAPI.SKD;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SKDModule.Model
{
	public class DayTrack : ReactiveObject
	{
		#region Properties

		private DayTimeTrack _dayTimeTrack;
		public DayTimeTrack DayTimeTrack
		{
			get { return _dayTimeTrack; }
			private set
			{
				_dayTimeTrack = value;
				this.RaisePropertyChanged(x => x.DayTimeTrack);
			}
		}
		public TimeTrackFilter TimeTrackFilter { get; private set; }
		public ShortEmployee ShortEmployee { get; private set; }
		public ObservableCollection<TimeTrackTotal> Totals { get; private set; }
		#endregion

		#region Constructors

		public DayTrack(DayTimeTrack dayTimeTrack, TimeTrackFilter timeTrackFilter, ShortEmployee shortEmployee)
		{
			DayTimeTrack = dayTimeTrack;
			TimeTrackFilter = timeTrackFilter;
			ShortEmployee = shortEmployee;
			Update();
		}

		#endregion

		#region Methods

		public void Update()
		{
			Totals = GetTotals(TimeTrackFilter.TotalTimeTrackTypeFilters);
		}

		private ObservableCollection<TimeTrackTotal> GetTotals(List<TimeTrackType> totalTimeTrackTypeFilters)
		{
			if (DayTimeTrack.Totals == null) return new ObservableCollection<TimeTrackTotal>();

			var result = new ObservableCollection<TimeTrackTotal>();

			foreach (var element in totalTimeTrackTypeFilters)
			{
				var timeTrackTotal = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == element);
				if (timeTrackTotal != null)
				{
					result.Add(timeTrackTotal);
				}
			}

			return result;
		}

		#endregion
	}
}