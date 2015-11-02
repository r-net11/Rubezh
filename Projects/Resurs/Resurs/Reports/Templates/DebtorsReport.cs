using DevExpress.XtraReports.UI;
using Resurs.Reports.DataSources;
using Resurs.ViewModels;
using ResursDAL;

namespace Resurs.Reports.Templates
{
	public partial class DebtorsReport : XtraReport
	{
		public DebtorsReport()
		{
			InitializeComponent();
			var dataSet = new DebtorsDataSet();
			var filter = ReportsViewModel.Filter;
			var consumers = DbCache.GetAllConsumers();
			MinDebt.Value = filter.MinDebt;
			foreach (var consumer in consumers)
			{
				var dataRow = dataSet.Data.NewDataRow();
				dataRow.AbonentID = consumer.Address;
				dataRow.Debt = 0;
				dataRow.AbonentName = consumer.Name;
				dataSet.Data.Rows.Add(dataRow);
			}
			DataSource = dataSet;
		}
	}
}