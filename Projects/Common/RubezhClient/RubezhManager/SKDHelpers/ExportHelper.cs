using RubezhAPI.SKD;

namespace RubezhClient.SKDHelpers
{
	public static class ExportHelper
	{
		public static bool ExportOrganisation(ExportFilter filter)
		{
			var operationResult = ClientManager.RubezhService.ExportOrganisation(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool ImportOrganisation(ImportFilter filter)
		{
			var operationResult = ClientManager.RubezhService.ImportOrganisation(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool ExportOrganisationList(ExportFilter filter)
		{
			var operationResult = ClientManager.RubezhService.ExportOrganisationList(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool ImportOrganisationList(ImportFilter filter)
		{
			var operationResult = ClientManager.RubezhService.ImportOrganisationList(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool ExportJournal(JournalExportFilter filter)
		{
			var result = ClientManager.RubezhService.ExportJournal(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static bool ExportConfiguration(ConfigurationExportFilter filter)
		{
			var result = ClientManager.RubezhService.ExportConfiguration(filter);
			return Common.ShowErrorIfExists(result);
		}
	}
}
