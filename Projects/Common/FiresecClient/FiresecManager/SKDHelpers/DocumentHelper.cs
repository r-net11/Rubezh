using System;
using System.Collections.Generic;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class DocumentHelper
	{
		public static bool Save(Document document)
		{
			var operationResult = FiresecManager.FiresecService.SaveDocuments(new List<Document> { document });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Guid uid)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedDocuments(new List<Guid> { uid });
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

		public static IEnumerable<ShortDocument> GetByOrganization(Guid? organizationUID)
		{
			if (organizationUID == null)
				return null;
			var filter = new DocumentFilter();
			filter.OrganizationUIDs.Add(organizationUID.Value);
			var operationResult = FiresecManager.FiresecService.GetDocumentList(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
