using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using StrazhAPI.SKD.ReportFilters;

namespace SKDModule.Reports.ViewModels
{
	public class PassCardMainPageViewModel : FilterContainerViewModel
	{
		private bool _useExpirationDate;
		public bool UseExpirationDate
		{
			get { return _useExpirationDate; }
			set
			{
				_useExpirationDate = value;
				OnPropertyChanged(() => UseExpirationDate);
			}
		}
		private DateTime _expirationDate;
		public DateTime ExpirationDate
		{
			get { return _expirationDate; }
			set
			{
				_expirationDate = value;
				OnPropertyChanged(() => ExpirationDate);
			}
		}
		private EndDateType _expirationType;
		public EndDateType ExpirationType
		{
			get { return _expirationType; }
			set
			{
				_expirationType = value;
				OnPropertyChanged(() => ExpirationType);
				IsExpirationArbitrary = ExpirationType == EndDateType.Arbitrary;
				OnPropertyChanged(() => IsExpirationArbitrary);
			}
		}
		public bool IsExpirationArbitrary { get; private set; }

		public override void LoadFilter(SKDReportFilter filter)
		{
			var passCardFilter = filter as CardsReportFilter;
			if (passCardFilter == null)
				return;
			UseExpirationDate = passCardFilter.UseExpirationDate;
			ExpirationDate = passCardFilter.ExpirationDate;
			ExpirationType = passCardFilter.ExpirationType;
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var passCardFilter = filter as CardsReportFilter;
			if (passCardFilter == null)
				return;
			passCardFilter.UseExpirationDate = UseExpirationDate;
			passCardFilter.ExpirationDate = ExpirationDate;
			passCardFilter.ExpirationType = ExpirationType;
		}
	}
}
