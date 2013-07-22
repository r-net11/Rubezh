using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Controls.Converters;

namespace Controls
{
	public class PopupButton : ToggleButton
	{
		static PopupButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupButton), new FrameworkPropertyMetadata(typeof(PopupButton)));
		}

		public static readonly DependencyProperty PopupContentProperty = DependencyProperty.Register("PopupContent", typeof(object), typeof(PopupButton), new UIPropertyMetadata(null));
		public object PopupContent
		{
			get { return (object)GetValue(PopupContentProperty); }
			set { SetValue(PopupContentProperty, value); }
		}

		public PopupButton()
		{
			var content = new ContentPresenter();
			content.SetBinding(ContentPresenter.ContentProperty, new Binding("PopupContent") { Source = this });
			var border = new Border()
			{
				CornerRadius = new CornerRadius(5),
				BorderThickness = new Thickness(1),
				Child = content
			};
			border.SetResourceReference(Border.BackgroundProperty, "BaseWindowBackgroundBrush");
			border.SetResourceReference(Border.BorderBrushProperty, "WindowBorderBrush");
			_popup = new Popup()
			{
				AllowsTransparency = true,
				StaysOpen = false,
				Placement = PlacementMode.Bottom,
				PlacementTarget = this,
				DataContext = this,
				Child = border,
			};
			_popup.SetBinding(Popup.IsOpenProperty, "IsChecked");
			_popup.SetBinding(Popup.WidthProperty, "Width");
			SetBinding(PopupButton.IsHitTestVisibleProperty, new Binding("IsOpen") { Source = _popup, Mode = BindingMode.OneWay, Converter = new BoolInverterConverter() });
		}

		private Popup _popup;
	}
}
