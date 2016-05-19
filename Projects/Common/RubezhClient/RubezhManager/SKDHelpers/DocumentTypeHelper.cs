using RubezhAPI.SKD;
using System;
using System.Collections.Generic;

namespace RubezhClient.SKDHelpers
{
	public static class DocumentTypeHelper
	{
		public static IEnumerable<TimeTrackDocumentType> GetByOrganisation(Guid organisationUID)
		{
			var result = ClientManager.RubezhService.GetTimeTrackDocumentTypes(organisationUID);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Add(TimeTrackDocumentType documentType)
		{
			var operationResult = ClientManager.RubezhService.AddTimeTrackDocumentType(documentType);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Edit(TimeTrackDocumentType documentType)
		{
			var operationResult = ClientManager.RubezhService.EditTimeTrackDocumentType(documentType);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Remove(Guid documentTypeUID)
		{
			var operationResult = ClientManager.RubezhService.RemoveTimeTrackDocumentType(documentTypeUID);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}