using Infrastructure.Common.Windows.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Infrastructure.Common.Windows.DataTemplates
{
	internal class MulticlientDataTemplateSelector : HierarhicalDataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var root = GetLogicalRoot(container);
			var template = root is ContentPresenter ? SelectTamplate(typeof(HeaderedWindowViewModel), container) : base.SelectTemplate(item, container);
			return template;
		}
	}
}