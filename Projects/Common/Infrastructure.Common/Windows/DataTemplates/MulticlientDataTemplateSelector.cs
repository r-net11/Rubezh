using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Windows.DataTemplates
{
	class MulticlientDataTemplateSelector : HierarhicalDataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var root = GetLogicalRoot(container);
			var template = root is ContentPresenter ? SelectTamplate(typeof(HeaderedWindowViewModel), container) : base.SelectTemplate(item, container);
			return template;
		}
	}
}
