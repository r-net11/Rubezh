using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.AutomationCallback;

namespace FiresecService.Automation
{
	public static class LayoutPropertyCache
	{
		private static object _lock = new object();
		private static Dictionary<Guid, List<VisualPropertyData>> _cache = new Dictionary<Guid, List<VisualPropertyData>>();

		public static void SetProperty(Guid layoutUID, VisualPropertyData data)
		{
			lock (_lock)
			{
				if (!_cache.ContainsKey(layoutUID))
					_cache.Add(layoutUID, new List<VisualPropertyData>());
				var propertyData = _cache[layoutUID].FirstOrDefault(item => item.LayoutPart == data.LayoutPart && item.Property == data.Property);
				if (propertyData == null)
					_cache[layoutUID].Add(data);
				else
					propertyData.Value = data.Value;
			}
		}
		public static List<VisualPropertyData> GetProperties(Guid layoutUID)
		{
			lock (_lock)
				return new List<VisualPropertyData>(_cache.ContainsKey(layoutUID) ? _cache[layoutUID] : Enumerable.Empty<VisualPropertyData>());
		}
	}
}