using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows.Views;
using SKDModule.Model;

namespace SKDModule.ViewModels
{
	public class WarningIntersectionIntervalDialogWindowViewModel : OverridedSaveCancelDialogViewModel
	{
		public DayTimeTrackPart CurrentDayTimeTrackPart { get; set; }

		public List<DayTimeTrackPart> IntersectionPartsCollection { get; set; }

		public WarningIntersectionIntervalDialogWindowViewModel(DayTimeTrackPart dayTimeTrackPart,
			IEnumerable<DayTimeTrackPart> intersectionTimeTrackParts) : this()
		{
			CurrentDayTimeTrackPart = dayTimeTrackPart;
			IntersectionPartsCollection = intersectionTimeTrackParts.ToList();
		}

		public WarningIntersectionIntervalDialogWindowViewModel()
		{
			OkCommand = new RelayCommand(OnOkPress);
		}

		public RelayCommand OkCommand { get; private set; }

		public void OnOkPress()
		{
			Title = "Пересечение границ интервалов";
			Close(false);
		}
	}
}
