using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class GUDHelper
	{
		public static IEnumerable<GUD> Get(GUDFilter filter)
		{
			var result = FiresecManager.FiresecService.GetGUDs(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Save(GUD gUD)
		{
			var result = FiresecManager.FiresecService.SaveGUDs(new List<GUD> { gUD });
			return Common.ShowErrorIfExists(result);
		}

		public static bool MarkDeleted(GUD gUD)
		{
			var result = FiresecManager.FiresecService.MarkDeletedGUDs(new List<GUD> { gUD });
			return Common.ShowErrorIfExists(result);
		}
	}
}
