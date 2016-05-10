using System;
using System.Collections.Generic;
using System.Linq;

namespace StrazhAPI.SKD
{
	internal static class SKDHelper
	{
		public static List<Guid> GetUIDs<T>(List<T> items)
			where T : SKDModelBase
		{
			return items != null ? items.Select(x => x.UID).ToList() : new List<Guid>();
		}

		public static Guid? GetUID<T>(T item)
			where T : SKDModelBase
		{
			if (item == null)
				return null;
			return item.UID;
		}
	}
}