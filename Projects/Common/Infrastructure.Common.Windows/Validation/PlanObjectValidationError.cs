using System;
using Infrastructure.Common.Windows.Navigation;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Common.Windows.Validation
{
	public abstract class PlanObjectValidationError<TObject, TEvent, TKey> : ObjectValidationError<TObject, TEvent, ShowOnPlanArgs<TKey>>
		where TEvent : CompositePresentationEvent<ShowOnPlanArgs<TKey>>, new()
	{
		public PlanObjectValidationError(TObject obj, string error, ValidationErrorLevel validationErrorLevel, bool? isRightPanelVisible, Guid? planUID)
			: base(obj, error, validationErrorLevel)
		{
			IsRightPanelVisible = isRightPanelVisible;
			PlanUID = planUID;
		}

		protected override ShowOnPlanArgs<TKey> Key
		{
			get
			{
				return new ShowOnPlanArgs<TKey>()
				{
					Value = KeyValue,
					RightPanelVisible = IsRightPanelVisible,
					PlanUID = PlanUID,
				};
			}
		}
		protected abstract TKey KeyValue { get; }
		public bool? IsRightPanelVisible { get; private set; }
		public Guid? PlanUID { get; private set; }
	}
}