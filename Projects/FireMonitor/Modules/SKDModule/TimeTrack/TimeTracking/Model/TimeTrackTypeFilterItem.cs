using StrazhAPI.SKD;
using ReactiveUI;

namespace SKDModule.Model
{
	public class TimeTrackTypeFilterItem : ReactiveObject
	{
		public TimeTrackTypeFilterItem(TimeTrackType timeTrackType)
		{
			TimeTrackType = timeTrackType;
		}

		public TimeTrackType TimeTrackType { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set { this.RaiseAndSetIfChanged(ref _isChecked, value); }
		}
	}
}