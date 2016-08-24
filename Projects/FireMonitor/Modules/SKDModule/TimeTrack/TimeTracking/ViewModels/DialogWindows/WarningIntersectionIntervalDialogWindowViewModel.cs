using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Localization.SKD.ViewModels;
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
			Title = CommonViewModels.CrossIntervalBounds;
			OkCommand = new RelayCommand(OnOkPress);
		}

		public RelayCommand OkCommand { get; private set; }

		public void OnOkPress()
		{
			Close(false);
		}
	}
}
