namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterPassCardType
	{
		bool PassCardActive { get; set; }

		bool PassCardPermanent { get; set; }

		bool PassCardTemprorary { get; set; }

		bool PassCardGuest { get; set; }

		bool PassCardForcing { get; set; }

		bool PassCardLocked { get; set; }
	}
}