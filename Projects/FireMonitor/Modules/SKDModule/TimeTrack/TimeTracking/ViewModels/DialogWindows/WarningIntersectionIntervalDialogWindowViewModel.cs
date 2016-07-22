using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Model;
using System.Collections.Generic;
using System.Linq;

namespace SKDModule.ViewModels
{
	public class WarningIntersectionIntervalDialogWindowViewModel : OverridedSaveCancelDialogViewModel
	{
		public DayTimeTrackPart CurrentDayTimeTrackPart { get; private set; }

		public List<DayTimeTrackPart> IntersectionPartsCollection { get; private set; }

		public WarningIntersectionIntervalDialogWindowViewModel(DayTimeTrackPart dayTimeTrackPart,
			IEnumerable<DayTimeTrackPart> intersectionTimeTrackParts) : this()
		{
			CurrentDayTimeTrackPart = dayTimeTrackPart;
			IntersectionPartsCollection = intersectionTimeTrackParts.ToList();
		}

		public WarningIntersectionIntervalDialogWindowViewModel()
		{
			Title = "Пересечение границ интервалов";
			OkCommand = new RelayCommand(OnOkPress);
		}

		public RelayCommand OkCommand { get; private set; }

		public void OnOkPress()
		{
			Close(false);
		}
	}
}
