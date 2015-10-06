using DevExpress.XtraReports.UI;
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
			var dataSet = new CounterDataSet();
			for (int i = 0; i < 1000; i++)
			{
				var dataRow = dataSet.Data.NewDataRow();
				dataRow.DateTime = DateTime.Now.AddDays(i);
				dataRow.Tariff = 3;
				dataRow.CounterValue = 1434 + 32 * i;
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