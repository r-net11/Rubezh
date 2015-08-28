using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using SKDModule.Model;

namespace SKDModule.TemplateSelectors
{
	public class IsOpenedIntervalTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			//получаем вызывающий контейнер
			var element = container as FrameworkElement;
			var dayTimeTrackPart = item as DayTimeTrackPart;

			if (element == null || dayTimeTrackPart == null) return null;

			if (dayTimeTrackPart.IsOpen)
				return element.FindResource("OpenedIntervalsTemplate") as DataTemplate;

			return element.FindResource("NotOpenedIntervalsTemplate") as DataTemplate;
		}
	}
}
