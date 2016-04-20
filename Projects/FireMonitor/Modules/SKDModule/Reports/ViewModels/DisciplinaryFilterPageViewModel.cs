using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.SKDReports;
using RubezhAPI.SKD.ReportFilters;

namespace SKDModule.Reports.ViewModels
{
	public class DisciplinaryFilterPageViewModel : FilterContainerViewModel
	{
		public DisciplinaryFilterPageViewModel()
		{
			Title = "Фильтры";
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
			ShowDelay = disciplinaryFilter.ShowDelay;
			ShowEarlуRetirement = disciplinaryFilter.ShowEarlуRetirement;
			ShowAbsence = disciplinaryFilter.ShowAbsence;
			ShowOvertime = disciplinaryFilter.ShowOvertime;
			ShowConfirmed = disciplinaryFilter.ShowConfirmed;
			ShowWithoutTolerance = disciplinaryFilter.ShowWithoutTolerance;
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var disciplinaryFilter = filter as DisciplineReportFilter;
			if (disciplinaryFilter == null)
				return;
			disciplinaryFilter.ShowDelay = ShowDelay;
			disciplinaryFilter.ShowEarlуRetirement = ShowEarlуRetirement;
			disciplinaryFilter.ShowAbsence = ShowAbsence;
			disciplinaryFilter.ShowOvertime = ShowOvertime;
			disciplinaryFilter.ShowConfirmed = ShowConfirmed;
			disciplinaryFilter.ShowWithoutTolerance = ShowWithoutTolerance;
		}
	}
}