﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using FiresecAPI.SKD.ReportFilters;

namespace SKDModule.Reports.ViewModels
{
	public class DocumentFilterPageViewModel : FilterContainerViewModel
	{
		public DocumentFilterPageViewModel()
		{
			Title = "Документ";
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

		public override void LoadFilter(SKDReportFilter filter)
		{
			var documentFilter = filter as DocumentsReportFilter;
			if (documentFilter == null)
				return;
			Overtime = documentFilter.Overtime;
			Presence = documentFilter.Presence;
			Abcense = documentFilter.Abcense;
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var documentFilter = filter as DocumentsReportFilter;
			if (documentFilter == null)
				return;
			documentFilter.Overtime = Overtime;
			documentFilter.Presence = Presence;
			documentFilter.Abcense = Abcense;
		}
	}
}
