namespace RubezhAPI.SKD.ReportFilters
{
	public interface IReportFilterZoneWithDirection : IReportFilterZone
	{
		bool ZoneIn { get; set; }
		bool ZoneOut { get; set; }
	}
}