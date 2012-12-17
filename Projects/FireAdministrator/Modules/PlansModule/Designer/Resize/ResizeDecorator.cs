using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Infrustructure.Plans.Designer;

namespace PlansModule.Designer.Resize
{
	public class ResizeDecorator : Control
	{
		private Adorner _adorner;

		public static readonly DependencyProperty ShowDecoratorProperty = DependencyProperty.Register("ShowDecorator", typeof(bool), typeof(ResizeDecorator), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ShowDecoratorProperty_Changed)));
		public bool ShowDecorator
		{
			get { return (bool)GetValue(ShowDecoratorProperty); }
			set { SetValue(ShowDecoratorProperty, value); }
		}

		private static void ShowDecoratorProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ResizeDecorator decorator = (ResizeDecorator)d;
			if (decorator.Visibility == Visibility.Collapsed)
				return;
			bool showDecorator = (bool)e.NewValue;
			if (showDecorator)
				decorator.ShowAdorner();
			else
				decorator.HideAdorner();
		}

		public ResizeDecorator()
		{
			Unloaded += (s, e) => UnloadAdorner();
			DataContextChanged += (s, e) =>
				{
					//UnloadAdorner();
					//DesignerItem designerItem = DataContext as DesignerItem;
					//if (designerItem != null)
					//{
					//    _adorner = new ResizeAdorner(designerItem);
					//    _adorner.Visibility = Visibility.Hidden;
					//    AdornerLayer.Add(_adorner);
					//}
				};
		}

		private void UnloadAdorner()
		{
			if (_adorner != null)
			{
				AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
				if (adornerLayer != null)
					adornerLayer.Remove(_adorner);
				_adorner = null;
			}
		}

		private void ShowAdorner()
		{
			if (_adorner == null)
			{
				AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
				if (adornerLayer != null)
				{
					DesignerItem designerItem = DataContext as DesignerItem;
					_adorner = new ResizeAdorner(designerItem);
					adornerLayer.Add(_adorner);
					if (ShowDecorator)
						_adorner.Visibility = Visibility.Visible;
					else
						_adorner.Visibility = Visibility.Hidden;
				}
			}
			else
				_adorner.Visibility = Visibility.Visible;
		}
		private void HideAdorner()
		{
			if (_adorner != null)
				_adorner.Visibility = Visibility.Hidden;
		}
	}
}
