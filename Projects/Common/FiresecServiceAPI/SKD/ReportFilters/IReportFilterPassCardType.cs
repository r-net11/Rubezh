namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterPassCardType
	{
		bool PassCardActive { get; set; }

		bool PassCardPermanent { get; set; }

		bool PassCardTemprorary { get; set; }

		bool PassCardOnceOnly { get; set; }

		bool PassCardForcing { get; set; }

		bool PassCardLocked { get; set; }
	}
}