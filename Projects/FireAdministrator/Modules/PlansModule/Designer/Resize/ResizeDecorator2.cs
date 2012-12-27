using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Infrustructure.Plans.Designer;

namespace PlansModule.Designer.Resize
{
	public class ResizeDecorator2 : Decorator
	{
		private const Visibility _hidden = Visibility.Collapsed;
		public static readonly DependencyProperty ShowDecoratorProperty = DependencyProperty.Register("ShowDecorator", typeof(bool), typeof(ResizeDecorator2), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ShowDecoratorProperty_Changed)));
		public bool ShowDecorator
		{
			get { return (bool)GetValue(ShowDecoratorProperty); }
			set { SetValue(ShowDecoratorProperty, value); }
		}

		private static void ShowDecoratorProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ResizeDecorator2 decorator = (ResizeDecorator2)d;
			if (decorator.Visibility == Visibility.Collapsed)
				return;
			bool showDecorator = (bool)e.NewValue;
			decorator.SetChild();
			if (decorator.Child != null)
				decorator.Child.Visibility = showDecorator ? Visibility.Visible : _hidden;
		}

		public ResizeDecorator2()
		{
			DataContextChanged += (s, e) => SetChild();
		}
		private void SetChild()
		{
			if (Child == null)
			{
				DesignerItem designerItem = DataContext as DesignerItem;
				if (designerItem != null && designerItem.ResizeChrome != null)
				{
					Child = designerItem.ResizeChrome;
					designerItem.ResizeChrome.Visibility = _hidden;
				}
			}
		}
	}
}
