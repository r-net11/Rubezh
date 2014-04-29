using System;
using System.Collections.Generic;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class DocumentHelper
	{
		public static bool Save(Document document)
		{
			var operationResult = FiresecManager.FiresecService.SaveDocument(document);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Guid uid)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedDocument(uid);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static Document GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var operationResult = FiresecManager.FiresecService.GetDocumentDetails(uid.Value);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortDocument> Get(DocumentFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetDocumentList(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortDocument> GetByOrganisation(Guid organisationUID)
		{
			var result = FiresecManager.FiresecService.GetDocumentList(new DocumentFilter
			{
				OrganisationUIDs = new List<System.Guid> { organisationUID }
			});
			return Common.ShowErrorIfExists(result);
		}
	}
}
