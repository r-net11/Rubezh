using Infrastructure.Common.SKDReports;
using Localization.SKD.ViewModels;
using StrazhAPI.SKD.ReportFilters;

namespace SKDModule.Reports.ViewModels
{
	public class DisciplinaryFilterPageViewModel : FilterContainerViewModel
	{
		#region Properties
		private bool _showAllViolation;
		public bool ShowAllViolation
		{
			get { return _showAllViolation; }
			set
			{
				_showAllViolation = value;
				OnPropertyChanged(() => ShowAllViolation);
			}
		}

		private bool _showLate;
		public bool ShowLate
		{
			get { return _showLate; }
			set
			{
				_showLate = value;
				OnPropertyChanged(() => ShowLate);
			}
		}

		private bool _showEarlуLeave;
		public bool ShowEarlуLeave
		{
			get { return _showEarlуLeave; }
			set
			{
				_showEarlуLeave = value;
				OnPropertyChanged(() => ShowEarlуLeave);
			}
		}

		private bool _showAbsence;
		public bool ShowAbsence
		{
			get { return _showAbsence; }
			set
			{
				_showAbsence = value;
				OnPropertyChanged(() => ShowAbsence);
			}
		}

		private bool _showOvertime;
		public bool ShowOvertime
		{
			get { return _showOvertime; }
			set
			{
				_showOvertime = value;
				OnPropertyChanged(() => ShowOvertime);
			}
		}

		private bool _showShiftedViolation;

		public bool ShowShiftedViolation
		{
			get { return _showShiftedViolation; }
			set
			{
				_showShiftedViolation = value;
				OnPropertyChanged(() => ShowShiftedViolation);
			}
		}

		#endregion

		#region Constructors

		public DisciplinaryFilterPageViewModel()
		{
			Title = CommonViewModels.Filters;
		}

		#endregion

		#region Override Methods

		public override void LoadFilter(SKDReportFilter filter)
		{
			var disciplinaryFilter = filter as DisciplineReportFilter;
			if (disciplinaryFilter == null)
				return;
			ShowAllViolation = disciplinaryFilter.ShowAllViolation;
			ShowShiftedViolation = disciplinaryFilter.ShowShiftedViolation;
			ShowLate = disciplinaryFilter.ShowLate;
			ShowEarlуLeave = disciplinaryFilter.ShowEarlуLeave;
			ShowAbsence = disciplinaryFilter.ShowAbsence;
			ShowOvertime = disciplinaryFilter.ShowOvertime;
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var disciplinaryFilter = filter as DisciplineReportFilter;
			if (disciplinaryFilter == null)
				return;
			disciplinaryFilter.ShowAllViolation = ShowAllViolation;
			disciplinaryFilter.ShowLate = ShowLate;
			disciplinaryFilter.ShowEarlуLeave = ShowEarlуLeave;
			disciplinaryFilter.ShowAbsence = ShowAbsence;
			disciplinaryFilter.ShowOvertime = ShowOvertime;
			disciplinaryFilter.ShowShiftedViolation = ShowShiftedViolation;
		}

		#endregion
	}
}
