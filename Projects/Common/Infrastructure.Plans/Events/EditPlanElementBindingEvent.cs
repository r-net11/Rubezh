using Microsoft.Practices.Prism.Events;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrastructure.Plans.Events
{
	public class EditPlanElementBindingEvent : CompositePresentationEvent<EditPlanElementBindingEventArgs>
	{
	}

	public class EditPlanElementBindingEventArgs
	{
		public PlanElementBindingItem PlanElementBindingItem { get; set; }

		public static EditPlanElementBindingEventArgs Create<T>(List<PlanElementBindingItem> planElementBindingItems, Expression<Func<T>> propertyExpression)
		{
			var propertyName = ExtractPropertyName(propertyExpression);
			var planElementBindingItem = planElementBindingItems.FirstOrDefault(x => x.PropertyName == propertyName);
			if (planElementBindingItem == null)
			{
				planElementBindingItem = new PlanElementBindingItem()
				{
					PropertyName = propertyName
				};
				planElementBindingItems.Add(planElementBindingItem);
			}

			var editPlanElementBindingEventArgs = new EditPlanElementBindingEventArgs()
			{
				PlanElementBindingItem = planElementBindingItem
			};
			return editPlanElementBindingEventArgs;
		}

		static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
		{
			if (propertyExpression == null)
				throw new ArgumentNullException("propertyExpression");
			var memberExpression = propertyExpression.Body as MemberExpression;
			if (memberExpression == null)
				throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");
			var property = memberExpression.Member as PropertyInfo;
			if (property == null)
				throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");
			return property.Name;
		}
	}
}