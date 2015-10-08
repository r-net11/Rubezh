using DevExpress.XtraReports.UI;
using Resurs.Reports.DataSources;

namespace Resurs.Reports.Templates
{
	public partial class DebtorsReport : XtraReport
	{
		public DebtorsReport()
		{
			InitializeComponent();
			var dataSet = new DebtorsDataSet();
			for (int i = 0; i < 10; i++)
			{
				var dataRow = dataSet.Data.NewDataRow();
				dataRow.AbonentID = i;
				dataRow.Debt = -i * 432;
				dataRow.AbonentName = string.Format("Имя {0}", i);
				dataSet.Data.Rows.Add(dataRow);
			}
			DataSource = dataSet;
		}
	}
}