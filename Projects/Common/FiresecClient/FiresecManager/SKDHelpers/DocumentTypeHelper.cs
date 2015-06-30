using System;
using System.Collections.Generic;
using FiresecAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class DocumentTypeHelper
	{
		public static IEnumerable<TimeTrackDocumentType> GetByOrganisation(Guid organisationUID)
		{
			var result = FiresecManager.FiresecService.GetTimeTrackDocumentTypes(organisationUID);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Add(TimeTrackDocumentType documentType)
		{
			var operationResult = FiresecManager.FiresecService.AddTimeTrackDocumentType(documentType);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Edit(TimeTrackDocumentType documentType)
		{
			var operationResult = FiresecManager.FiresecService.EditTimeTrackDocumentType(documentType);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Remove(Guid documentTypeUID)
		{
			var operationResult = FiresecManager.FiresecService.RemoveTimeTrackDocumentType(documentTypeUID);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}