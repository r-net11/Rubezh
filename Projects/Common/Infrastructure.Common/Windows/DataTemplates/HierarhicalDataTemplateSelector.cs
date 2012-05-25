using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows.Media;
using Infrastructure.Common.Windows.Views;

namespace Infrastructure.Common.Windows.DataTemplates
{
	public class HierarhicalDataTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			Type type = null;
			DataTemplate template = null;
			DependencyObject root = GetLogicalRoot(container);

			if (root is DialogView)
			{
				if (item is SaveCancelDialogViewModel)
					type = typeof(SaveCancelDialogViewModel);
				else if (item is MessageBoxViewModel)
					type = typeof(MessageBoxViewModel);
			}
			else if (root is HeaderedWindowView)
			{
				if (item is DialogViewModel)
					type = typeof(DialogViewModel);
				else if (item is ApplicationViewModel)
					type = typeof(ApplicationViewModel);
			}
			else if (root is WindowBaseView)
			{
				if (item is HeaderedWindowViewModel)
					type = typeof(HeaderedWindowViewModel);
			}

			if (type != null)
			{
				FrameworkElement element = container as FrameworkElement;
				var key = new DataTemplateKey(type);
				template = (DataTemplate)element.FindResource(key);
			}
			return template ?? base.SelectTemplate(item, container);
		}
		private DependencyObject GetLogicalRoot(DependencyObject container)
		{
			var root = container;
			DependencyObject obj = container;
			while ((obj = LogicalTreeHelper.GetParent(root)) != null)
				root = obj;
			return root;
		}
	}
}
