using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using Infrastructure.Common.Windows;

namespace FiresecClient.SKDHelpers
{
	public static class DocumentHelper
	{
		public static bool Save(Document document)
		{
			var operatonResult = FiresecManager.FiresecService.SaveDocuments(new List<Document> { document });
			if (operatonResult.HasError)
			{
				MessageBoxService.ShowWarning(operatonResult.Error);
				return false;
			}
			return true;
		}

		public static bool MarkDeleted(Document document)
		{
			var operatonResult = FiresecManager.FiresecService.MarkDeletedDocuments(new List<Document> { document });
			if (operatonResult.HasError)
			{
				MessageBoxService.ShowWarning(operatonResult.Error);
				return false;
			}
			return true;
		}

		public static Document GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new DocumentFilter();
			filter.Uids.Add((Guid)uid);
			var operationResult = FiresecManager.FiresecService.GetDocuments(filter);
			if (operationResult.HasError)
			{
				MessageBoxService.ShowWarning(operationResult.Error);
				return null;
			}
			else
				return operationResult.Result.FirstOrDefault();
		}

		public static IEnumerable<Document> Get(DocumentFilter filter)
		{
			var operatonResult = FiresecManager.FiresecService.GetDocuments(filter);
			if (operatonResult.HasError)
			{
				MessageBoxService.ShowWarning(operatonResult.Error);
				return null;
			}
			return operatonResult.Result;
		}
	}
}
