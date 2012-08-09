using System;
using System.Windows;
using System.Windows.Controls;

namespace Controls.TemplateSelectors
{
	public class InterfaceTemplateSelector : DataTemplateSelector
	{
		public Type Type { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var dataTemplate = base.SelectTemplate(item, container);
			if (dataTemplate == null && Type != null && Type.IsAssignableFrom(item.GetType()))
			{
				var key = new DataTemplateKey(Type);
				FrameworkElement element = container as FrameworkElement;
				dataTemplate = (DataTemplate)element.FindResource(key);
			}
			return dataTemplate;
		}
	}
}
