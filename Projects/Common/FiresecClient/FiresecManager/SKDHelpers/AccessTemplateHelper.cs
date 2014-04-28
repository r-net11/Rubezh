using System;
using System.Collections.Generic;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class AccessTemplateHelper
	{
		public static IEnumerable<AccessTemplate> Get(AccessTemplateFilter filter)
		{
			var result = FiresecManager.FiresecService.GetAccessTemplates(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Save(AccessTemplate accessTemplate)
		{
			var result = FiresecManager.FiresecService.SaveAccessTemplate(accessTemplate);
			return Common.ShowErrorIfExists(result);
		}

		public static bool MarkDeleted(AccessTemplate accessTemplate)
		{
			return MarkDeleted(accessTemplate.UID);
		}

		public static bool MarkDeleted(Guid uid)
		{
			var result = FiresecManager.FiresecService.MarkDeletedAccessTemplate(uid);
			return Common.ShowErrorIfExists(result);
		}
	}
}