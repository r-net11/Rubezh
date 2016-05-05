using StrazhAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class ExportHelper
	{
		public static bool ExportOrganisation(ExportFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.ExportOrganisation(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool ImportOrganisation(ImportFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.ImportOrganisation(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool ExportOrganisationList(ExportFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.ExportOrganisationList(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool ImportOrganisationList(ImportFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.ImportOrganisationList(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool ExportJournal(JournalExportFilter filter)
		{
			var result = FiresecManager.FiresecService.ExportJournal(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static bool ExportConfiguration(ConfigurationExportFilter filter)
		{
			var result = FiresecManager.FiresecService.ExportConfiguration(filter);
			return Common.ShowErrorIfExists(result);
		}
	}
}
