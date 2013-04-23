using System;
using System.Windows;
using System.Windows.Controls;
using Infrastructure.Common.Windows.ViewModels;
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

			if (root is ApplicationView)
			{
				if (item is ShellViewModel)
					type = typeof(ShellViewModel);
			}
			else if (root is DialogView)
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
			template = SelectTamplate(type, container);
			return template ?? base.SelectTemplate(item, container);
		}
		protected DataTemplate SelectTamplate(Type type, DependencyObject container)
		{
			DataTemplate template = null;
			if (type != null)
			{
				FrameworkElement element = container as FrameworkElement;
				var key = new DataTemplateKey(type);
				template = (DataTemplate)element.FindResource(key);
			}
			return template;
		}
		protected DependencyObject GetLogicalRoot(DependencyObject container)
		{
			var root = container;
			DependencyObject obj = container;
			while ((obj = LogicalTreeHelper.GetParent(root)) != null)
				root = obj;
			return root;
		}
	}
}