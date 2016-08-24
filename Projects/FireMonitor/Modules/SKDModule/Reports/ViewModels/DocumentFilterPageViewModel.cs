using Infrastructure.Common.SKDReports;
using Localization.SKD.Common;
using StrazhAPI.SKD.ReportFilters;

namespace SKDModule.Reports.ViewModels
{
	public class DocumentFilterPageViewModel : FilterContainerViewModel
	{
		public DocumentFilterPageViewModel()
		{
			Title = CommonResources.Document;
		}

		private bool _overtime;
		public bool Overtime
		{
			get { return _overtime; }
			set
			{
				_overtime = value;
				OnPropertyChanged(() => Overtime);
			}
		}
		private bool _presence;
		public bool Presence
		{
			get { return _presence; }
			set
			{
				_presence = value;
				OnPropertyChanged(() => Presence);
			}
		}
		private bool _abcense;
		public bool Abcense
		{
			get { return _abcense; }
			set
			{
				_abcense = value;
				OnPropertyChanged(() => Abcense);
			}
		}

		private bool _abcenseReasonable;
		public bool AbcenseReasonable
		{
			get { return _abcenseReasonable; }
			set
			{
				_abcenseReasonable = value;
				OnPropertyChanged(() => AbcenseReasonable);
			}
		}

		public override void LoadFilter(SKDReportFilter filter)
		{
			var documentFilter = filter as DocumentsReportFilter;
			if (documentFilter == null)
				return;
			Overtime = documentFilter.Overtime;
			Presence = documentFilter.Presence;
			Abcense = documentFilter.Abcense;
			AbcenseReasonable = documentFilter.AbcenseReasonable;
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var documentFilter = filter as DocumentsReportFilter;
			if (documentFilter == null)
				return;
			documentFilter.Overtime = Overtime;
			documentFilter.Presence = Presence;
			documentFilter.Abcense = Abcense;
			documentFilter.AbcenseReasonable = AbcenseReasonable;
		}
	}
}
