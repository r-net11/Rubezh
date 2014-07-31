using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Converters
{
	public class DocumentTypeTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is LayoutDocument)
			{
				return DocumentTemplate;
			}
			return OtherTemplate;
		}

		public DataTemplate DocumentTemplate { get; set; }
		public DataTemplate OtherTemplate { get; set; }
	}
}
