using System;

namespace Infrastructure.Common.Windows.Navigation
{
	public class ShowOnPlanArgs<T>
	{
		public bool? RightPanelVisible { get; set; }
		public Guid? PlanUID { get; set; }
		public T Value { get; set; }

		public static implicit operator ShowOnPlanArgs<T>(T value)
		{
			return new ShowOnPlanArgs<T>()
			{
				Value = value,
				RightPanelVisible = null,
				PlanUID = null,
			};
		}
		public static implicit operator T(ShowOnPlanArgs<T> obj)
		{
			return obj.Value;
		}
	}
}
