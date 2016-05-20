using DevExpress.XtraReports.UI;
using RubezhAPI.SKD.ReportFilters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;

namespace RubezhService.Report.Templates
{
	public partial class BaseReport : DevExpress.XtraReports.UI.XtraReport, IFilteredReport
	{
		private float _topMargin;
		private float _bottomMargin;

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

		protected SKDReportFilter Filter { get; private set; }
		protected T GetFilter<T>()
			where T : SKDReportFilter
		{
			return (T)Filter ?? Activator.CreateInstance<T>();
		}

		#region IFilteredReport Members

		public virtual void ApplyFilter(SKDReportFilter filter)
		{
			Filter = filter;
		}

		#endregion

		protected override void OnBeforePrint(PrintEventArgs e)
		{
			Landscape = ForcedLandscape;
			base.OnBeforePrint(e);
			TopMargin.HeightF = _topMargin;
			BottomMargin.HeightF = _bottomMargin;

			ReportName.Value = Filter.PrintFilterNameInHeader ? string.Format("{0} ({1})", ReportTitle, Filter.Name) : ReportTitle;
			FilterName.Value = Filter.Name;
			Timestamp.Value = Filter.Timestamp;
			UserName.Value = Filter.User;
			var periodFilter = Filter as IReportFilterPeriod;
			if (periodFilter != null)
				Period.Value = string.Format("c {0:dd.MM.yyyy HH:mm:ss} по {1:dd.MM.yyyy HH:mm:ss}", periodFilter.DateTimeFrom, periodFilter.DateTimeTo);

			lTimestamp.Visible = Filter.PrintDate;
			lFilterName.Visible = Filter.PrintFilterName;
			lPeriod.Visible = Filter.PrintPeriod && periodFilter != null;
			lUserName.Visible = Filter.PrintUser;

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
			if (!IsNotDataBase)
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
			if (IsNotDataBase)
			{
				DataSet = CreateDataSet();
				//UpdateDataSource(dataProvider);
				DataSource = DataSet;
				ApplySort();
			}
		}

		protected DataSet DataSet { get; private set; }

		/// <summary>
		/// Фиксированная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>
		protected virtual bool ForcedLandscape
		{
			get;
			set;
		}

		protected virtual bool IsNotDataBase
		{
			get { return false; }
		}

		protected virtual DataSet CreateDataSet(DataProvider dataProvider)
		{
			return new DataSet();
		}

		protected virtual DataSet CreateDataSet()
		{
			return new DataSet();
		}
		protected virtual void UpdateDataSource(DataProvider dataProvider)
		{
		}

		protected virtual void ApplySort()
		{
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

		protected void FillTestData(int count = 20)
		{
			if (!DataSet.Tables.Contains(DataMember))
				throw new ApplicationException();
			var dt = DataSet.Tables[DataMember];
			FillTestData(dt, count);
		}
		protected void FillTestData(DataTable table, int count)
		{
			for (int i = 0; i < count; i++)
			{
				var row = table.NewRow();
				foreach (DataColumn column in table.Columns)
				{
					if (column.DataType == typeof(string))
						row[column] = string.Format("{0} {1}", column.ColumnName, i);
					else if (column.DataType == typeof(int) || column.DataType == typeof(long))
						row[column] = i;
					else if (column.DataType == typeof(double) || column.DataType == typeof(decimal))
						row[column] = i;
					else if (column.DataType == typeof(DateTime))
						row[column] = DateTime.Today.AddDays(-i);
					else if (column.DataType == typeof(TimeSpan))
						row[column] = new TimeSpan(i, i + 1, i + 2);
					else if (column.DataType == typeof(bool))
						row[column] = i % 2 == 0;
					else if (column.DataType == typeof(Guid))
						row[column] = Guid.NewGuid();
				}
				table.Rows.Add(row);
			}
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
		private string BuildFilterString()
		{
			var sb = new StringBuilder();
			sb.AppendLine("ФИЛЬТР:");
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
		///<summary>
		/// Метод позволяет вручную создать исключительную ситуацию при создании отчета.
		/// Исключение обробатывается внутри сборки DevExpress.
		/// Вызванное исключение может быть обработано на клиенте с помощью события CreateDocumentError.
		/// </summary>
		protected void ThrowException(string message)
		{
			throw new Exception(message);
		}
	}
}
