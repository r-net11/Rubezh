using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class AccessTemplateHelper
	{
		public static IEnumerable<AccessTemplate> Get(AccessTemplateFilter filter)
		{
			var result = FiresecManager.FiresecService.GetGUDs(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Save(AccessTemplate accessTemplate)
		{
			var result = FiresecManager.FiresecService.SaveGUDs(new List<AccessTemplate> { accessTemplate });
			return Common.ShowErrorIfExists(result);
		}

		public static bool MarkDeleted(AccessTemplate accessTemplate)
		{
			var result = FiresecManager.FiresecService.MarkDeletedGUDs(new List<AccessTemplate> { accessTemplate });
			return Common.ShowErrorIfExists(result);
		}
	}
}