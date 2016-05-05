using StrazhAPI.AutomationCallback;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiresecService.Automation
{
	public static class ProcedurePropertyCache
	{
		private static object _lock = new object();
		private static object _lockPlan = new object();
		private static Dictionary<Guid, List<VisualPropertyCallbackData>> _cache = new Dictionary<Guid, List<VisualPropertyCallbackData>>();
		private static List<PlanCallbackData> _cachePlan = new List<PlanCallbackData>();

		public static void SetProperty(PlanCallbackData data)
		{
			lock (_lockPlan)
			{
				var planData = _cachePlan.FirstOrDefault(item => item.PlanUid == data.PlanUid && item.ElementUid == data.ElementUid && item.ElementPropertyType == data.ElementPropertyType);
				if (planData == null)
					_cachePlan.Add(data);
				else
				{
					planData.Value = data.Value;
					planData.LayoutFilter = data.LayoutFilter;
				}
			}
		}

		public static void SetProperty(Guid layoutUID, VisualPropertyCallbackData data)
		{
			lock (_lock)
			{
				if (!_cache.ContainsKey(layoutUID))
					_cache.Add(layoutUID, new List<VisualPropertyCallbackData>());
				var propertyData = _cache[layoutUID].FirstOrDefault(item => item.LayoutPart == data.LayoutPart && item.Property == data.Property);
				if (propertyData == null)
					_cache[layoutUID].Add(data);
				else
				{
					propertyData.Value = data.Value;
					propertyData.LayoutFilter = data.LayoutFilter;
				}
			}
		}

		public static ProcedureProperties GetProcedureProperties(Guid layoutUID)
		{
			return new ProcedureProperties()
			{
				VisualProperties = GetProperties(layoutUID),
				PlanProperties = GetPlanProperties(),
			};
		}

		private static List<VisualPropertyCallbackData> GetProperties(Guid layoutUID)
		{
			lock (_lock)
				return new List<VisualPropertyCallbackData>(_cache.ContainsKey(layoutUID) ? _cache[layoutUID] : Enumerable.Empty<VisualPropertyCallbackData>());
		}

		private static List<PlanCallbackData> GetPlanProperties()
		{
			lock (_lockPlan)
				return new List<PlanCallbackData>(_cachePlan);
		}
	}
}