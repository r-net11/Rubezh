using System;
using System.Windows;
using System.Windows.Controls;

namespace Controls.TemplateSelectors
{
	public class InterfaceTemplateSelector : DataTemplateSelector
	{
		public InterfaceTemplateSelector()
		{
			UseDataKey = false;
		}

		public Type Type { get; set; }

		public bool UseDataKey { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var dataTemplate = base.SelectTemplate(item, container);
			FrameworkElement element = container as FrameworkElement;
			if (dataTemplate == null && item != null && UseDataKey)
			{
				var key = new DataTemplateKey(item.GetType());
				dataTemplate = (DataTemplate)element.TryFindResource(key);
			}
			if (dataTemplate == null && item != null && Type != null && Type.IsAssignableFrom(item.GetType()))
			{
				var key = new DataTemplateKey(Type);
				dataTemplate = (DataTemplate)element.FindResource(key);
			}
			return dataTemplate;
		}
	}
}