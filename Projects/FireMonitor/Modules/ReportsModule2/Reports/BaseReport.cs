
namespace ReportsModule2.Reports
{
	public class BaseReport
	{
		public string ReportFileName { get; protected set; }

		public virtual void LoadData()
		{
		}

		//public virtual void LoadCrystalReportDocument(ReportDocument reportDocument)
		//{
		//}
	}
}