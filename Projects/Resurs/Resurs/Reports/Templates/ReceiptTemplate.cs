using System;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Resurs.ViewModels;
using ResursAPI;
using ResursDAL;
using Resurs.Reports.DataSources;

namespace Resurs.Reports.Templates
{
	public partial class ReceiptTemplate : DevExpress.XtraReports.UI.XtraReport
	{
		public ReceiptTemplate()
		{
			InitializeComponent();
		}
		public override void CreateDocument(bool buildPagesInBackground)
		{
			var bill = BillViewModel.Bill;
			ФИО.Value = bill.Consumer.FIO;
			Адрес.Value = bill.Consumer.Address;
			Счет.Value = bill.Name;
			Баланс.Value = bill.Balance.ToString();
			var dataSet = new ReceiptTemplateDataSet();
			foreach (var device in bill.Devices)
			{
				var dataRow = dataSet.Data.NewDataRow();
				dataRow.Счетчик = device.Description;
				var measures = DBCash.GetMeasures(device.UID, new DateTime(2015, 10, 01), DateTime.Now);
				var firstMeasure = measures.FirstOrDefault();
				var lastMeasure = measures.LastOrDefault();
				dataRow.Тариф_1 = Math.Round(lastMeasure.Value - firstMeasure.Value, 2);
				dataRow.Тариф_2 = 0;
				dataRow.Тариф_3 = 0;
				dataRow.Тариф_4 = 0;
				dataRow.К_оплате = 0;
				dataSet.Data.Rows.Add(dataRow);
			}
			DataSource = dataSet;
			base.CreateDocument(buildPagesInBackground);
		}
	}
}
