namespace StrazhAPI.SKD.ReportFilters
{
	public interface IReportFilterPassCardTypeFull : IReportFilterPassCardType
	{
		bool PassCardInactive { get; set; }
	}
}