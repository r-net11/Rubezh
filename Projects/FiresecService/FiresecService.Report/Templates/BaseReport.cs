using DevExpress.XtraReports.UI;
using Localization.FiresecService.Report.Common;
using StrazhAPI.SKD.ReportFilters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;

namespace FiresecService.Report.Templates
{
	public partial class BaseReport : XtraReport, IFilteredReport
	{
		private readonly float _topMargin;
		private readonly float _bottomMargin;

		protected BaseReport()
		{
			InitializeComponent();
			_topMargin = TopMargin.HeightF;
			_bottomMargin = BottomMargin.HeightF;
			Name = ReportTitle;
		}

		public virtual string ReportTitle
		{
			get { return ""; }
		}



		protected T GetFilter<T>()
			where T : SKDReportFilter
		{
			return (T)Filter ?? Activator.CreateInstance<T>();
		}

		#region IFilteredReport Members

		public SKDReportFilter Filter { get; private set; }

		public virtual void ApplyFilter(SKDReportFilter filter)
		{
			Filter = filter;
		}

		#endregion IFilteredReport Members

		protected override void OnBeforePrint(PrintEventArgs e)
		{
			Landscape = ForcedLandscape;
			base.OnBeforePrint(e);
			TopMargin.HeightF = _topMargin;
			BottomMargin.HeightF = _bottomMargin;

			if (Filter != null)
			{
				ReportName.Value = Filter.PrintFilterNameInHeader
					? string.Format("{0} ({1})", ReportTitle, Filter.Name)
					: ReportTitle;
				FilterName.Value = Filter.Name;
				Timestamp.Value = Filter.Timestamp;
				UserName.Value = Filter.User;
				var periodFilter = Filter as IReportFilterPeriod;
				if (periodFilter != null)
					Period.Value = string.Format(CommonResources.FromDateToDate, periodFilter.DateTimeFrom,
						periodFilter.DateTimeTo);

				lTimestamp.Visible = Filter.PrintDate;
				lFilterName.Visible = Filter.PrintFilterName;
				lPeriod.Visible = Filter.PrintPeriod && periodFilter != null;
				lUserName.Visible = Filter.PrintUser;
			}

			if (!lFilterName.Visible)
				TopMargin.HeightF -= lFilterName.HeightF;
			if (!lPeriod.Visible)
				TopMargin.HeightF -= lPeriod.HeightF;
			if (!lTimestamp.Visible)
				BottomMargin.HeightF -= lTimestamp.HeightF;
			if (!lUserName.Visible && lTimestamp.Visible)
				BottomMargin.HeightF -= lUserName.HeightF;
			ReportPrintOptions.DetailCountOnEmptyDataSource = 0;

			var width = PageWidth - Margins.Left - Margins.Right;
			lReportName.WidthF = width;
			lFilterName.WidthF = width;
			lPeriod.WidthF = width;
			lPage.LocationF = new PointF(width - lPage.WidthF, lPage.LocationF.Y);
		}

		private void BaseReport_DataSourceDemanded(object sender, EventArgs e)
		{
			if (DesignMode)
				return;
			DataSourceRequered();
		}

		protected virtual void DataSourceRequered()
		{
			using (var dataProvider = new DataProvider())
			{
				DataSet = CreateDataSet(dataProvider);
				UpdateDataSource(dataProvider);
				DataSource = DataSet;
#if DEBUG
				//PrintFilter();
#endif
				ApplySort();
			}
		}

		protected DataSet DataSet { get; private set; }

		/// <summary>
		/// Фиксированная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>
		protected virtual bool ForcedLandscape
		{
			get { return false; }
		}

		protected virtual DataSet CreateDataSet(DataProvider dataProvider)
		{
			return new DataSet();
		}

		protected virtual void UpdateDataSource(DataProvider dataProvider)
		{
		}

		protected virtual void ApplySort()
		{
			if (Filter == null) return;
			if (string.IsNullOrEmpty(Filter.SortColumn) && DataSet.Tables.Contains(DataMember))
			{
				var table = DataSet.Tables[DataMember];
				if (table.Columns.Count == 0)
					return;
				Filter.SortColumn = table.Columns[0].ColumnName;
				Filter.SortAscending = true;
			}
			var details = GetDetailBand();
			if (details == null)
				return;
			details.SortFields.Add(new GroupField(Filter.SortColumn, Filter.SortAscending ? XRColumnSortOrder.Ascending : XRColumnSortOrder.Descending));
		}

		protected DetailBand GetDetailBand()
		{
			return Bands.OfType<DetailBand>().FirstOrDefault();
		}

		protected virtual T GetCurrentRow<T>()
			where T : class
		{
			var rowView = GetCurrentRow() as DataRowView;
			if (rowView == null)
				return default(T);
			return rowView.Row as T;
		}

		protected void PrintFilter()
		{
			var footer = new ReportFooterBand()
			{
				Borders = DevExpress.XtraPrinting.BorderSide.All,
				BorderWidth = 3,
				KeepTogether = true,
			};
			var label = new XRLabel()
			{
				CanGrow = true,
				CanShrink = true,
				Multiline = true,
				LocationF = new PointF(0, 20),
				TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft,
				WordWrap = true,
				Text = BuildFilterString(),
			};
			footer.Controls.Add(label);
			Bands.Add(footer);
			label.WidthF = footer.RightF;
		}

		protected override void BeforeReportPrint()
		{
			base.BeforeReportPrint();

			this.lFilterName.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding(this.FilterName, "Text", CommonResources.FilterWithColon)});
			this.lPeriod.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding(this.Period, "Text", CommonResources.DuringPeriod)});
			this.lUserName.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding(this.UserName, "Text", CommonResources.UserWithColon)});
			this.lTimestamp.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
			new DevExpress.XtraReports.UI.XRBinding(this.Timestamp, "Text", CommonResources.ReportDate)});
			this.lPage.Format = CommonResources.PageOf;

		}

		private string BuildFilterString()
		{
			var sb = new StringBuilder();
			sb.AppendLine(CommonResources.FilterUpperCase);
			foreach (var property in Filter.GetType().GetProperties().OrderBy(prop => prop.Name))
			{
				var propType = property.PropertyType;
				var value = property.GetValue(Filter, new object[0]);
				if (propType == typeof(List<Guid>))
					sb.AppendFormat("{0} = {{{1}}}\r\n", property.Name, value == null ? "NULL" : string.Join(",", ((List<Guid>)value).ToArray()));
				else
					sb.AppendFormat("{0} = '{1}'\r\n", property.Name, value);
			}
			return sb.ToString();
		}
	}
}