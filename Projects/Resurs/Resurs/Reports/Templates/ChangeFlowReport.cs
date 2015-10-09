using DevExpress.XtraReports.UI;
using Resurs.Reports.DataSources;
using Resurs.ViewModels;
using ResursDAL;
using System;

namespace Resurs.Reports.Templates
{
	public partial class ChangeFlowReport : XtraReport, IReport
	{
		public ChangeFlowReport()
		{
			InitializeComponent();
			var filter = ReportsViewModel.Filter;
			StartTime.Value = filter.StartDate;
			EndTime.Value = filter.EndDate;
			DeviceName.Value = filter.Device.Name;
			Address.Value = filter.Device.FullAddress;
			AbonentName.Value = "Лавров Генадий Павлович";
			var measures = DBCash.GetMeasures(filter.Device.UID, filter.StartDate, filter.EndDate);
			var dataSet = new CounterDataSet();
			foreach (var measure in measures)
			{
				var dataRow = dataSet.Data.NewDataRow();
				dataRow.DateTime = measure.DateTime;
				dataRow.Tariff = measure.TariffPartNo;
				dataRow.CounterValue = Math.Round(measure.Value,2);
				dataSet.Data.Rows.Add(dataRow);
			}
			DataSource = dataSet;
		}
		public ReportType ReportType
		{
			get { return Reports.ReportType.ChangeFlow; }
		}
	}
}