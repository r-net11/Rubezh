using DevExpress.XtraReports.UI;
using ResursDAL;
using System;

namespace Resurs.Reports.Templates
{
	public partial class ChangeFlowReport : XtraReport, IReport
	{
		public ChangeFlowReport(ReportFilter filter)
		{
			InitializeComponent();
			StartTime.Value = filter.StartDate;
			EndTime.Value = filter.EndDate;
			DeviceName.Value = filter.Device.Name;
			Address.Value = filter.Device.Address;
			AbonentName.Value = "Лавров Генадий Павлович";
			var id = Guid.Parse("7E5006B1-800E-4284-953B-EAF04B387034");
			var measures = DBCash.GetMeasures(id, filter.StartDate, filter.EndDate);
			var dataSet = new CounterDataSet();
			foreach (var measure in measures)
			{
				var dataRow = dataSet.Data.NewDataRow();
				dataRow.DateTime = measure.DateTime;
				dataRow.Tariff = measure.TariffPartNo;
				dataRow.CounterValue = measure.Value;
				dataSet.Data.Rows.Add(dataRow);
			}
			//for (int i = 0; i < 1000; i++)
			//{
			//	var dataRow = dataSet.Data.NewDataRow();
			//	dataRow.DateTime = DateTime.Now.AddDays(i);
			//	dataRow.Tariff = 3;
			//	dataRow.CounterValue = 1434 + 32 * i;
			//	dataSet.Data.Rows.Add(dataRow);
			//}
			DataSource = dataSet;
		}
		public ReportType ReportType
		{
			get { return Reports.ReportType.ChangeFlow; }
		}
	}
}