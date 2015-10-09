using ResursDAL;
using ResursAPI;
using Resurs.Reports.DataSources;
using System.Linq;
using DevExpress.XtraReports.UI;
using Resurs.ViewModels;

namespace Resurs.Reports.Templates
{
	public partial class ChangeValueReport : XtraReport
	{
		public ChangeValueReport()
		{
			InitializeComponent();
			var filter = ReportsViewModel.Filter; 
			var counters = DBCash.GetAllChildren(DBCash.RootDevice).Where(x => x.DeviceType == DeviceType.Counter).ToList();
			var dataSet = new ChangeValueDataSet();
			foreach (var counter in counters)
			{
				var dataRow = dataSet.Data.NewDataRow();
				var measures = DBCash.GetMeasures(counter.UID, filter.StartDate, filter.EndDate);
				if (measures.Count == 0)
					break;
				var firstmeasure = measures.First();
				var lastmeasure = measures.Last();
				dataRow.Name = counter.Name;
				dataRow.Id = counter.Address;
				dataRow.Tariff = "не задан";
				dataRow.OldValue = firstmeasure.Value;
				dataRow.NewValue = lastmeasure.Value;
				dataRow.ChangeValue = lastmeasure.Value -firstmeasure.Value;
				dataSet.Data.Rows.Add(dataRow);
			}
			DataSource = dataSet;
		}
	}
}