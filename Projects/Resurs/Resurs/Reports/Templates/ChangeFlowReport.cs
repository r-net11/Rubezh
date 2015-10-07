﻿using DevExpress.XtraReports.UI;
using Resurs.Reports.DataSources;
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
			var measures = DBCash.GetMeasures(filter.Device.UID, filter.StartDate, filter.EndDate);
			var dataSet = new CounterDataSet();
			foreach (var measure in measures)
			{
				var dataRow = dataSet.Data.NewDataRow();
				dataRow.DateTime = measure.DateTime;
				dataRow.Tariff = measure.TariffPartNo;
				dataRow.CounterValue = measure.Value;
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