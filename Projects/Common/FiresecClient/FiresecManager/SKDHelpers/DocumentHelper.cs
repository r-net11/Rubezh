using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class DocumentHelper
	{
		public static void MarkDeleted(Document document)
		{
			FiresecManager.FiresecService.MarkDeletedDocuments(new List<Document> { document });
		}

		public static void Save(Document document)
		{
			FiresecManager.FiresecService.SaveDocuments(new List<Document> { document });
		}
	}
}
