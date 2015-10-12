
namespace RubezhAPI.SKD.ReportFilters
{
	public interface IReportFilterEmployeeAndVisitor : IReportFilterEmployee
	{
		bool IsEmployee { get; set; }
	}
}
