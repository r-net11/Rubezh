using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using FiresecAPI.SKD.ReportFilters;

namespace SKDModule.Reports.ViewModels
{
	public class DisciplinaryFilterPageViewModel : FilterContainerViewModel
	{
		public DisciplinaryFilterPageViewModel()
		{
			Title = "Фильтры";
		}

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
		private bool _showDelay;
		public bool ShowDelay
		{
			get { return _showDelay; }
			set
			{
				_showDelay = value;
				OnPropertyChanged(() => ShowDelay);
			}
		}
		private bool _showEarlуRetirement;
		public bool ShowEarlуRetirement
		{
			get { return _showEarlуRetirement; }
			set
			{
				_showEarlуRetirement = value;
				OnPropertyChanged(() => ShowEarlуRetirement);
			}
		}
		private bool _showTolerance;
		public bool ShowTolerance
		{
			get { return _showTolerance; }
			set
			{
				_showTolerance = value;
				OnPropertyChanged(() => ShowTolerance);
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
		private bool _showMissingtime;
		public bool ShowMissingtime
		{
			get { return _showMissingtime; }
			set
			{
				_showMissingtime = value;
				OnPropertyChanged(() => ShowMissingtime);
			}
		}
		private bool _showConfirmed;
		public bool ShowConfirmed
		{
			get { return _showConfirmed; }
			set
			{
				_showConfirmed = value;
				OnPropertyChanged(() => ShowConfirmed);
			}
		}
		private bool _showWithoutTolerance;
		public bool ShowWithoutTolerance
		{
			get { return _showWithoutTolerance; }
			set
			{
				_showWithoutTolerance = value;
				OnPropertyChanged(() => ShowWithoutTolerance);
			}
		}	

		public override void LoadFilter(SKDReportFilter filter)
		{
			var disciplinaryFilter = filter as DisciplineReportFilter;
			if (disciplinaryFilter == null)
				return;
			ShowAllViolation = disciplinaryFilter.ShowAllViolation;
			ShowDelay = disciplinaryFilter.ShowDelay;
			ShowEarlуRetirement = disciplinaryFilter.ShowEarlуRetirement;
			ShowTolerance = disciplinaryFilter.ShowTolerance;
			ShowAbsence = disciplinaryFilter.ShowAbsence;
			ShowOvertime = disciplinaryFilter.ShowOvertime;
			ShowMissingtime = disciplinaryFilter.ShowMissingtime;
			ShowConfirmed = disciplinaryFilter.ShowConfirmed;
			ShowWithoutTolerance = disciplinaryFilter.ShowWithoutTolerance;
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var disciplinaryFilter = filter as DisciplineReportFilter;
			if (disciplinaryFilter == null)
				return;
			disciplinaryFilter.ShowAllViolation = ShowAllViolation;
			disciplinaryFilter.ShowDelay = ShowDelay;
			disciplinaryFilter.ShowEarlуRetirement = ShowEarlуRetirement;
			disciplinaryFilter.ShowTolerance = ShowTolerance;
			disciplinaryFilter.ShowAbsence = ShowAbsence;
			disciplinaryFilter.ShowOvertime = ShowOvertime;
			disciplinaryFilter.ShowMissingtime = ShowMissingtime;
			disciplinaryFilter.ShowConfirmed = ShowConfirmed;
			disciplinaryFilter.ShowWithoutTolerance = ShowWithoutTolerance;
		}
	}
}
