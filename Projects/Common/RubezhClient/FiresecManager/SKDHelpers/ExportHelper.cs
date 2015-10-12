using FiresecAPI.SKD;

namespace RubezhClient.SKDHelpers
{
	public static class ExportHelper
	{
		public static bool ExportOrganisation(ExportFilter filter)
		{
			var operationResult = ClientManager.FiresecService.ExportOrganisation(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool ImportOrganisation(ImportFilter filter)
		{
			var operationResult = ClientManager.FiresecService.ImportOrganisation(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool ExportOrganisationList(ExportFilter filter)
		{
			var operationResult = ClientManager.FiresecService.ExportOrganisationList(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool ImportOrganisationList(ImportFilter filter)
		{
			var operationResult = ClientManager.FiresecService.ImportOrganisationList(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool ExportJournal(JournalExportFilter filter)
		{
			var result = ClientManager.FiresecService.ExportJournal(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static bool ExportConfiguration(ConfigurationExportFilter filter)
		{
			var result = ClientManager.FiresecService.ExportConfiguration(filter);
			return Common.ShowErrorIfExists(result);
		}
	}
}
