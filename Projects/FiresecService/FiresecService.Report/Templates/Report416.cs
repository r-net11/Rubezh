using System;
using Common;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;
using FiresecAPI.SKD.ReportFilters;
using System.Text;
using System.Collections.Generic;
using FiresecService.Report.DataSources;

namespace FiresecService.Report.Templates
{
	public partial class Report416 : BaseSKDReport
	{
		public Report416()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Список должностей организации"; }
		}
		protected override DataSet CreateDataSet()
		{
			return new DataSet416();
		}
		protected override string BuildSelectRoutine()
		{
			return @"select Position.Name as Position, Position.Description as Description from dbo.Position Position";
		}
		protected override string BuildWhereRouting()
		{
			var filter = GetFilter<ReportFilter416>();
			if (filter.Organisations.IsEmpty())
				filter.Organisations = new List<Guid>() { DataHelper.GetDefaultOrganisation() };
			var sb = new StringBuilder();
			SqlBuilder.BuildConditionOR(sb, "Position.OrganisationUID", filter.Organisations);
			if (!filter.Organisations.IsEmpty() && !filter.Positions.IsEmpty())
				sb.Append(SqlBuilder.AND);
			SqlBuilder.BuildConditionOR(sb, "Position.UID", filter.Positions);
			return sb.ToString();
		}
	}
}