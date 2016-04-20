using RubezhAPI.SKD.ReportFilters;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.SKDReports;

namespace SKDModule.Reports.ViewModels
{
	public class PassCardTypePageViewModel : FilterContainerViewModel
	{
		public PassCardTypePageViewModel()
		{
			Title = "Типы пропусков";
		}

		bool _passCardActive;
		public bool PassCardActive
		{
			get { return _passCardActive; }
			set
			{
				_passCardActive = value;
				OnPropertyChanged(() => PassCardActive);
			}
		}
		bool _passCardInactive;
		public bool PassCardInactive
		{
			get { return _passCardInactive; }
			set
			{
				_passCardInactive = value;
				OnPropertyChanged(() => PassCardInactive);
			}
		}

		bool _allowInactive;
		public bool AllowInactive
		{
			get { return _allowInactive; }
			set
			{
				_allowInactive = value;
				OnPropertyChanged(() => AllowInactive);
			}
		}

		public override void LoadFilter(SKDReportFilter filter)
		{
			var passCardTypeFilter = filter as IReportFilterPassCardType;
			if (passCardTypeFilter == null)
				return;
			PassCardActive = passCardTypeFilter.PassCardActive;
			var fullPassCardTypeFilter = passCardTypeFilter as IReportFilterPassCardTypeFull;
			AllowInactive = fullPassCardTypeFilter != null;
			if (AllowInactive)
				PassCardInactive = fullPassCardTypeFilter.PassCardInactive;
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var passCardTypeFilter = filter as IReportFilterPassCardType;
			if (passCardTypeFilter == null)
				return;
			passCardTypeFilter.PassCardActive = PassCardActive;
			var fullPassCardTypeFilter = passCardTypeFilter as IReportFilterPassCardTypeFull;
			if (fullPassCardTypeFilter != null)
				fullPassCardTypeFilter.PassCardInactive = PassCardInactive;
		}
	}
}