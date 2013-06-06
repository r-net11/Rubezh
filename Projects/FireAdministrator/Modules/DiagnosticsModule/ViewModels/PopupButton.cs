using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using Controls.Converters;

namespace DiagnosticsModule.ViewModels
{
	public class PopupButton : ToggleButton
	{
		static PopupButton()
		{
			//DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupButton), new FrameworkPropertyMetadata(typeof(PopupButton)));
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
				BorderThickness = new Thickness(1),
				Child = content
			};
			border.SetResourceReference(Border.BackgroundProperty, "BaseWindowBackgroundBrush");
			border.SetResourceReference(Border.BorderBrushProperty, "WindowBorderBrush");
			_popup = new Popup()
			{
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
